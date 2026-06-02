using System.Collections;
using UnityEngine;

public class ExpCoin : MonoBehaviour
{
    //종류별 코인을 구조체로 정의
    public enum CoinType { Melee, Ranged }
    public CoinType type;

    [Header("# Lifetime Settings")]
    public float maxLifetime = 120f; // 인스펙터에서 조정 가능 (기본 2분)
    private float lifeTimer;

    [Header("# Exp Settings")]
    public int baseExp = 1;          // 기본 경험치 양
    private int finalExp;            // 최종 획득 경험치 양

    private Transform playerTransform;
    private Player playerScript;
    private bool isAttracted = false; // 흡입 시작 여부
    private Vector3 originalScale;    // 기본 크기 저장용

    void Awake()
    {
        originalScale = transform.localScale;
    }

    //플레이어 찾아오는 로직 분리
    void FindPlayerReference()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            playerScript = GameManager.instance.player;
            playerTransform = playerScript.transform;
        }
    }

    void OnEnable()
    {
        // 풀에서 켜질 때마다 상태 초기화
        lifeTimer = 0f;
        isAttracted = false;
        transform.localScale = originalScale;

        // Z축을 강제로 0으로 고정하여 2D 거리 연산 오류 방지
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        //켜질 때 플레이어 찾기 1회 시도
        FindPlayerReference();
    }

    void Update()
    {
        if (!GameManager.instance.isLive) return;

        // 수명 타이머 체크
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifetime)
        {
            //풀에 반환
            Deactivate();
            return;
        }

        //플레이어를 아직 찾지 못한 상태라면 다시 찾기 시도
        if (playerTransform == null)
        {
            FindPlayerReference();
        }
        // 다시 찾았는데도 못찾았다면 에러방지를 위해 스킵
        if (playerTransform == null) return;

        // 자석 흡입 체크 (아직 흡입이 시작되지 않았을 때만 거리 측정)
        if (!isAttracted && playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            // 추후 자석 아이템으로 playerScript.magnetRadius가 커지면 자동으로 감지 범위가 넓어짐
            if (distance <= playerScript.magnetRadius)
            {
                Debug.Log("코루틴 실행");
                StartCoroutine(BounceAndSuckRoutine());
            }
        }
    }

    // 근거리 몬스터용 초기화 (티어별 크기 및 경험치 1.2배 가중치)
    public void InitMelee(int tier)
    {
        type = CoinType.Melee;

        // 거듭제곱(Pow)을 이용해 배수량이 티어당 1.2배씩 증가
        float multiplier = Mathf.Pow(1.2f, tier);

        finalExp = Mathf.RoundToInt(baseExp * multiplier);
        if (finalExp < 1) finalExp = 1; // 최소 경험치 1 보장

        transform.localScale = originalScale * multiplier;
    }

    // 원거리 몬스터용 초기화
    public void InitRanged()
    {
        type = CoinType.Ranged;
        finalExp = baseExp; // 원거리 코인의 경험치 밸런스는 여기서 조절 가능
        transform.localScale = originalScale;
    }

    // 튕겨나갔다가 빨려들어가는 연출 코루틴
    IEnumerator BounceAndSuckRoutine()
    {
        isAttracted = true;

        // 플레이어 반대 방향으로 살짝 튕겨나가기 (Bounce)
        Vector2 playerPos2D = playerTransform.position;
        Vector2 coinPos2D = transform.position;
        Vector2 bounceDir = (coinPos2D - playerPos2D).normalized;

        if (bounceDir == Vector2.zero)
        {
            //Random.insideUnitCircle : 반경 1의 원 안의 랜덤한 위치를 반환
            bounceDir = Random.insideUnitCircle.normalized; // 완전히 겹쳐있을 때의 예외 처리
        }
        float bounceDuration = 0.15f; // 튕겨나가는 시간
        float bounceSpeed = 3f;       // 초기 튕겨나가는 속도
        float elapsed = 0f; //연출 시간의 경과

        //연출 진행
        while (elapsed < bounceDuration)
        {
            if (!GameManager.instance.isLive) { yield return null; continue; }

            //Vector2를 Vector3로 변환하여 적용
            transform.position += (Vector3)(bounceDir * bounceSpeed * Time.deltaTime);

            //Lerp : 선형보간법(Linear Interpolation)의 약자.
            //시작점, 끝점, 비율로 이루어져있으며 시작점과 끝점을 연결하여 비율에 따라(0~1) 중간점을 계산
            bounceSpeed = Mathf.Lerp(bounceSpeed, 0f, elapsed / bounceDuration); // 점차 감속
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 플레이어에게 가속하며 빨려들어가기 (Suck) ---
        float suckSpeed = 2f;
        float acceleration = 15f; // 초당 추가될 가속도

        while (playerTransform != null)
        {
            if (!GameManager.instance.isLive) { yield return null; continue; }

            Vector2 targetDir = ((Vector2)playerTransform.position 
                - (Vector2)transform.position).normalized;
            float dist = Vector2.Distance(transform.position, playerTransform.position);

            // 플레이어 중심점에 완전히 근접하면 습득 완료
            if (dist <= 0.2f)
            {
                Collect();
                yield break;
            }

            //근접하지 않았다면 플레이어 중심점을 향해 가속
            suckSpeed += acceleration * Time.deltaTime; // 매 프레임 가속
            transform.position += (Vector3)(targetDir * suckSpeed * Time.deltaTime);

            yield return null;
        }
    }

    void Collect()
    {
        // 변경된 GameManager의 GetExp 호출
        GameManager.instance.GetExp(finalExp);

        // 필요 시 코인 습득 효과음 재생(오디오 찾으면 추가)
        // AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        //습득한 코인은 화면에서 제거
        Deactivate();
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
