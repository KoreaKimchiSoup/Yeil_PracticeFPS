using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // 네비게이션AI를 사용하기 위한 라이브러리

// 해당 어트리뷰트를 통해서 필수(반드시) 컴포넌트를 지정함 (없으면 빌드가 안되게 한다)
[RequireComponent(typeof(MoveAgent))]

public class MoveAgent : MonoBehaviour
{
    // 리스트는 가변 배열 길이로써
    // 데이터가 추가/삭제 될 때 따라서 길이 및 인덱스가 바뀐다
    public List<Transform> wayPoints; // AI가 돌아다닐 웨이포인트를 리스트로 선언하려 해당 리스트에 위치값을 저장함
    public int nextIndex; // 다음 순찰지점의 인덱스를 지정하는 변수

    NavMeshAgent agent; // 인스펙터에서 NavMeshAgent 컴포넌트를 붙인 오브젝트를 컨트롤하기 위한 변수
                                     // 해당 변수를 통하여 NavMeshAgent에 있는 프로퍼티를 사용한다
    Transform enemyTr;   // enemy의 트랜스폼

    // 프로퍼티 관련
    readonly float patrolSpeed = 1.5f; // 순찰할때의 속도
    readonly float traceSpeed = 4f;     // 추적할때의 속도
    float damping = 1f; // 회전속도를 조절 계수

    bool _patrolling; // 실제 값이 저장되는 변수
    public bool patrolling //  대외적으로 표출되는 프로퍼티
    {
        get
        {
            return _patrolling;
        }
        set
        {
            // value는 외부에서 전달되는 값
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
            // 대상 추적 메소드 호출
            TraceTarget(_traceTartget);
        }
    }

    public float speed
    {

        // 속도의 크기는 방향을 고려하지 않고 물체가 얼마나 빨리 움직이는지를 나타내는 "속도"라고도 합니다.
        // 반면, 속도 벡터에는 물체가 움직이는 속도와 방향이 모두 포함됩니다. 
        get { return agent.velocity.magnitude; }
    }

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        // 목적지에 가까워질수록 속도 줄이는 옵션을 비활성화 한다
        agent.autoBraking = false;
        agent.updateRotation = false;

        agent.speed = patrolSpeed;

        // 하이어라키뷰에서 모든 오브젝트를 검색하여
        // WayPointGroup 이름의 오브젝트 검색 (성능상의 문제가 생길 수 있음)
        var group = GameObject.Find("WayPointGroup");

        if (group != null)
        {
            // WayPointGroup 하위에 있는 모든 Transform 컴포넌트를 추출함
            // 추출된 컴포넌트를 List wayPoints에 자동으로 추가한다
            // 다만 이 때 주의할 점은 ~s InChildren 메서드는
            // 부모 오브젝트가 0번째에 들어가고 이후 자식이 들어간다 (부모 오브젝트 + 자식 오브젝트)
            group.GetComponentsInChildren<Transform>(wayPoints);
            // 따라서, 0번째 index 요소를 지움으로써 부모 오브젝트를 제외한다
            wayPoints.RemoveAt(0);

            // 첫 번째 순찰 위치를 랜덤하게 추출한다
            nextIndex = Random.Range(0, wayPoints.Count);
        }

        // 웨이포인트로 움직이는 메소드 호출
        // MoveWayPoint();
        this.patrolling = true;
    }

    void MoveWayPoint()
    {
        // 에이전트가 최단경로 계산'중'이라면 이동을 하지 않음
        if (agent.isPathStale)
        { 
            return;
        }

        // 경로 계산이 끝났다면
        // 다음 목적지를 리스트에서 추출한 위치로 설정함
        agent.destination = wayPoints[nextIndex].position;
        // 에이전트 활성화
        agent.isStopped = false;
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) // Enemy가 경로를 계산중 일 때
        {
            return;
        }

        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop() // 스탑 함수를 호출하면
    {
        agent.isStopped = true; // 
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    void Update()
    {
        // 이동중 이라면 (멈춘게 아니라면) 즉, 이동이 멈춘경우(공격, 다이 일때는 회전하지 않음)
        if (!agent.isStopped)
        {
            // NavMeshAgent가 가야할 방향을 쿼터니언 각도로 변환한다
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            // 위에서 계산한 방향 각도를 토대로 Enemy의 rotation 값을 변경한다
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        // 순찰 상태가 아닐경우(추적상태) 에는 순찰지점 변경 로직을 수행하지 않게 함
        if (!_patrolling)
        {
            return;
        }

        // 움직이는 중이나 목적지에 거의 도착한 상태
        // 다음 목적지를 결정하기 위한 단계
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            //nextIndex++;
            //nextIndex = nextIndex % wayPoints.Count; // 배열을 순환하여 돌게됨
            nextIndex = Random.Range(0, wayPoints.Count);

            MoveWayPoint();
        }
    }
}
