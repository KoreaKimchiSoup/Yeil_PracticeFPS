using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// EnemyFOV 라는 스크립트에 사용될 커스텀 에디터라고 명시함
// 그래야 유니티에서 인식함
[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        // EnemyFOV 클래스를 참조
        EnemyFOV fov = (EnemyFOV)target;

        // 원 주위의 시작점의 좌표를 계산(뷰앵글 120도의 1/2)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        // 색상을 화이트로 설정
        Handles.color = Color.white;

        // 선으로 이루어진 원을 그림
        Handles.DrawWireDisc(fov.transform.position, //원점 좌표
                                          Vector3.up,                   // 노말 벡터
                                          fov.viewAngle);             // 원의 반지름

        // 흰색이지만 투명도가 20퍼짜리 색상을 지정
        Handles.color = new Color(1, 1, 1, 0.2f);

        Handles.DrawSolidArc(fov.transform.position,  // 원점 좌표
                                         Vector3.up,                    // 노말 벡터
                                         fromAnglePos,                // 부채꼴 시작 위치
                                         fov.viewAngle,               // 부채꼴 각도
                                         fov.viewRange);             // 부채꼴 범위
        // 텍스트 출력
        Handles.Label(fov.transform.position +
                             (fov.transform.forward * 2f),
                              fov.viewAngle.ToString("ㅁㄴㅇ"));
    }
}
