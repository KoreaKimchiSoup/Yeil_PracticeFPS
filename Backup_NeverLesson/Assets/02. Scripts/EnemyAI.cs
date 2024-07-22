using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL;

    Transform playerTR; // 플레이어 위치 저장 변수
    Transform enemyTR; // 적의 위치 저장 변수 (자기 자신)
    Animator animator;

    // 애니메이션 제어 파라미터
    readonly int hashMove = Animator.StringToHash("IsMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieIndex = Animator.StringToHash("DieIndex");
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    public float attackDist = 5f; // 공격 사거리
    public float traceDist = 10f; // 추적 사거리
    public bool isDie = false; // 사망 여부 판단 변수

    MoveAgent moveAgent; // Enemy 움직임을 제어하는 MoveAgent 스크립트 가져오기
    WaitForSeconds waitTime; // 코루틴에서 사용할 시간 지연 변수
    EnemyFire enemyFire;

    EnemyFOV enemyFOV;

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        if (player != null)
        {
            playerTR = player.GetComponent<Transform>();
        }

        enemyTR = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();
        enemyFOV = GetComponent<EnemyFOV>();

        waitTime = new WaitForSeconds(0.3f);

        animator.SetFloat(hashOffset, Random.Range(0f, 1f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1f, 1.2f));
    }

    private void OnEnable()
    {
        // StartCoroutine("CheckState"); 방법은 2가지 그러나 문자열보단 함수이름을 그대로 가져다주자
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        // DamageScript 에 있는 정적 이벤트에
        // EnemyAI 스크립트의 E_PlayerDie 메서드를 연결시킨다
        // 이벤트 연결 시에는 += 뺄때는 -= 을 쓴다
        DamageScript.PlayerDieEvent += E_PlayerDie;
    }

    private void OnDisable()
    {
        // 이벤트가 종료되어도 GC에 의해서 메모리 반환이 안되는 경우가 있다
        // 그런 이유로 메모리의 사용량이 조금씩 쌓이며
        // 언젠가는 터질 위험이 있으므로
        // 이를 방지하기 위해 -= 으로 이벤트 연결을 해제한다
        DamageScript.PlayerDieEvent -= E_PlayerDie;
    }

    IEnumerator CheckState()
    {
        // 오브젝트 풀 등 여러 스크립트들이 준비가 끝날때 까지 잠시 대기한다
        yield return new WaitForSeconds(1f);

        // 생존해 있는 동안 무한루프
        while (!isDie)
        {
            // 상태가 사망이면 코루틴 함수를 종료함
            if (state == State.DIE)
            {
                yield break;
            }
            // Distance(A, B) = A와 B사이의 거리를 계산함
            float dist = Vector3.Distance(playerTR.position, enemyTR.position);

            if (dist <= attackDist) // A와 B 사이의 거리가 attackDist 보다 작거나 같다면
            {
                // 플레이어가 내 시야에 보일때 (장애물 없이)
                if (enemyFOV.isViewPlayer())
                {
                    state = State.ATTACK; // 공격 상태로 바꿈
                }
                else
                {
                    state = State.TRACE;
                }
            }
            else if (enemyFOV.isTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            yield return waitTime;
        }
    }

    //  변화된 State에 따라서 실제 행동을 처리하는 코루틴 함수
    IEnumerator Action()
    {
        while(!isDie)
        {
            yield return waitTime;

            switch(state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true; // 순찰 실행
                    animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTR.position; // 추격할땐 플레이어의 포지션을 따라감
                    animator.SetBool(hashMove, true);
                    break;

                case State.ATTACK:
                    moveAgent.Stop(); // 상태가 ATTACK으로 바뀌면 이동을 멈춤
                    animator.SetBool(hashMove, false);
                    
                    if(!enemyFire.isFire) // 만약 isFire가 false라면
                    {
                        enemyFire.isFire = true;
                    }
                    break;

                case State.DIE:
                    // Enemy가 죽었을 때 Enemy Count에서 제외되도록 한다
                    gameObject.tag = "Untagged";

                    isDie = true;
                    enemyFire.isFire = false;

                    moveAgent.Stop(); // 상태가 DIE로 바뀌면 이동을 멈춤

                    animator.SetInteger(hashDieIndex, Random.Range(0, 3)); // 0 ~ 2
                    animator.SetTrigger(hashDie);

                    // 사망이후 콜라이더 비활성화 하여 총알이 맞지 않음
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    private void Update()
    {
        // moveAgent가 가지고 있는 speed 프로퍼티 값을
        // 애니메이터의 speed 파라미터에 전달하여 속도를 변경함
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void E_PlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        // 동작중인 코루틴 모두 종료
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);
    }
}
