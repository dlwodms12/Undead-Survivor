using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f;
    private float damage;
    private Vector2 direction;
    private Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, Vector2 direction)
    {
        this.damage = damage;
        this.direction = direction;

        // 총알이 날아가는 방향을 바라보게 회전 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive) { rigid.velocity = Vector2.zero; return; }
        rigid.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //충돌한 오브젝트에서 Player 컴포넌트 가져오기
            Player playerScript = collision.GetComponent<Player>();

            if (playerScript != null)
            {
                // 플레이어의 대미지 함수를 호출하며 총알 대미지를 전달
                playerScript.TakeDamage(damage);
            }

            // 풀 반환 (비활성화)
            gameObject.SetActive(false);
        }
    }
}