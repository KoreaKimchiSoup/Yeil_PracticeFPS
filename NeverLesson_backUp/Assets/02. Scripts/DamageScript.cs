using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    const string BULLETTAG = "BULLET";
    const string ENEMYTAG = "ENEMY";
    float initHp = 100f;
    public float currHp;

    // ��������Ʈ �� �̺�Ʈ ����
    public delegate void PlayerDieHandler();
    // ��������Ʈ���� �Ļ��� �̺�Ʈ
    public static event PlayerDieHandler PlayerDieEvent; // �÷��̾ �׾��� �� ����� �̺�Ʈ

    public Image bloodScreen; // bloodScreen�� ��Ʈ�� �� ����

    public Image hpBar;
    readonly Color initColor = new Vector4(0f, 1f, 0f, 1f);
    Color currColor;

    void Start()
    {
        // ������ �����ϸ� ���� hp�� �ʱ�ȭ�Ѵ�
        currHp = initHp;

        hpBar.color = initColor;
        currColor = initColor;
    }

    private void OnTriggerEnter(Collider other) // �÷��̾�� ��� �Ѿ��� ��Ʈ�� �ϴ� OnTriggerEnter
    {
        if (other.CompareTag(BULLETTAG)) // �÷��̾�� ��� ������Ʈ�� �±װ� BULLETTAG �� ��
        {
            Destroy(other.gameObject); // �ش� ������Ʈ�� �ı��Ѵ�

            // ����ȭ�� ȿ�� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(ShowBloodScreen());

            currHp -= 5f; // DamageScript ��ũ���� ���� ������Ʈ�� Hp�� 5 ��´�
            DisplayHpBar();

            // print(currentHp); // ���� ����  Hp�� ���

            if (currHp <= 0f) // Hp�� 0���� �۰ų� ���ٸ�
            {
                PlayerDie(); // �÷��̾� ���� �޼��� ȣ��
            }
        }
    }

    IEnumerator ShowBloodScreen()
    {
        // �������� ������ 2 ~ 30�� ������ �����ϰ� �����Ѵ� (1�� �ǹ̴� 100%�� �ǹ���)
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        // Blood Screen�� ����� �� �� 0.1�� ���� ������
        yield return new WaitForSeconds(0.1f);
        // �ٽ� ���� 0, ���� 0
        bloodScreen.color = Color.clear;
    }

    void PlayerDie()
    {
        //print("�÷��̾� ���");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(ENEMYTAG);
        //foreach(GameObject enemy in enemies)
        //{
        //    // SendMessage("ȣ���� �޼��� �̸�", ���� ���);
        //    // SendMessage �޼���� Ư�� ��ũ��Ʈ�� �����ϴ� ���� �ƴ϶�
        //    // ������Ʈ�� ���Ե� ��� ��ũ��Ʈ�� �ϳ��� �˻��Ͽ�
        //    // �ش� �ϴ� �̸��� �����ϸ� �޼��带 ȣ�� �ϴ� ����̴�
        //    enemy.SendMessage("E_PlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        // �̺�Ʈ ȣ��
        PlayerDieEvent();
        // �̱��� ������ �̿��Ͽ� �ս��� �����Ѵ�.
        GameManager.instance.isGameOver = true;
    }

    void DisplayHpBar()
    {
        // ���� ü���� ��ü ü���߿� �� �ۼ�Ʈ����
        float currHpRate = currHp / initHp;

        if (currHpRate > 0.5f) // ü���� 50���� ���� ũ�ٸ�
        {
            // �������� ���� = ��� + ���� = ���
            // ��� -> ���
            currColor.r = (1 - currHpRate) * 2f;
        }
        else // ü���� 50���� ������ ��
        {
            // ����� ���ҽ�Ŵ
            // ��� -> ����
            currColor.g = currHpRate * 2f;
        }

        hpBar.color = currColor; // ü�¹� ���� ����
        hpBar.fillAmount = currHpRate;
    }
}
