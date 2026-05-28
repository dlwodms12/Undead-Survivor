using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    //효과음
    [Header("SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels; //채널 개수
    AudioSource[] sfxPlayers;
    int channelIndex; //가장 최근에 플레이한 채널

    public enum Sfx
    {
        //열거형은 숫자 지정 가능
        Dead, Hit, LevelUp=3, Lose, Melee, Range=7, Select, Win
    }

    private void Awake()
    {
        instance = this;
        Init();
    }

    private void Init()
    {
        //BGM 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        //오디오 매니저의 자식으로 등록(bgmPlayer의 부모 = 자신)
        bgmObject.transform.parent = transform;
        //bgmPlayer 초기화
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        //옵션 설정
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;


        //효과음 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        //오디오 매니저의 자식으로 등록(bgmPlayer의 부모 = 자신)
        sfxObject.transform.parent = transform;
        //채널 개수만큼 생성
        sfxPlayers = new AudioSource[channels];

        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        //항상 0번 채널부터 사용하는 것을 막기 위해 아래 로직을 사용
        //비어있는 AudioSource 찾기
        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            //채널을 한바퀴 돌며 비어있는 채널 찾기 (index + 최근에 플레이한 채널 수)%채널 길이(16)
            //만약 5번 채널을 최근에 사용했다면, index가 11을 넘어가는 순간 1번부터 검사하게 됨
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;
            //이미 사용 중이라면 건너뛰기
            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            //최근 플레이한 채널을 갱신
            channelIndex = loopIndex;

            //비어있는 채널을 발견했다면 효과음을 재생
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
