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
    public Type type = Type.PATROLPOINT; // 기본 타입 설정

    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    private void OnDrawGizmos()
    {
        //만약 타입이 PATROLPOINT 라면
        if (type == Type.PATROLPOINT)
        {
            //기즈모 색상 
            Gizmos.color = _color;
            //기즈모 모양 (위치 ,크기)
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else //리스폰 포인트이면
        {
            Gizmos.color = _color;

            //DrawIcon(위치, 이미지 파일명, 스케일 적용 여부)
            //스케일 적용 여부의 경우는 씬뷰의 카메라 줌 인/아웃에
            //따라서 아이콘 크기가 커지고 작아지는 효과

            // DrawIcon(아이콘의 위치, 파일명, bool값)
            // 해당 스크립트가 들어간 오브젝트에 이미지를 생성한다 + Vector3.up
            // 파일명은 spawnPointImg 저장된 "Enemy" 라는 파일이며
            // 이는 유니티가 자체적으로  Assets/Gizmos/ 라는
            // 폴더 안에 파일을 검색한다
            Gizmos.DrawIcon(transform.position + Vector3.up, spawnPointImg, true);
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}

