using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float moveDamping = 15f;   // 이동 속도 계수
    public float rotateDamping = 10f;  // 회전 속도 계수
    public float ditance = 5f;               // 추적 대상과의 거리
    public float height = 4f;                // 추적 대상과의 높이
    public float targetOffset = 2f;       // 추적 좌표의 오프셋

    Transform tr;

    [Header("벽 감지 관련")]
    public float heightAboveWall = 7f;
    public float colliderRadius = 1f;
    public float overDamping = 5f;
    float originHeight;

    [Header("장애물 감지 관련")]
    public float heightAboveObstacle = 12f;
    // 플레이어한테 뿌릴 레이캐스트 높이 오프셋
    public float castOffset = 1f;

    void Start()
    {
        tr = GetComponent<Transform>();
        originHeight = height;
    }

    private void Update()
    {
        #region 벽 충돌
        // CheckSphere (생성위치, 반경)
        // 충돌 유무 체크해서 충돌할 경우 높이를 부드럽게 상승한다
        if (Physics.CheckSphere(tr.position, colliderRadius))
        {
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else // 뭔가 충돌을 안하면 원래 높이로 부드럽게 조정
        {
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }
        #endregion // 장애물 충돌

        #region 장애물 충돌
        // 레이캐스트가 겨냥할 지점을 설정
        // 플레이어의 발바닥 위치에서 조금 윗쪽
        Vector3 castTarget = target.position + (target.up * castOffset);
        // 방향 벡터를 계산함 (A - B) B가 A를 바라본다
        Vector3 castDir = (castTarget - tr.position).normalized;

        RaycastHit hit;

        if (Physics.Raycast(tr.position, castDir, out hit, Mathf.Infinity))
        {
            // 플레이어가 레이캐스트에 충돌하지 않았다 = 장애물이 있다는 말
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
        // var 변수는 가변 자료형이라고 볼 수 있지만,
        // 처음 값이 설정될 때 지정된 타입으로 굳혀진다
        // 이 후 타입의 변경은 불가하다
        // 코드의 유연성을 위해서 사용한다
        var camPos = target.position - (target.forward * ditance) + (target.up * height);
        //                                                           0, 0, 5             +             0, 4, 0

        // 이동할 때 속도 계수 적용
        // Lerp는 자동차 운전을 생각하면 쉬운데 가속과 감속을 할 때 멈춰 있다가 한번에 훅 가는 느낌이 아니고
        // 부드러운 가속, 부드러운 감속을 합니다 즉, 카메라는 target을 향해 부드러운 움직임을 위해 Lerp 함수를 사용합니다
        tr.position = Vector3.Lerp(tr.position, camPos, Time.deltaTime * moveDamping);

        // 회전할 때 속도 계수를 구해보자
        // 반면에 Slerp는 Lerp와 다르게 곡선형태로 가속과 감속을 합니다
        // 야구공을 던지면 포물선으로 운동하게 되는데 마찬가지로 카메라는 target 기준에서 회전을 할 때 곡선운동을 합니다
        tr.rotation = Quaternion.Slerp(tr.rotation, target.rotation, Time.deltaTime * rotateDamping);

        // 카메라가 위치 및 회전 이동 후에 타겟을 바라보도록
        // 플레이어의 pivot의 위치를 보정하여 카메라가 플레이어의 머리부분을 바라보게함
        tr.LookAt(target.position + (target.up * targetOffset));
    }

    // 기즈모라는 씬뷰에서 보이는 가상의 라인들을 확인하기 위한 메소드
    private void OnDrawGizmos()
    {
        // 기즈모의 색상 지정
        Gizmos.color = Color.green;

        // DrawWireSphere(위치, 반경)
        // 기즈모에는 몇가지 지정된 모양이 있음
        // DrawWireSphere는 선으로 이루어진 구형 모양
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f);

        // 메인 카메라의 대상간의 선을 표시함
        // DrawLine(A위치, B위치) = A와 B 사의 선 긋기
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position);

        // 카메라의 충돌체 표현
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);

        // 플레이어 캐릭터가 장애물이 가려졌는지 판단할 레이
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position + (target.up * castOffset), transform.position);
    }
}
