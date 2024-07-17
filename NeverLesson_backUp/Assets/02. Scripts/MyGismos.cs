using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGismos : MonoBehaviour
{
    public enum Type
    {
        PATROLPOINT,
        SPAWNPOINT
    }
    const string spawnPointImg = "Enemy";
    public Type type = Type.PATROLPOINT; // �⺻ Ÿ�� ����

    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    private void OnDrawGizmos()
    {
        //���� Ÿ���� PATROLPOINT ���
        if (type == Type.PATROLPOINT)
        {
            //����� ���� 
            Gizmos.color = _color;
            //����� ��� (��ġ ,ũ��)
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else //������ ����Ʈ�̸�
        {
            Gizmos.color = _color;

            //DrawIcon(��ġ, �̹��� ���ϸ�, ������ ���� ����)
            //������ ���� ������ ���� ������ ī�޶� �� ��/�ƿ���
            //���� ������ ũ�Ⱑ Ŀ���� �۾����� ȿ��

            // DrawIcon(�������� ��ġ, ���ϸ�, bool��)
            // �ش� ��ũ��Ʈ�� �� ������Ʈ�� �̹����� �����Ѵ� + Vector3.up
            // ���ϸ��� spawnPointImg ����� "Enemy" ��� �����̸�
            // �̴� ����Ƽ�� ��ü������  Assets/Gizmos/ ���
            // ���� �ȿ� ������ �˻��Ѵ�
            Gizmos.DrawIcon(transform.position + Vector3.up, spawnPointImg, true);
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}

