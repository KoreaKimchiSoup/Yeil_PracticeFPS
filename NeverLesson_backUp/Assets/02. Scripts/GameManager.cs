using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("������ ���� ����")]
    public Transform[] points;
    public GameObject enemy;
    public float createTime = 2f; // enemy ���� �ֱ�
    public int maxEnemy = 10; // �ִ� ���� ��

    public bool isGameOver = false;

    [Header("������ƮǮ ���� ����")]
    public GameObject bulletPrefab;
    // ������Ʈ Ǯ�� �ִ� ������
    public int maxPool = 1;
    // ������Ʈ Ǯ �� ����Ʈ
    public List<GameObject> bulletPool = new List<GameObject>();

    // �̱��� ������ Ȱ���Ͽ� �ش� ��ũ��Ʈ(GameManager)�� �����ϱ����� ����
    public static GameManager instance = null;

    public CanvasGroup inventoryCanvasGroup;

    [HideInInspector]
    public int killCount;
    public Text killCountTxt;
    
    void LoadGameData()
    {
        // ������ ������ ����ҿ� ����� ���� �����´�
        // �̶� ������ �Ǽ��� �ʱⰪ�� �������൵ ��
        // GetInt("Ű ��", �ʱⰪ)
        killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountTxt.text = "KILL " + killCount.ToString("0000"); // ų ī��Ʈ�� �׻� 4�ڸ��� ���´� ex) 0001, 0002
    }

    public void IncresementKillCount()
    {
        killCount++;
        killCountTxt.text = "KILL " + killCount.ToString("0000");
        // ����ҿ� KILL_COUNT ��� Ű ������ killCoumt ���� ������
        PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }

    private void Awake()
    {
        if (instance == null) // instance�� null �̶��
        {
            instance = this; // instance�� �ڱ� �ڽ��� ������ �Ҵ��Ѵ�
        }
        else if (instance != this) // null�� �ƴ����� instance�� �ٸ� ���� ���� ���
        {
            Destroy(this.gameObject); // �� �Ҵ��� �ϱ� ���� GameManager�� �ı��Ѵ�
        }

        // �� ������ �߻��Ͽ��� �ش� ���� ������Ʈ�� �ı����� ����
        // ������ ����� ������ ���� �ı����� ����(��� ������ ���� �ʴ� �̻�)
        DontDestroyOnLoad(this.gameObject);

        // ����� ���� ������ �ҷ�����
        LoadGameData();

        // ������Ʈ Ǯ ���� �޼��� ȣ��
        CreateObjectPool();
    }

    void Start()
    {
        OnInventoryCanvasOpen(false);

        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        if (points.Length > 0) // ��� �ڵ�
        {
            // ���� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(CreateEnemy());
        }
    }

    IEnumerator CreateEnemy()
    {
        while(!isGameOver)
        {
            // ENEMY �±׸� ���� ������Ʈ�� �˻��Ͽ� ����(ũ��) ���� �����Ѵ�
            int enemyCount = GameObject.FindGameObjectsWithTag("ENEMY").Length;
            if (enemyCount < maxEnemy) // 9 < 10 ����
            {
                // �����ֱ⸸ŭ ����Ѵ�
                yield return new WaitForSeconds(createTime);

                // ����� ������ ����Ʈ�� ���� ��ŭ ������ ���ڸ� �����Ѵ�
                // �ش� �ε����� ���Ͽ� ���� ��ġ�� �����ϰ� ������
                int index = Random.Range(0, points.Length);

                Instantiate(enemy, points[index].position, points[index].rotation);
            }
            else // �������� Enemy�� ���� Max�� ���� ���� ��
            {
                yield return null;
            }
        }
    }

    // ������ƮǮ ���� �޼���
    public void CreateObjectPool()
    {
        // ObjectPools ��� �̸��� �� ������Ʈ�� �����Ѵ�
        GameObject objectPools = new GameObject("ObjectPools");

        // Ǯ�� ������ŭ �Ѿ��� �����ϱ� ���� �ݺ���
        for (int i = 0; i < maxPool; i++)
        {
            // �Ѿ� �������� ���� ���� �ϸ鼭
            // ������ ������ ObjectPools�� �ڽ����� �̽��Ѵ�
            /*GameObject*/ GameObject bullet = Instantiate(bulletPrefab, objectPools.transform);
            bullet.name = "Bullet_" + i.ToString("00");
            // �Ѿ� ��Ȱ��ȭ
            bullet.SetActive(false);
            // ������ƮǮ(����Ʈ)�� ������ �Ѿ��� �߰���
            bulletPool.Add(bullet);
        }
    }

    // ������Ʈ Ǯ���� ��� �ִ� �Ѿ��� ��� ��ȯ�ϴ� �޼���
    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            // Ǯ���� ������ �Ѿ��� ���°� Active False�� ��� (��Ȱ��ȭ ������ ��)
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

        // bool ������ true�� 0, �ƴϸ� 1
        // timeScale�� 1�� �������� �۾����� �������� Ŀ���� ������
        // 0�̵Ǹ� �Ͻ������̴� �ִ� 4 �̻� ������ �ʴ°��� ����
        // ����� ���� �߿� �� ��Ÿ����� ������ �ֱ� ������
        Time.timeScale = (isPaused ? 0.0f : 1.0f);

        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        // �÷��̾ �߰��� ��ũ��Ʈ ��θ� ������
        // MonoBehaviour�� ���� ��ũ��Ʈ�� ���δ� ������
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        // �Ͻ� ���� �� �� ��� ��ũ��Ʈ�� ������
        // �Ͻ� ���� �����Ǹ� �ٽ� ������
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }

        // ���� ��ü UI�� CancasGroup�� �����ϱ� ���� �ڵ� �߰�
        var canvasGroup = GameObject.Find("Panel_Weapon").GetComponent<CanvasGroup>();
        // BlocksRaycasts�� RayCast Target ���� �켱������ ����
        // blocksRaycasts�� ������ RayCast Target
        canvasGroup.blocksRaycasts = !isPaused;
    }

    public void OnInventoryCanvasOpen(bool isOpened)
    {
        inventoryCanvasGroup.alpha = (isOpened) ? 1f : 0f;

        // ������ 0�� �Ǿ UI�� ������ �ʴ���
        // ����ĳ��Ʈ�� ���� ��ġ �̺�Ʈ�� �߻��ϱ� ������
        // �Ʒ� �ڵ带 ���ؼ� ��ġ �̺�Ʈ�� �����ϵ����Ѵ�
        inventoryCanvasGroup.interactable = isOpened;
        inventoryCanvasGroup.blocksRaycasts = isOpened;
    }
}