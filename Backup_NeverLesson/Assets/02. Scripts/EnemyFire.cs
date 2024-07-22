using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    AudioSource audio; // 오디오 소스를 참조하는 변수
    Animator animator; // 애니메이터를 컨트롤하기 위한 변수
    Transform playerTr; // Player의 위치값을 컨트롤 또는 참조하기 위한 변수
    Transform enemyTr; // Enemy의 위치값을 컨트롤 또는 참조하기 위한 변수

    // hashFire는 Animator에 있는 파라미터 값을 설정하는 용도이며 이름이 'Fire' 인 파라미터 값을 hashFire에 저장한다
    readonly int hashFire = Animator.StringToHash("Fire");
    // hashReload는 Animator에 있는 파라미터 값을 설정하는 용도이며 이름이 'Reload' 인 파라미터 값을 hashReload에 저장한다
    readonly int hashReload = Animator.StringToHash("Reload"); // 애니메이터에서 가져온 trigger인 Reload

    // 자동 공격 관련 변수
    [Header("자동공격")]
    public float nextFire = 0f;
    readonly float fireRate = 0.1f; // 발사 간격 설정을 위한 변수
    readonly float damping = 10f;  // 

    [Header("재장전")]
    int currentBullet = 10; // 게임이 시작되면 Enemy의 현재 총알을 초기화함 
    readonly float reloadTime = 2f; // 재장전에 걸리는 시간 2초
    readonly int maxBullet = 10; // Enemy가 가지고 있는 총의 장탄 수
    bool isReload = false;            // Reload를 하거나 하지 않기 위한 bool 변수
    WaitForSeconds wsReload;     // 코루틴의 대기시간을 컨트롤 하기 위한 변수

    public bool isFire = false; // 총을 발사하고 있는지 체크하는 bool 변수
    public AudioClip fireSfx;  // 총을 발사할 때의 사운드
    public AudioClip reloadSfx; // 재장전 사운드

    // 총알 발사 관련 변수
    public GameObject bullet; // Bullet 프리팹을 컨트롤하기 위한 변수
    public Transform firePos;    // 총알이 발사되는 지점을 참조하기 위한 변수

    // muzzleFlash의 이미지를 총알이 발사될 때만 켜기 위한 MeshRenderer 참조 변수
    // 당연하게도 Enemy가 Player를 처음부터 본게 아니라면 꺼둬야 함
    // 이 후, Player를 향해 총을 발사할 때만 킴
    public MeshRenderer muzzleFlash;


    void Start()
    {
        //태그가 "PLAYER"인 게임오브젝트를 찾고 해당 오브젝트의 Transform(위치값, 회전값, 스케일)을 참조하기 위한 변수
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();

        // enemyTr(Transform)에 Transform 컴포넌트를 가져옴
        enemyTr = GetComponent<Transform>();

        // animator(Animator)에 Animator 컴포넌트를 가져옴
        animator = GetComponent<Animator>();

        // audio(AudioSource)에 AudioSource 컴포넌트를 가져옴
        audio = GetComponent<AudioSource>();

        wsReload = new WaitForSeconds(reloadTime); // 코루틴이 제어권을 반납하기까지 기다리는 시간

        muzzleFlash.enabled = false; // 게임이 시작하면 발사되면 안되기 때문에 false
    }

    void Update()
    {
        // isFire의 빈도수가 더 높기 때문에 상대적으로 적은 isReload가 true일 땐 굳이 isFire를 체크하지 않기 위함 (소량의 성능 향상)
        if(!isReload && isFire) // 재장전이 아닐 때, 공격중 일 때
        {
            // Time.time(시간)이 nextFire보다 클 때
            if(Time.time > nextFire)
            {
                // 총알 발사 함수 호출
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0f, 0.3f);
            }
            // A - B 는 B가 A를 바라본다
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        // 총구화염 코루틴 함수 호출
        StartCoroutine(ShowMuzzleFlash());

        animator.SetTrigger(hashFire); // 트리거는 true, false값이 없고 바로 사용하면됨
        audio.PlayOneShot(fireSfx, 1f); // fireSfx의 볼륨을 1로 설정 (1이 최대값)

        GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3f); // 3초후 총알 삭제

        // 총알을 발사할 때 마다 총알을 하나씩 감소시킴
        currentBullet--;

        // 현재 총알 갯수를 비교하여 재장전의 유무를 판단함
        // = 우측의 조건을 비교하여 참일경우 true 또는 false;
        // 총알을 발사할 때 비교할 실제 조건은 아래와 같음
        // 9/10, 8/10, ~ ,1/10, 0/10 ( 10, 9, ~ ,2, 1 )
        // 0/10이 될 때, isReload는 0이 되며 bool 값은 
        isReload = (currentBullet % maxBullet == 0);

        // 즉, currentBullet % maxBullet 이 값이 0이 아닐 경우엔 전부 false
        //if (currentBullet % maxBullet == 0) // 해당 코드는 위 isReload 조건식과 같은 코드임
        //{
        //    isReload = false;
        //}
        //else
        //{
        //    isReload = true;
        //}


        if (isReload)
        {
            // 재장전 코루틴 함수 호출함
            StartCoroutine(Reload());
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        // 발사할 때 보이는 총구화염 이펙트가 너무 빨리 사라지므로 잠깐이나마 시간을 주기 위하며 굳이 코루틴을 사용함
        muzzleFlash.enabled = true;

        // 회전값을 (0,0,1) * (0 ~ 360)로 설정하고
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        // 설정된 값을 muzzleFlash의 localRotation(지역회전값)에 전달함
        muzzleFlash.transform.localRotation = rot;

        // muzzleFlash의 Scale 값을 xyz 모두 동일하게 1 ~ 2배 만큼 랜덤하게 조정
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);

        // 텍스처 오프셋 조정하기
        // Random Range에 의해서 0 또는 1값이 나오는데
        // 여기에 5를 곱해서 0 또는 0.5 값이 나오도록 계산
        // 참고로 Random.Range(x, y) 는 x포함 y미포함이다
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;

        // muzzleFlash에 존재하는 Material의 offset 값을 전달하는데,
        // 정확히는 Material에 존재하는 Shader가 값을 변경하므로  Shader에 값을 전달함
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        // 대기시간을 0.05 ~ 0.2초 사이 랜덤하게 설정함
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        // 모든 로직이 끝난 후 muzzleFlash를 끔 (muzzleFlash의 Mesh Renderer를 끈다는 의미)
        muzzleFlash.enabled = false;

    }

    IEnumerator Reload()
    {
        // 이미 hashReload = Animator.StringToHash("Reload"); 가 전역변수로 설정되오 있으므로,
        // Animator의 파라미터 값인 Reload를 찾아서 트리거를 true 시켜줌
        animator.SetTrigger(hashReload);
        // 재장전을 할 때 마다 소리를 한 번씩 재생함
        audio.PlayOneShot(reloadSfx, 1f);

        // 재장전 애니메이션을 기다려주기 위함
        // 밑의 코드가 없으면 재장전이 진행도는 와중에 총알 수가 장전됨
        yield return wsReload;

        // 재장전이 끝났다면 현재 총알에 maxBullet을 대입하여 총알을 보충함
        currentBullet = maxBullet;

        // 재장전이 끝났다면 isReload가 false로 바뀜
        // 그러면 총알이 발사될 때 마다 Fire 함수가 실행되는데
        // Fire 함수엔 Reload 코루틴을 실행시키는 로직이 있어
        // 한 발 발사할 때마다 재장전이 되지 않기 위함
        // 그리고 Reload 코루틴은 현재 로직에선 총알을 다 써야 실행되는임
        isReload = false;
    }
}
