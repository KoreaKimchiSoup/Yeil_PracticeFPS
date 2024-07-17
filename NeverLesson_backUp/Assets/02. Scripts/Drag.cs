using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static GameObject draggingItem = null;
    Transform itemTr;
    Transform inventoryTr;

    Transform itemListTr;
    CanvasGroup canvasGroup;

    void Start()
    {
        itemTr = GetComponent<Transform>();
        inventoryTr = GameObject.Find("Inventory").GetComponent<Transform>();
        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    // 드래그핸들러 인터페이스의 메서드 구현
    // 인터페이스가 가지고 있는 메서드는 반드시 자식 클래스에서 구현해야함
    // 이것이 바로 오버라이딩
    // 마우스 드래그중 일 때 (Stay)
    public void OnDrag(PointerEventData eventData)
    {
        itemTr.position = Input.mousePosition;
    }

    // 마우스 드래그가 시작될 때 호출되는 메서드 (Enter)
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 부모를 Inventory로 변경한다
        this.transform.SetParent(inventoryTr);
        // 드래그가 시작될 때 드래그되는 아이템 정보 저장
        draggingItem = this.gameObject;

        // 드래그가 시작될때 다른 UI 이벤트를 받지 않도록 설정함
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그가 끝날 때 드래그 아이템을 null로 설정
        draggingItem = null;
        // 드래그가 끝나면 다시 UI 이벤트 활성화함
        canvasGroup.blocksRaycasts = true;

        if (itemTr.parent == inventoryTr)
        {
            // Slot에 아이템을 놓지 않으면
            // 다시 itemListTr로 되돌아간다
            itemTr.SetParent(itemListTr.transform);
        }
    }
}
