using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Slot�� �ڽ��� ������ 0 �̶�� �ǹ̴�
        // ���� ������Ʈ�� ���� ��츦 ���Ѵ�
        if (transform.childCount == 0)
        {
            // �巡�� ���� �������� slot�� �ڽ����� ����Ѵ�
            Drag.draggingItem.transform.SetParent(this.transform);
        }
    }
}
