using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 이벤트에 연결하여 호출 되는 메서드는 public 으로 선언되며
    // 해당 스크립트는 반드시 하이어라키에 올라가야된다
    public void OnClickStartButton()
    {
        print("스타트 버튼 클릭");
    }
}
