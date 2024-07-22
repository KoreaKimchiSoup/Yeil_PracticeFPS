using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// EnemyFOV ��� ��ũ��Ʈ�� ���� Ŀ���� �����Ͷ�� �����
// �׷��� ����Ƽ���� �ν���
[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        // EnemyFOV Ŭ������ ����
        EnemyFOV fov = (EnemyFOV)target;

        // �� ������ �������� ��ǥ�� ���(��ޱ� 120���� 1/2)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        // ������ ȭ��Ʈ�� ����
        Handles.color = Color.white;

        // ������ �̷���� ���� �׸�
        Handles.DrawWireDisc(fov.transform.position, //���� ��ǥ
                                          Vector3.up,                   // �븻 ����
                                          fov.viewAngle);             // ���� ������

        // ��������� ������ 20��¥�� ������ ����
        Handles.color = new Color(1, 1, 1, 0.2f);

        Handles.DrawSolidArc(fov.transform.position,  // ���� ��ǥ
                                         Vector3.up,                    // �븻 ����
                                         fromAnglePos,                // ��ä�� ���� ��ġ
                                         fov.viewAngle,               // ��ä�� ����
                                         fov.viewRange);             // ��ä�� ����
        // �ؽ�Ʈ ���
        Handles.Label(fov.transform.position +
                             (fov.transform.forward * 2f),
                              fov.viewAngle.ToString("������"));
    }
}
