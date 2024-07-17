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

    Transform playerTR; // �÷��̾� ��ġ ���� ����
    Transform enemyTR; // ���� ��ġ ���� ���� (�ڱ� �ڽ�)
    Animator animator;

    // �ִϸ��̼� ���� �Ķ����
    readonly int hashMove = Animator.StringToHash("IsMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieIndex = Animator.StringToHash("DieIndex");
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    public float attackDist = 5f; // ���� ��Ÿ�
    public float traceDist = 10f; // ���� ��Ÿ�
    public bool isDie = false; // ��� ���� �Ǵ� ����

    MoveAgent moveAgent; // Enemy �������� �����ϴ� MoveAgent ��ũ��Ʈ ��������
    WaitForSeconds waitTime; // �ڷ�ƾ���� ����� �ð� ���� ����
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
        // StartCoroutine("CheckState"); ����� 2���� �׷��� ���ڿ����� �Լ��̸��� �״�� ����������
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        // DamageScript �� �ִ� ���� �̺�Ʈ��
        // EnemyAI ��ũ��Ʈ�� E_PlayerDie �޼��带 �����Ų��
        // �̺�Ʈ ���� �ÿ��� += ������ -= �� ����
        DamageScript.PlayerDieEvent += E_PlayerDie;
    }

    private void OnDisable()
    {
        // �̺�Ʈ�� ����Ǿ GC�� ���ؼ� �޸� ��ȯ�� �ȵǴ� ��찡 �ִ�
        // �׷� ������ �޸��� ��뷮�� ���ݾ� ���̸�
        // �������� ���� ������ �����Ƿ�
        // �̸� �����ϱ� ���� -= ���� �̺�Ʈ ������ �����Ѵ�
        DamageScript.PlayerDieEvent -= E_PlayerDie;
    }

    IEnumerator CheckState()
    {
        // ������Ʈ Ǯ �� ���� ��ũ��Ʈ���� �غ� ������ ���� ��� ����Ѵ�
        yield return new WaitForSeconds(1f);

        // ������ �ִ� ���� ���ѷ���
        while (!isDie)
        {
            // ���°� ����̸� �ڷ�ƾ �Լ��� ������
            if (state == State.DIE)
            {
                yield break;
            }
            // Distance(A, B) = A�� B������ �Ÿ��� �����
            float dist = Vector3.Distance(playerTR.position, enemyTR.position);

            if (dist <= attackDist) // A�� B ������ �Ÿ��� attackDist ���� �۰ų� ���ٸ�
            {
                // �÷��̾ �� �þ߿� ���϶� (��ֹ� ����)
                if (enemyFOV.isViewPlayer())
                {
                    state = State.ATTACK; // ���� ���·� �ٲ�
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

    //  ��ȭ�� State�� ���� ���� �ൿ�� ó���ϴ� �ڷ�ƾ �Լ�
    IEnumerator Action()
    {
        while(!isDie)
        {
            yield return waitTime;

            switch(state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true; // ���� ����
                    animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTR.position; // �߰��Ҷ� �÷��̾��� �������� ����
                    animator.SetBool(hashMove, true);
                    break;

                case State.ATTACK:
                    moveAgent.Stop(); // ���°� ATTACK���� �ٲ�� �̵��� ����
                    animator.SetBool(hashMove, false);
                    
                    if(!enemyFire.isFire) // ���� isFire�� false���
                    {
                        enemyFire.isFire = true;
                    }
                    break;

                case State.DIE:
                    // Enemy�� �׾��� �� Enemy Count���� ���ܵǵ��� �Ѵ�
                    gameObject.tag = "Untagged";

                    isDie = true;
                    enemyFire.isFire = false;

                    moveAgent.Stop(); // ���°� DIE�� �ٲ�� �̵��� ����

                    animator.SetInteger(hashDieIndex, Random.Range(0, 3)); // 0 ~ 2
                    animator.SetTrigger(hashDie);

                    // ������� �ݶ��̴� ��Ȱ��ȭ �Ͽ� �Ѿ��� ���� ����
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    private void Update()
    {
        // moveAgent�� ������ �ִ� speed ������Ƽ ����
        // �ִϸ������� speed �Ķ���Ϳ� �����Ͽ� �ӵ��� ������
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void E_PlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        // �������� �ڷ�ƾ ��� ����
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);
    }
}
