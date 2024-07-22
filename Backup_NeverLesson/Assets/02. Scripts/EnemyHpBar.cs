using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    Camera uiCamera; // 캔버스를 찍고 있는 카메라
    Canvas canvas;

    RectTransform rectParent;
    RectTransform rectHp;

    [HideInInspector]
    public Vector3 offset = Vector3.zero; // HpBar 이미지의 위치 조절용 오프셋
    [HideInInspector]
    public Transform targetTr; // 추적 대상 Transform 컴포넌트

    void Start()
    {
        // 동적생성 될 때 부모인 캔버스를 가져오기 위해서
        // InParent를 사용한다
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        // 월드 좌표 -> 스크린 좌표로 변환 (Camera.main은 Main Camera 라는 태그를 가진 카메라를 의미함) Main Camera 태그가 없으면 널뜸
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);

        // 카메라의 뒷 쪽으로 갈 때 좌표값을 보정한다
        // 정면으로 볼 때의 hpBar와 뒤에서 봤을 때의 hpBar는 반전되어 보여서 그럼
        if (screenPos.z < 0f)
        {
            screenPos *= -1;
        }

        var localPos = Vector2.zero;
        // 스크린 좌표 -> 렉트트랜스폼 좌표로 변환함
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        // 최종적으로 변환된 RectTransform 좌표를 rectHp 에 전달한다
        rectHp.localPosition = localPos;
    }
}
