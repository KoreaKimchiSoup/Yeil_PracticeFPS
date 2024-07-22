using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float moveDamping = 15f;   // �̵� �ӵ� ���
    public float rotateDamping = 10f;  // ȸ�� �ӵ� ���
    public float ditance = 5f;               // ���� ������ �Ÿ�
    public float height = 4f;                // ���� ������ ����
    public float targetOffset = 2f;       // ���� ��ǥ�� ������

    Transform tr;

    [Header("�� ���� ����")]
    public float heightAboveWall = 7f;
    public float colliderRadius = 1f;
    public float overDamping = 5f;
    float originHeight;

    [Header("��ֹ� ���� ����")]
    public float heightAboveObstacle = 12f;
    // �÷��̾����� �Ѹ� ����ĳ��Ʈ ���� ������
    public float castOffset = 1f;

    void Start()
    {
        tr = GetComponent<Transform>();
        originHeight = height;
    }

    private void Update()
    {
        #region �� �浹
        // CheckSphere (������ġ, �ݰ�)
        // �浹 ���� üũ�ؼ� �浹�� ��� ���̸� �ε巴�� ����Ѵ�
        if (Physics.CheckSphere(tr.position, colliderRadius))
        {
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else // ���� �浹�� ���ϸ� ���� ���̷� �ε巴�� ����
        {
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }
        #endregion // ��ֹ� �浹

        #region ��ֹ� �浹
        // ����ĳ��Ʈ�� �ܳ��� ������ ����
        // �÷��̾��� �߹ٴ� ��ġ���� ���� ����
        Vector3 castTarget = target.position + (target.up * castOffset);
        // ���� ���͸� ����� (A - B) B�� A�� �ٶ󺻴�
        Vector3 castDir = (castTarget - tr.position).normalized;

        RaycastHit hit;

        if (Physics.Raycast(tr.position, castDir, out hit, Mathf.Infinity))
        {
            // �÷��̾ ����ĳ��Ʈ�� �浹���� �ʾҴ� = ��ֹ��� �ִٴ� ��
            if (!hit.collider.CompareTag("PLAYER"))
            {
                height = Mathf.Lerp(height, heightAboveObstacle, Time.deltaTime * overDamping);
            }
            else
            {
                height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
            }
        }
        #endregion
    }

    void LateUpdate()
    {
        // var ������ ���� �ڷ����̶�� �� �� ������,
        // ó�� ���� ������ �� ������ Ÿ������ ��������
        // �� �� Ÿ���� ������ �Ұ��ϴ�
        // �ڵ��� �������� ���ؼ� ����Ѵ�
        var camPos = target.position - (target.forward * ditance) + (target.up * height);
        //                                                           0, 0, 5             +             0, 4, 0

        // �̵��� �� �ӵ� ��� ����
        // Lerp�� �ڵ��� ������ �����ϸ� ��� ���Ӱ� ������ �� �� ���� �ִٰ� �ѹ��� �� ���� ������ �ƴϰ�
        // �ε巯�� ����, �ε巯�� ������ �մϴ� ��, ī�޶�� target�� ���� �ε巯�� �������� ���� Lerp �Լ��� ����մϴ�
        tr.position = Vector3.Lerp(tr.position, camPos, Time.deltaTime * moveDamping);

        // ȸ���� �� �ӵ� ����� ���غ���
        // �ݸ鿡 Slerp�� Lerp�� �ٸ��� ����·� ���Ӱ� ������ �մϴ�
        // �߱����� ������ ���������� ��ϰ� �Ǵµ� ���������� ī�޶�� target ���ؿ��� ȸ���� �� �� ���� �մϴ�
        tr.rotation = Quaternion.Slerp(tr.rotation, target.rotation, Time.deltaTime * rotateDamping);

        // ī�޶� ��ġ �� ȸ�� �̵� �Ŀ� Ÿ���� �ٶ󺸵���
        // �÷��̾��� pivot�� ��ġ�� �����Ͽ� ī�޶� �÷��̾��� �Ӹ��κ��� �ٶ󺸰���
        tr.LookAt(target.position + (target.up * targetOffset));
    }

    // ������� ���信�� ���̴� ������ ���ε��� Ȯ���ϱ� ���� �޼ҵ�
    private void OnDrawGizmos()
    {
        // ������� ���� ����
        Gizmos.color = Color.green;

        // DrawWireSphere(��ġ, �ݰ�)
        // ����𿡴� ��� ������ ����� ����
        // DrawWireSphere�� ������ �̷���� ���� ���
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f);

        // ���� ī�޶��� ����� ���� ǥ����
        // DrawLine(A��ġ, B��ġ) = A�� B ���� �� �߱�
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position);

        // ī�޶��� �浹ü ǥ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);

        // �÷��̾� ĳ���Ͱ� ��ֹ��� ���������� �Ǵ��� ����
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position + (target.up * castOffset), transform.position);
    }
}
