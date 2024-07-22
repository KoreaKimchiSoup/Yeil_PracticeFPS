using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UseItem : MonoBehaviour
{
    // �÷��̾�� ����� ������ ����
    PlayerController playerController;

    DamageScript playerHp;

    // �θ��� ������ �������� ���� ����
    Transform parentTransform;

    // �Ʒ��� ��ũ��Ʈ�� �پ��� ���ӿ�����Ʈ�� ã�� ���� ����
    // ItemList�� �ڽ����� �����ϱ� ����
    // ItemList ������Ʈ�� FindItemList ��ũ��Ʈ�� �߰��Ͽ���
    FindItemList findItemList;

    WaitForSeconds waitForSeconds = new WaitForSeconds(3f);

    public CanvasGroup itemCanvasGroup;



    // �÷��̾� ������ �ӵ��� ������ ����
    float playerMoveSpeed;
    // Update �޼��忡�� �ڷ�ƾ�� ��� ����Ǵ°��� �����ϱ� ���� ����
    bool isCoroutineActive;


    private void Awake()
    {
        // �� ����� Ư�� Ÿ���� ������Ʈ�� ������ 'ù ��°' GameObject�� ã���ϴ�.
        // ���� ���, ���ӿ��� PlayerController ������Ʈ�� ���� ������Ʈ�� ã������
        // ������ ���� ����� �� �ֽ��ϴ�:
        playerController = FindObjectOfType<PlayerController>();
        findItemList = FindObjectOfType<FindItemList>();
        playerHp = FindObjectOfType<DamageScript>();
        isCoroutineActive = false;
    }

    private void Start()
    {
        // �÷��̾ �����ϴ� ������ �÷��̾� �ӵ��� �����Ͽ�
        // playerMoveSpeed ������ �����Ѵ�
        // �÷��̾� �ӵ��� ���������Ͱ� �ٲ�� �Ʒ��� �������� ���� �ٲ��
        playerMoveSpeed = playerController.moveSpeed;
    }

    private void Update()
    {
        // ������� parentTransform = transform.parent; �� �ڵ尡
        // ���ӽ��� �� �� Awake�� Start���� �ʱ�ȭ �߰�
        // ���� ����(������ �� �� ����) ItemSpeed �������� �ٸ� ������Ʈ�� �ڽ����� �����Ǿ��� ��
        // �ٽ� �θ��� �±׸� �˻��ؾ��� ��Ȳ���� �ʱ�ȭ�� ���� ���ϱ� ������
        // Update �޼��忡�� �׻� �θ��� �±׸� parentTransform�� �ֽ�ȭ �ϱ� ���� �ڵ��̴�
        parentTransform = transform.parent;

        // ItempSpeed�� �θ� ������Ʈ�� �ִ��� �˻��ϰ�
        // �ִٸ� �±װ� "SLOT"���� �˻��Ѵ�
        if (parentTransform != null && parentTransform.CompareTag("SLOT"))
        {
            if (!isCoroutineActive)
            {
                //if (false)
                //{
                //    StartCoroutine(PlayerSpeedUp());
                //}
                //else
                //{
                //    StartCoroutine(PlayerHpUp());
                //}
            }
        }
    }

    IEnumerator PlayerSpeedUp()
    {
        //  Update �޼��忡�� ��� �������� ���ϰ� ���� ���� �� false�� ����
        isCoroutineActive = true;
        Debug.Log("���ǵ� �� �ڷ�ƾ ����");

        // SpeedItem Ŭ�� �Ұ����ϰ�
        //itemCanvasGroup.interactable = false;
        itemCanvasGroup.blocksRaycasts = false;

        // �÷��̾� �ӵ� 2�� ����
        Debug.Log("�ӵ� 2�� ����");
        playerController.moveSpeed = playerMoveSpeed * 2;

        // 3�� ���
        Debug.Log("3�� ���");
        yield return waitForSeconds;

        // �ӵ����� �������� �ٽ� ItemList ������Ʈ�� �ڽ����� �����Ͽ�
        // �ڷ�ƾ�� ����� �Ǵ°��� �����ϱ� ���� �ڵ�

        // 1. ItemList ������Ʈ�� �����ϴ� ������ �����
        // 2. ItemSpeed ������Ʈ�� �θ� ������ ������ �����Ѵ�
        Debug.Log("�θ� ����");
        transform.SetParent(findItemList.transform);


        //transform.SetParent(findItemList.transform, false);
        // SetParent�޼����� bool ���ڴ�
        // �θ��� ��ġ, ȸ��,  �������� ��ӹ������� ���� �����Դϴ�

        // �÷��̾� �ӵ��� �ٽ� ���󺹱��Ѵ�
        Debug.Log("�ӵ� ����");
        playerController.moveSpeed = playerMoveSpeed;

        // ��Ÿ���� ������ �ٽ� Ŭ���� ������
        //itemCanvasGroup.interactable = true;
        itemCanvasGroup.blocksRaycasts = true;

        // �ٽ� �������� ����� �̿��ϱ� ���Ͽ� true ���󺹱�
        isCoroutineActive = false;
    }

    IEnumerator PlayerHpUp()
    {
        isCoroutineActive = true;
        //  Update �޼��忡�� ��� �������� ���ϰ� ���� ���� �� false�� ����
        Debug.Log("Hp ȸ�� �ڷ�ƾ ����");

        // SpeedItem Ŭ�� �Ұ����ϰ�
        //itemCanvasGroup.interactable = false;
        itemCanvasGroup.blocksRaycasts = false;

        // �÷��̾� +20 hp ����
        // �÷��̾� hp�� Damege ��ũ��Ʈ�� ����
        Debug.Log("hp 20����");
        playerHp.currHp += 20;
        Debug.Log(playerHp.currHp);

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
        isCoroutineActive = false;
    }
}
