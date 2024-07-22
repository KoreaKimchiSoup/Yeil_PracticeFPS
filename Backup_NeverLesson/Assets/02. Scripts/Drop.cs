using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    // �÷��̾�� ����� ������ ����
    PlayerController playerController;

    // �÷��̾� ü�� ����
    DamageScript playerHp;

    // �Ʒ��� ��ũ��Ʈ�� �پ��� ���ӿ�����Ʈ�� ã�� ���� ����
    // ItemList�� �ڽ����� �����ϱ� ����
    // ItemList ������Ʈ�� FindItemList ��ũ��Ʈ�� �߰��Ͽ���
    WaitForSeconds waitForSeconds = new WaitForSeconds(3f);

    // �÷��̾� ������ �ӵ��� ������ ����
    float playerMoveSpeed;
    // Update �޼��忡�� �ڷ�ƾ�� ��� ����Ǵ°��� �����ϱ� ���� ����
    bool isCoroutineActive;
    // Hp ���� �������� ȸ�� ��ġ
    float increaseHp = 20f;

    private void Awake()
    {
        // �� ����� Ư�� Ÿ���� ������Ʈ�� ������ 'ù ��°' GameObject�� ã���ϴ�.
        // ���� ���, ���ӿ��� PlayerController ������Ʈ�� ���� ������Ʈ�� ã������
        // ������ ���� ����� �� �ֽ��ϴ�:
        playerController = FindObjectOfType<PlayerController>();
        playerHp = FindObjectOfType<DamageScript>();
        isCoroutineActive = true;
    }

    private void Start()
    {
        // �÷��̾ �����ϴ� ������ �÷��̾� �ӵ��� �����Ͽ�
        // playerMoveSpeed ������ �����Ѵ�
        // �÷��̾� �ӵ��� ���������Ͱ� �ٲ�� �Ʒ��� �������� ���� �ٲ��
        playerMoveSpeed = playerController.moveSpeed;
    }

    //class ABC
    //{
    //}

    void Update()
    {
        //List<ABC> abc = new List<ABC>();
        //for (int i = 0; i < abc.Count; i++)
        //{
        //    abc[3]
        //}

        /* 
        foreach ������ �۵� ���
        foreach ������ �÷��� ���� �� �׸� ���� �ڵ� ����� �� ���� �����մϴ�. 
        ���⼭ �÷����� transform�� �ڽĵ��̸�, �� child�� Transform Ÿ���� ��ü�Դϴ�.
        ������ �θ� ������Ʈ�� ù ��° �ڽĿ��� �����Ͽ�, 
        ������ �ڽı��� ���ʷ� ������ Transform ������Ʈ�� �����մϴ�.

        ��� ����
        �̷��� �ڵ�� ���� ������Ʈ�� ��� �ڽ��� �˻��ϰų� ������ �� �����մϴ�. 
        ���� ���, Ư�� �±׸� ���� �ڽ��� ã�ų�, ��� �ڽ��� ������ �����ϴ� ���� �۾��� ���� �� �ֽ��ϴ�.
        */
        foreach (Transform child in transform)
        {
            if (child.CompareTag("SPEEDUP") && isCoroutineActive)
            {
                StartCoroutine(PlayerSpeedUp());
            }
            else if (child.CompareTag("HPUP") && isCoroutineActive)
            {
                StartCoroutine(PlayerHpUp());
            }
            else if (child.CompareTag("GRANADE") && isCoroutineActive)
            {
                // ����ź �߻� �޼���
            }
            else if (child.CompareTag("SHORK") && isCoroutineActive)
            {
                // ��ũ? �߻� �޼���
            }
        }
    }

    IEnumerator PlayerSpeedUp()
    {
        //  Update �޼��忡�� ��� �������� ���ϰ� ���� ���� �� false�� ����
        isCoroutineActive = false;
        Debug.Log("���ǵ� �� �ڷ�ƾ ����");
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");

        // SpeedItem Ŭ�� �Ұ����ϰ�
        //itemCanvasGroup.interactable = false;
        //itemCanvasGroup.blocksRaycasts = false;

        // �÷��̾� �ӵ� 2�� ����
        Debug.Log("�ӵ� 2�� ����");
        playerController.moveSpeed = playerMoveSpeed * 2;

        // 3�� ���
        Debug.Log("3�� ���");
        yield return waitForSeconds;

        // �÷��̾� �ӵ��� �ٽ� ���󺹱��Ѵ�
        Debug.Log("�ӵ� ����");
        playerController.moveSpeed = playerMoveSpeed;

        // ��Ÿ���� ������ �ٽ� Ŭ���� ������
        //itemCanvasGroup.interactable = true;
        //itemCanvasGroup.blocksRaycasts = true;

        // �ٽ� �������� ����� �̿��ϱ� ���Ͽ� isCoroutineActive = true ���󺹱�
        isCoroutineActive = true;
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");
    }
    IEnumerator PlayerHpUp()
    {
        //  Update �޼��忡�� ��� �������� ���ϰ� ���� ���� �� false�� ����
        isCoroutineActive = false;
        Debug.Log("Hp ȸ�� �ڷ�ƾ ����");
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");

        // HpItem Ŭ�� �Ұ����ϰ�
        // itemCanvasGroup.interactable = false;
        // itemCanvasGroup.blocksRaycasts = false;

        // �÷��̾� +20 hp ����
        // �÷��̾� hp�� Damege ��ũ��Ʈ�� ����
        Debug.Log("hp 20����");
        playerHp.currHp += increaseHp;
        Debug.Log($"���� hp: {playerHp.currHp}");

        // �÷��̾� hpBar ü������ ����
        playerHp.DisplayHpBar();

        // 3�� ���
        Debug.Log("3�� ���");
        yield return waitForSeconds;

        // �ӵ����� �������� �ٽ� ItemList ������Ʈ�� �ڽ����� �����Ͽ�
        // �ڷ�ƾ�� ����� �Ǵ°��� �����ϱ� ���� �ڵ�

        // 1. ItemList ������Ʈ�� �����ϴ� ������ �����
        // 2. ItemSpeed ������Ʈ�� �θ� ������ ������ �����Ѵ�

        //transform.SetParent(findItemList.transform, false);
        // SetParent�޼����� bool ���ڴ�
        // �θ��� ��ġ, ȸ��,  �������� ��ӹ������� ���� �����Դϴ�

        // ��Ÿ���� ������ �ٽ� Ŭ���� ������
        // itemCanvasGroup.interactable = true;
        // itemCanvasGroup.blocksRaycasts = true;

        // �ٽ� �������� ����� �̿��ϱ� ���Ͽ� true ���󺹱�
        isCoroutineActive = true;
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Slot�� �ڽ��� ������ 0 �̶�� �ǹ̴�
        // ���� ������Ʈ�� ���� ��츦 ���Ѵ�
        if (transform.childCount == 0)
        {
            // �巡�� ���� �������� Slot�� ����� ���
            // Drop ��ũ��Ʈ�� �ִ� ������Ʈ��
            // Slot�� �ڽ����� �����Ѵ�
            // ����, Drag Ŭ������ draggingItem�� static �̸�
            // ���̷�Ʈ�� ������ ������
            Drag.draggingItem.transform.SetParent(this.transform);
        }
    }
}
