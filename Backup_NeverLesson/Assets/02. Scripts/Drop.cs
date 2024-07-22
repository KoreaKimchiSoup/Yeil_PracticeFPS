using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    // 플레이어에게 기능을 전달할 변수
    PlayerController playerController;

    // 플레이어 체력 변수
    DamageScript playerHp;

    // 아래의 스크립트가 붙어진 게임오브젝트를 찾기 위한 변수
    // ItemList의 자식으로 설정하기 위해
    // ItemList 오브젝트에 FindItemList 스크립트를 추가하였음
    WaitForSeconds waitForSeconds = new WaitForSeconds(3f);

    // 플레이어 움직임 속도를 저장할 변수
    float playerMoveSpeed;
    // Update 메서드에서 코루틴이 계속 실행되는것을 방지하기 위한 변수
    bool isCoroutineActive;
    // Hp 증가 아이템의 회복 수치
    float increaseHp = 20f;

    private void Awake()
    {
        // 이 방법은 특정 타입의 컴포넌트가 부착된 '첫 번째' GameObject를 찾습니다.
        // 예를 들어, 게임에서 PlayerController 컴포넌트를 가진 오브젝트를 찾으려면
        // 다음과 같이 사용할 수 있습니다:
        playerController = FindObjectOfType<PlayerController>();
        playerHp = FindObjectOfType<DamageScript>();
        isCoroutineActive = true;
    }

    private void Start()
    {
        // 플레이어를 참조하는 변수로 플레이어 속도를 접근하여
        // playerMoveSpeed 변수에 저장한다
        // 플레이어 속도의 원본데이터가 바뀌면 아래의 변수값도 같이 바뀐다
        playerMoveSpeed = playerController.moveSpeed;
    }

    //class ABC
    //{
    //}

    void Update()
    {
        //List<ABC> abc = new List<ABC>();
        //for (int i = 0; i < abc.Count; i++)
        //{
        //    abc[3]
        //}

        /* 
        foreach 루프의 작동 방식
        foreach 루프는 컬렉션 내의 각 항목에 대해 코드 블록을 한 번씩 실행합니다. 
        여기서 컬렉션은 transform의 자식들이며, 각 child는 Transform 타입의 객체입니다.
        루프는 부모 오브젝트의 첫 번째 자식에서 시작하여, 
        마지막 자식까지 차례로 각각의 Transform 컴포넌트에 접근합니다.

        사용 예제
        이러한 코드는 게임 오브젝트의 모든 자식을 검사하거나 변형할 때 유용합니다. 
        예를 들어, 특정 태그를 가진 자식을 찾거나, 모든 자식의 색상을 변경하는 등의 작업에 사용될 수 있습니다.
        */
        foreach (Transform child in transform)
        {
            if (child.CompareTag("SPEEDUP") && isCoroutineActive)
            {
                StartCoroutine(PlayerSpeedUp());
            }
            else if (child.CompareTag("HPUP") && isCoroutineActive)
            {
                StartCoroutine(PlayerHpUp());
            }
            else if (child.CompareTag("GRANADE") && isCoroutineActive)
            {
                // 수류탄 발사 메서드
            }
            else if (child.CompareTag("SHORK") && isCoroutineActive)
            {
                // 쇼크? 발생 메서드
            }
        }
    }

    IEnumerator PlayerSpeedUp()
    {
        //  Update 메서드에서 계속 실행하지 못하게 실행 중일 땐 false로 세팅
        isCoroutineActive = false;
        Debug.Log("스피드 업 코루틴 실행");
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");

        // SpeedItem 클릭 불가능하게
        //itemCanvasGroup.interactable = false;
        //itemCanvasGroup.blocksRaycasts = false;

        // 플레이어 속도 2배 증가
        Debug.Log("속도 2배 증가");
        playerController.moveSpeed = playerMoveSpeed * 2;

        // 3초 대기
        Debug.Log("3초 대기");
        yield return waitForSeconds;

        // 플레이어 속도를 다시 원상복구한다
        Debug.Log("속도 복구");
        playerController.moveSpeed = playerMoveSpeed;

        // 쿨타임이 끝나고 다시 클릭이 가능함
        //itemCanvasGroup.interactable = true;
        //itemCanvasGroup.blocksRaycasts = true;

        // 다시 아이템의 기능을 이용하기 위하여 isCoroutineActive = true 원상복구
        isCoroutineActive = true;
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");
    }
    IEnumerator PlayerHpUp()
    {
        //  Update 메서드에서 계속 실행하지 못하게 실행 중일 땐 false로 세팅
        isCoroutineActive = false;
        Debug.Log("Hp 회복 코루틴 실행");
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");

        // HpItem 클릭 불가능하게
        // itemCanvasGroup.interactable = false;
        // itemCanvasGroup.blocksRaycasts = false;

        // 플레이어 +20 hp 증가
        // 플레이어 hp는 Damege 스크립트에 있음
        Debug.Log("hp 20증가");
        playerHp.currHp += increaseHp;
        Debug.Log($"현재 hp: {playerHp.currHp}");

        // 플레이어 hpBar 체력증가 적용
        playerHp.DisplayHpBar();

        // 3초 대기
        Debug.Log("3초 대기");
        yield return waitForSeconds;

        // 속도증가 아이콘이 다시 ItemList 오브젝트의 자식으로 복귀하여
        // 코루틴이 재실행 되는것을 방지하기 위한 코드

        // 1. ItemList 오브젝트를 참조하는 변수를 만들고
        // 2. ItemSpeed 오브젝트의 부모를 참조한 변수로 설정한다

        //transform.SetParent(findItemList.transform, false);
        // SetParent메서드의 bool 인자는
        // 부모의 위치, 회전,  스케일을 상속받을지에 대한 조건입니다

        // 쿨타임이 끝나고 다시 클릭이 가능함
        // itemCanvasGroup.interactable = true;
        // itemCanvasGroup.blocksRaycasts = true;

        // 다시 아이템의 기능을 이용하기 위하여 true 원상복구
        isCoroutineActive = true;
        Debug.Log($"isCoroutineActive = {isCoroutineActive}");
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Slot의 자식의 갯수가 0 이라는 의미는
        // 하위 오브젝트가 없는 경우를 말한다
        if (transform.childCount == 0)
        {
            // 드래그 중인 아이템을 Slot에 등록할 경우
            // Drop 스크립트가 있는 오브젝트를
            // Slot의 자식으로 설정한다
            // 참고, Drag 클래스의 draggingItem은 static 이며
            // 다이렉트로 참조가 가능함
            Drag.draggingItem.transform.SetParent(this.transform);
        }
    }
}
