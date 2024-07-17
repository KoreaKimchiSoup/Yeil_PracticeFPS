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

    // �巡���ڵ鷯 �������̽��� �޼��� ����
    // �������̽��� ������ �ִ� �޼���� �ݵ�� �ڽ� Ŭ�������� �����ؾ���
    // �̰��� �ٷ� �������̵�
    // ���콺 �巡���� �� �� (Stay)
    public void OnDrag(PointerEventData eventData)
    {
        itemTr.position = Input.mousePosition;
    }

    // ���콺 �巡�װ� ���۵� �� ȣ��Ǵ� �޼��� (Enter)
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �θ� Inventory�� �����Ѵ�
        this.transform.SetParent(inventoryTr);
        // �巡�װ� ���۵� �� �巡�׵Ǵ� ������ ���� ����
        draggingItem = this.gameObject;

        // �巡�װ� ���۵ɶ� �ٸ� UI �̺�Ʈ�� ���� �ʵ��� ������
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�װ� ���� �� �巡�� �������� null�� ����
        draggingItem = null;
        // �巡�װ� ������ �ٽ� UI �̺�Ʈ Ȱ��ȭ��
        canvasGroup.blocksRaycasts = true;

        if (itemTr.parent == inventoryTr)
        {
            // Slot�� �������� ���� ������
            // �ٽ� itemListTr�� �ǵ��ư���
            itemTr.SetParent(itemListTr.transform);
        }
    }
}
