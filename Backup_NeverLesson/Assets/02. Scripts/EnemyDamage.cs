using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    const string bulletTag = "BULLET"; // �Ѿ��� �±�

    float initHp = 100f; // �ʱ� ����
    float hp = 100f;
    GameObject bloodEffect;

    // ���� ������ �������� ������ ����
    public GameObject hpBarPrefab;
    // ���� �������� ��ġ�� ������ ������
    public Vector3 hpBarOffset = new Vector3(0f, 2.2f, 0f);
    // �θ� �� Canvas ��ü
    Canvas uiCanvas;
    // ���� ��ġ�� ���� fillAmount �Ӽ��� ������ Image
    Image hpBarImage;

    void Start()
    {

        // Resources.Load<GameObject>("���ϰ��");
        // "Resources" �������� �����Ͽ� ���׸��� ���� Load ��
        bloodEffect = Resources.Load<GameObject>("Blood");
        SetHpBar();
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>();

        // hpBar�� ���������ϸ鼭 ĵ������ �ڽ����� �־��ش�
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);

        // hpBarImage = ���� hpBar
        // �θ� ���� Image�� ������ �������� �ε����� �̷���� 
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        // ü�¹ٰ� ���󰡾��� ���� ������ ����
        var enemyHpBar = hpBar.GetComponent<EnemyHpBar>();
        enemyHpBar.targetTr = this.gameObject.transform;
        enemyHpBar.offset = hpBarOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(bulletTag))
        {
            // ����ȿ�� ���� �Լ� ȣ��
            ShowBloodEffect(collision);

            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);

            // BulletController�� �ۼ��� damage ������ ���� �����ͼ�
            // ü���� ����
            hp -= collision.gameObject.GetComponent<BulletController>().damage;
            // ü�¹��� ������ ������ ���̱�
            hpBarImage.fillAmount = hp / initHp;

            // hp�� 0���� �۰ų� ������
            if (hp <= 0f)
            {// EnemyAI�� �����ϴ� State(����)�� DIE�� ������
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                // ���� �Ŵ����� ų ī��Ʈ ���� �Լ� ȣ��
                GameManager.instance.IncresementKillCount();
                // ��� �� �ݶ��̴� ��Ȱ��ȭ
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }

        void ShowBloodEffect(Collision collision)
        {
            // �浹������ ��ġ ����Ʈ�� �ϳ� ������
            Vector3 pos = collision.contacts[0].point;

            // �浹 ������ �������� ���ϱ�
            // �浹�� ù ��° ���� ������ �μ��մϴ�.
            //Debug.Log("ù ��° ���� ����: " + collision.contacts[0].normal);
            Vector3 _nomal = collision.contacts[0].normal;

            // �Ѿ��� ź�ο� ���ֺ��� ���Ⱚ ���ϱ�
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _nomal);

            // ���� ȿ���� 
            GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
            Destroy(blood, 1f);
        }
    }
}
