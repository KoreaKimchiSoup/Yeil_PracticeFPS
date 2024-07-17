using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    Camera uiCamera; // ĵ������ ��� �ִ� ī�޶�
    Canvas canvas;

    RectTransform rectParent;
    RectTransform rectHp;

    [HideInInspector]
    public Vector3 offset = Vector3.zero; // HpBar �̹����� ��ġ ������ ������
    [HideInInspector]
    public Transform targetTr; // ���� ��� Transform ������Ʈ

    void Start()
    {
        // �������� �� �� �θ��� ĵ������ �������� ���ؼ�
        // InParent�� ����Ѵ�
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        // ���� ��ǥ -> ��ũ�� ��ǥ�� ��ȯ (Camera.main�� Main Camera ��� �±׸� ���� ī�޶� �ǹ���) Main Camera �±װ� ������ �ζ�
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);

        // ī�޶��� �� ������ �� �� ��ǥ���� �����Ѵ�
        // �������� �� ���� hpBar�� �ڿ��� ���� ���� hpBar�� �����Ǿ� ������ �׷�
        if (screenPos.z < 0f)
        {
            screenPos *= -1;
        }

        var localPos = Vector2.zero;
        // ��ũ�� ��ǥ -> ��ƮƮ������ ��ǥ�� ��ȯ��
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        // ���������� ��ȯ�� RectTransform ��ǥ�� rectHp �� �����Ѵ�
        rectHp.localPosition = localPos;
    }
}
