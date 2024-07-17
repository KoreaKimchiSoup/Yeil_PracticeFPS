using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("리스폰 관련 내용")]
    public Transform[] points;
    public GameObject enemy;
    public float createTime = 2f; // enemy 생성 주기
    public int maxEnemy = 10; // 최대 생성 수

    public bool isGameOver = false;

    [Header("오브젝트풀 관련 내용")]
    public GameObject bulletPrefab;
    // 오브젝트 풀의 최대 사이즈
    public int maxPool = 1;
    // 오브젝트 풀 용 리스트
    public List<GameObject> bulletPool = new List<GameObject>();

    // 싱글톤 패턴을 활용하여 해당 스크립트(GameManager)에 접근하기위한 변수
    public static GameManager instance = null;

    public CanvasGroup inventoryCanvasGroup;

    [HideInInspector]
    public int killCount;
    public Text killCountTxt;
    
    void LoadGameData()
    {
        // 지정된 값으로 저장소에 저장된 값을 가져온다
        // 이때 정수나 실수는 초기값을 지정해줘도 됨
        // GetInt("키 값", 초기값)
        killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountTxt.text = "KILL " + killCount.ToString("0000"); // 킬 카운트는 항상 4자리를 갖는다 ex) 0001, 0002
    }

    public void IncresementKillCount()
    {
        killCount++;
        killCountTxt.text = "KILL " + killCount.ToString("0000");
        // 저장소에 KILL_COUNT 라는 키 값으로 killCoumt 값을 저장함
        PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }

    private void Awake()
    {
        if (instance == null) // instance가 null 이라면
        {
            instance = this; // instance에 자기 자신을 셀프로 할당한다
        }
        else if (instance != this) // null은 아니지만 instance에 다른 놈이 있을 경우
        {
            Destroy(this.gameObject); // 재 할당을 하기 위해 GameManager를 파괴한다
        }

        // 씬 변경이 발생하여도 해당 게임 오브젝트를 파괴하지 않음
        // 게임이 종료될 때까지 절대 파괴되지 않음(어떠한 로직이 있지 않는 이상)
        DontDestroyOnLoad(this.gameObject);

        // 저장된 게임 데이터 불러오기
        LoadGameData();

        // 오브젝트 풀 생성 메서드 호출
        CreateObjectPool();
    }

    void Start()
    {
        OnInventoryCanvasOpen(false);

        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        if (points.Length > 0) // 방어 코드
        {
            // 생성 코루틴 함수 호출
            StartCoroutine(CreateEnemy());
        }
    }

    IEnumerator CreateEnemy()
    {
        while(!isGameOver)
        {
            // ENEMY 태그를 지닌 오브젝트를 검색하여 길이(크기) 값을 저장한다
            int enemyCount = GameObject.FindGameObjectsWithTag("ENEMY").Length;
            if (enemyCount < maxEnemy) // 9 < 10 까지
            {
                // 생성주기만큼 대기한다
                yield return new WaitForSeconds(createTime);

                // 저장된 리스폰 포인트의 길이 만큼 랜덤한 숫자를 추출한다
                // 해당 인덱스를 통하여 생성 위치를 랜덤하게 설정함
                int index = Random.Range(0, points.Length);

                Instantiate(enemy, points[index].position, points[index].rotation);
            }
            else // 리스폰된 Enemy의 수가 Max에 도달 했을 때
            {
                yield return null;
            }
        }
    }

    // 오브젝트풀 관련 메서드
    public void CreateObjectPool()
    {
        // ObjectPools 라는 이름의 빈 오브젝트를 생성한다
        GameObject objectPools = new GameObject("ObjectPools");

        // 풀링 갯수만큼 총알을 생성하기 위한 반복문
        for (int i = 0; i < maxPool; i++)
        {
            // 총알 프리펩을 동적 생성 하면서
            // 위에서 생성한 ObjectPools의 자식으로 이식한다
            /*GameObject*/ GameObject bullet = Instantiate(bulletPrefab, objectPools.transform);
            bullet.name = "Bullet_" + i.ToString("00");
            // 총알 비활성화
            bullet.SetActive(false);
            // 오브젝트풀(리스트)에 생성된 총알을 추가함
            bulletPool.Add(bullet);
        }
    }

    // 오브젝트 풀에서 놀고 있는 총알을 골라서 반환하는 메서드
    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            // 풀에서 선택한 총알의 상태가 Active False일 경우 (비활성화 상태일 때)
            if (!bulletPool[i].activeSelf)
            {
                return bulletPool[i];
            }
        }

        return null;
    }

    bool isPaused;
    public void OnPauseClick()
    {
        isPaused = !isPaused;

        // bool 변수가 true면 0, 아니면 1
        // timeScale은 1을 기준으로 작아지면 느려지고 커지면 빨라짐
        // 0이되면 일시정지이니 최대 4 이상 높이지 않는것이 좋음
        // 모바일 기준 발열 및 기타등등의 문제가 있기 때문임
        Time.timeScale = (isPaused ? 0.0f : 1.0f);

        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        // 플레이어에 추가된 스크립트 모두를 가져옴
        // MonoBehaviour를 가진 스크립트를 전부다 가져옴
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        // 일시 정지 일 때 모든 스크립트를 중지함
        // 일시 정지 해제되면 다시 동작함
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }

        // 무기 교체 UI의 CancasGroup을 제어하기 위한 코드 추가
        var canvasGroup = GameObject.Find("Panel_Weapon").GetComponent<CanvasGroup>();
        // BlocksRaycasts는 RayCast Target 보다 우선순위가 높다
        // blocksRaycasts가 꺼지면 RayCast Target
        canvasGroup.blocksRaycasts = !isPaused;
    }

    public void OnInventoryCanvasOpen(bool isOpened)
    {
        inventoryCanvasGroup.alpha = (isOpened) ? 1f : 0f;

        // 투명도가 0이 되어서 UI가 보이지 않더라도
        // 레이캐스트에 의한 터치 이벤트는 발생하기 때문에
        // 아래 코드를 통해서 터치 이벤트를 무시하도록한다
        inventoryCanvasGroup.interactable = isOpened;
        inventoryCanvasGroup.blocksRaycasts = isOpened;
    }
}