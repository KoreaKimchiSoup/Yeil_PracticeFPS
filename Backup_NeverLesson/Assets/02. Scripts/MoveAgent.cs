using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // �׺���̼�AI�� ����ϱ� ���� ���̺귯��

// �ش� ��Ʈ����Ʈ�� ���ؼ� �ʼ�(�ݵ��) ������Ʈ�� ������ (������ ���尡 �ȵǰ� �Ѵ�)
[RequireComponent(typeof(MoveAgent))]

public class MoveAgent : MonoBehaviour
{
    // ����Ʈ�� ���� �迭 ���̷ν�
    // �����Ͱ� �߰�/���� �� �� ���� ���� �� �ε����� �ٲ��
    public List<Transform> wayPoints; // AI�� ���ƴٴ� ��������Ʈ�� ����Ʈ�� �����Ϸ� �ش� ����Ʈ�� ��ġ���� ������
    public int nextIndex; // ���� ���������� �ε����� �����ϴ� ����

    NavMeshAgent agent; // �ν����Ϳ��� NavMeshAgent ������Ʈ�� ���� ������Ʈ�� ��Ʈ���ϱ� ���� ����
                                     // �ش� ������ ���Ͽ� NavMeshAgent�� �ִ� ������Ƽ�� ����Ѵ�
    Transform enemyTr;   // enemy�� Ʈ������

    // ������Ƽ ����
    readonly float patrolSpeed = 1.5f; // �����Ҷ��� �ӵ�
    readonly float traceSpeed = 4f;     // �����Ҷ��� �ӵ�
    float damping = 1f; // ȸ���ӵ��� ���� ���

    bool _patrolling; // ���� ���� ����Ǵ� ����
    public bool patrolling //  ��������� ǥ��Ǵ� ������Ƽ
    {
        get
        {
            return _patrolling;
        }
        set
        {
            // value�� �ܺο��� ���޵Ǵ� ��
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1f;
                MoveWayPoint();
            }
        }
    }

    Vector3 _traceTartget;
    public Vector3 traceTarget
    {
        get { return _traceTartget; }
        set
        {
            _traceTartget = value;
            agent.speed = traceSpeed;
            damping = 7f;
            // ��� ���� �޼ҵ� ȣ��
            TraceTarget(_traceTartget);
        }
    }

    public float speed
    {

        // �ӵ��� ũ��� ������ ������� �ʰ� ��ü�� �󸶳� ���� �����̴����� ��Ÿ���� "�ӵ�"��� �մϴ�.
        // �ݸ�, �ӵ� ���Ϳ��� ��ü�� �����̴� �ӵ��� ������ ��� ���Ե˴ϴ�. 
        get { return agent.velocity.magnitude; }
    }

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        // �������� ����������� �ӵ� ���̴� �ɼ��� ��Ȱ��ȭ �Ѵ�
        agent.autoBraking = false;
        agent.updateRotation = false;

        agent.speed = patrolSpeed;

        // ���̾��Ű�信�� ��� ������Ʈ�� �˻��Ͽ�
        // WayPointGroup �̸��� ������Ʈ �˻� (���ɻ��� ������ ���� �� ����)
        var group = GameObject.Find("WayPointGroup");

        if (group != null)
        {
            // WayPointGroup ������ �ִ� ��� Transform ������Ʈ�� ������
            // ����� ������Ʈ�� List wayPoints�� �ڵ����� �߰��Ѵ�
            // �ٸ� �� �� ������ ���� ~s InChildren �޼����
            // �θ� ������Ʈ�� 0��°�� ���� ���� �ڽ��� ���� (�θ� ������Ʈ + �ڽ� ������Ʈ)
            group.GetComponentsInChildren<Transform>(wayPoints);
            // ����, 0��° index ��Ҹ� �������ν� �θ� ������Ʈ�� �����Ѵ�
            wayPoints.RemoveAt(0);

            // ù ��° ���� ��ġ�� �����ϰ� �����Ѵ�
            nextIndex = Random.Range(0, wayPoints.Count);
        }

        // ��������Ʈ�� �����̴� �޼ҵ� ȣ��
        // MoveWayPoint();
        this.patrolling = true;
    }

    void MoveWayPoint()
    {
        // ������Ʈ�� �ִܰ�� ���'��'�̶�� �̵��� ���� ����
        if (agent.isPathStale)
        { 
            return;
        }

        // ��� ����� �����ٸ�
        // ���� �������� ����Ʈ���� ������ ��ġ�� ������
        agent.destination = wayPoints[nextIndex].position;
        // ������Ʈ Ȱ��ȭ
        agent.isStopped = false;
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) // Enemy�� ��θ� ����� �� ��
        {
            return;
        }

        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop() // ��ž �Լ��� ȣ���ϸ�
    {
        agent.isStopped = true; // 
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    void Update()
    {
        // �̵��� �̶�� (����� �ƴ϶��) ��, �̵��� ������(����, ���� �϶��� ȸ������ ����)
        if (!agent.isStopped)
        {
            // NavMeshAgent�� ������ ������ ���ʹϾ� ������ ��ȯ�Ѵ�
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            // ������ ����� ���� ������ ���� Enemy�� rotation ���� �����Ѵ�
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        // ���� ���°� �ƴҰ��(��������) ���� �������� ���� ������ �������� �ʰ� ��
        if (!_patrolling)
        {
            return;
        }

        // �����̴� ���̳� �������� ���� ������ ����
        // ���� �������� �����ϱ� ���� �ܰ�
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            //nextIndex++;
            //nextIndex = nextIndex % wayPoints.Count; // �迭�� ��ȯ�Ͽ� ���Ե�
            nextIndex = Random.Range(0, wayPoints.Count);

            MoveWayPoint();
        }
    }
}
