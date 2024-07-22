using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 구조체는 메모리의 스택 영역에 할당됨
// 성능이 클래스보다 좋음
// 빠른 계산 및 활용이 필요할 경우 쓰면 좋다
[Serializable]
public struct PlayerSfx // 구조체
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireController : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN
    }
    // 현재 사용중인 무기 확인용 변수
    public WeaponType currentWeapon = WeaponType.RIFLE;

    public GameObject bulletPrefab;
    public Transform firePos; // FirePos의 위치값
    public ParticleSystem cartridge; // 탄피 파티클시스템용 변수
    private ParticleSystem muzzleFlash;

    public PlayerSfx playerSfx;
    private AudioSource _audio;

    // 카메라를 흔드는 스크립트를 가져오기
    Shake shake;

    public Image magazineImg;
    public Text magazineText;

    public int maxBullet = 10;
    public int reamainingBullet = 10;
    public float reloadTime = 2f;
    bool isReloading = false;

    public Sprite[] weaponIcons;
    public Image weaponImage;

    [Header("자동공격 관련")]
    bool isFire = false;
    float nextFire;
    public float fireRate = 0.1f;

    int enemyLayer;
    int obstacleLayer;
    int layerMask;

    // 레이어값을 추출해서 저장
    // 현재 ENEMY 레이어는 12번에 있으므로 2 ^ 12 = 4096의 값을 갖는다
    // 해당 값을 알고 있으면 enemyLayer = 4096 으로 바로 대입해도 되긴함

    private void Start()
    {
        // firePos의 자식오브젝트 중에서 ParticleSystem 컴포넌트 획득
        // 유니티의 모든 오브젝트는 상대성을 지니므로
        // 따라서 스크립트의 위치나 오브젝트의 위치가 매우 중요하다
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        // enemyLayer = 4906; // 레이어 12번째이므로 동일하다
        // NameToLayer 함수는 레이어의 index를 리턴한다
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        // 두 레이어를 결합함
        // 레이어를 2개 이상 병합할 때는 | (or 비트 연산자)를 이용한다
        layerMask = 1 << enemyLayer | 1 << obstacleLayer;
        // layerMask의 비트
        // 10 0000 0000
        // 01 0000 0000
        // --------------| (or) 연산자
        // 11 0000 0000 (8,192 + 4,096 = 12,288)

    }

    private void Update()
    {
        // 레이캐스트 디버그용
        Debug.DrawRay(firePos.position, firePos.forward * 20f, Color.red);

        // UI 등을 클릭 또는 터치 하게되면 True 아니면 False
        // 즉, 이벤트 시스템을 이용하는 버튼 같은 것들을
        // 클릭하게되면 Update 구문은 항상 return이 된다
        // 그리고 현재 BloodScreen 화면 전체를 덮고 있기 때문에
        // 밑의 if문 조건은 항상 true로 조건을 충족한다
        // 해결방법은 BloodScreen의 Image 컴포넌트인
        // Raycast Target 변수를 체크 해제한다
        // Raycast Target 이란 겹쳐져 있는 Canvas에
        // 상호작용을 할건지 말건지에 대한 bool 변수이다
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        RaycastHit hit;

        // Raycast는 충돌 유무만 판단하고 실제 충돌 객체 정보는
        // RaycastHit에 전달된다
        // 이 때 out으로 출력되는 값을 전달받기 위한 변수를 미리 선언한다
        // 레이 발사 위치, 레이 발사 방향, 충돌한 객체 정보를 반환받을 변수, 레이 사거리, 검출 레이어
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))  
        {
            // enemyLayer에 의해서 검출이 되면
            isFire = hit.collider.CompareTag("ENEMY");
        }
        else
        {
            isFire = false;
        }

        if (!isReloading && isFire)
        {
            if (Time.time > nextFire)
            {
                reamainingBullet--;
                Fire();

                if (reamainingBullet == 0)
                {
                    StartCoroutine(Reloading());
                }

                nextFire = Time.time + fireRate;
            }
        }


        // GetMouseButton은 마우스 누르고 있는 동안 지속 발생
        // GetMouseButtonDown은 누르는 순간 1번만 발생
        // GetMouseButtonUp은 떼는 순간 1번만 발생
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            // 현재 보유하고 있는 총알을 하나씩 뺀다
            reamainingBullet--;
            // 공격 메소드 호출
            Fire();

            if (reamainingBullet == 0)
            {
                // 재장전 코루틴 호출
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());
        // 총알 프리팹을 총구의 위치와 회전값을 가지고 동적 생성함
        //Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        // 위의 동적생성을 사용하지 않고 오브젝트풀을 사용한다

        // 싱글톤 기법을 활용하여 오브젝트풀의 놀고 있는 총알을 가져온다
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            // firePos의 포지션과
            _bullet.transform.position = firePos.position;
            // firePos의 회전값을 참조한다
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }


        // 총알이 발사되는 시점에 탄피가 떨어지는 파티클 재생
        cartridge.Play();
        muzzleFlash.Play();
        FireSfx();

        magazineImg.fillAmount = (float)reamainingBullet / (float)maxBullet;
        // 남은 총알 수 텍스트 갱신용 함수 호출
        UpdateBulletText();
    }

    void FireSfx()
    {
        // 현재 들고 있는 무기의 enum 값을 int 로 변환해서
        // 재생하고자 하는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currentWeapon];
        // 지정된 음원을 1(100%) 볼륨으로 재생
        _audio.PlayOneShot(_sfx, 1f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currentWeapon], 1f);

        // 재장전 음원의 길이 + 0.3초 만큼 대기함
        yield return new WaitForSeconds(playerSfx.reload[(int)currentWeapon].length + 0.3f);

        isReloading = false;
        magazineImg.fillAmount = 1f;
        reamainingBullet = maxBullet;
        // 남은 총알 수 텍스트 갱신용 함수 호출
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        string str = $"<color=#ff0000>{reamainingBullet}</color>/{maxBullet}";
        magazineText.text = str;
    }

    public void OnChangeWeapon()
    {
        currentWeapon++;
        currentWeapon = (WeaponType)((int)currentWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currentWeapon];
    }
}
