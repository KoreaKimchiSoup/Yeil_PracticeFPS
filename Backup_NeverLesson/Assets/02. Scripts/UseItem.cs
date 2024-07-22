using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UseItem : MonoBehaviour
{
    // 플레이어에게 기능을 전달할 변수
    PlayerController playerController;

    DamageScript playerHp;

    // 부모의 정보를 가져오기 위한 변수
    Transform parentTransform;

    // 아래의 스크립트가 붙어진 게임오브젝트를 찾기 위한 변수
    // ItemList의 자식으로 설정하기 위해
    // ItemList 오브젝트에 FindItemList 스크립트를 추가하였음
    FindItemList findItemList;

    WaitForSeconds waitForSeconds = new WaitForSeconds(3f);

    public CanvasGroup itemCanvasGroup;



    // 플레이어 움직임 속도를 저장할 변수
    float playerMoveSpeed;
    // Update 메서드에서 코루틴이 계속 실행되는것을 방지하기 위한 변수
    bool isCoroutineActive;


    private void Awake()
    {
        // 이 방법은 특정 타입의 컴포넌트가 부착된 '첫 번째' GameObject를 찾습니다.
        // 예를 들어, 게임에서 PlayerController 컴포넌트를 가진 오브젝트를 찾으려면
        // 다음과 같이 사용할 수 있습니다:
        playerController = FindObjectOfType<PlayerController>();
        findItemList = FindObjectOfType<FindItemList>();
        playerHp = FindObjectOfType<DamageScript>();
        isCoroutineActive = false;
    }

    private void Start()
    {
        // 플레이어를 참조하는 변수로 플레이어 속도를 접근하여
        // playerMoveSpeed 변수에 저장한다
        // 플레이어 속도의 원본데이터가 바뀌면 아래의 변수값도 같이 바뀐다
        playerMoveSpeed = playerController.moveSpeed;
    }

    private void Update()
    {
        // 예를들어 parentTransform = transform.parent; 의 코드가
        // 게임시작 직 후 Awake나 Start에서 초기화 했고
        // 게임 도중(시점은 알 수 없음) ItemSpeed 아이템이 다른 오브젝트의 자식으로 설정되었을 때
        // 다시 부모의 태그를 검사해야할 상황에서 초기화를 하지 못하기 때문에
        // Update 메서드에서 항상 부모의 태그를 parentTransform에 최신화 하기 위한 코드이다
        parentTransform = transform.parent;

        // ItempSpeed의 부모 오브젝트가 있는지 검사하고
        // 있다면 태그가 "SLOT"인지 검사한다
        if (parentTransform != null && parentTransform.CompareTag("SLOT"))
        {
            if (!isCoroutineActive)
            {
                //if (false)
                //{
                //    StartCoroutine(PlayerSpeedUp());
                //}
                //else
                //{
                //    StartCoroutine(PlayerHpUp());
                //}
            }
        }
    }

    IEnumerator PlayerSpeedUp()
    {
        //  Update 메서드에서 계속 실행하지 못하게 실행 중일 땐 false로 세팅
        isCoroutineActive = true;
        Debug.Log("스피드 업 코루틴 실행");

        // SpeedItem 클릭 불가능하게
        //itemCanvasGroup.interactable = false;
        itemCanvasGroup.blocksRaycasts = false;

        // 플레이어 속도 2배 증가
        Debug.Log("속도 2배 증가");
        playerController.moveSpeed = playerMoveSpeed * 2;

        // 3초 대기
        Debug.Log("3초 대기");
        yield return waitForSeconds;

        // 속도증가 아이콘이 다시 ItemList 오브젝트의 자식으로 복귀하여
        // 코루틴이 재실행 되는것을 방지하기 위한 코드

        // 1. ItemList 오브젝트를 참조하는 변수를 만들고
        // 2. ItemSpeed 오브젝트의 부모를 참조한 변수로 설정한다
        Debug.Log("부모 세팅");
        transform.SetParent(findItemList.transform);


        //transform.SetParent(findItemList.transform, false);
        // SetParent메서드의 bool 인자는
        // 부모의 위치, 회전,  스케일을 상속받을지에 대한 조건입니다

        // 플레이어 속도를 다시 원상복구한다
        Debug.Log("속도 복구");
        playerController.moveSpeed = playerMoveSpeed;

        // 쿨타임이 끝나고 다시 클릭이 가능함
        //itemCanvasGroup.interactable = true;
        itemCanvasGroup.blocksRaycasts = true;

        // 다시 아이템의 기능을 이용하기 위하여 true 원상복구
        isCoroutineActive = false;
    }

    IEnumerator PlayerHpUp()
    {
        isCoroutineActive = true;
        //  Update 메서드에서 계속 실행하지 못하게 실행 중일 땐 false로 세팅
        Debug.Log("Hp 회복 코루틴 실행");

        // SpeedItem 클릭 불가능하게
        //itemCanvasGroup.interactable = false;
        itemCanvasGroup.blocksRaycasts = false;

        // 플레이어 +20 hp 증가
        // 플레이어 hp는 Damege 스크립트에 있음
        Debug.Log("hp 20증가");
        playerHp.currHp += 20;
        Debug.Log(playerHp.currHp);

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
        isCoroutineActive = false;
    }
}
