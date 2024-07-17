using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    const string BULLETTAG = "BULLET";
    const string ENEMYTAG = "ENEMY";
    float initHp = 100f;
    public float currHp;

    // 델리게이트 및 이벤트 생성
    public delegate void PlayerDieHandler();
    // 델리게이트에서 파생된 이벤트
    public static event PlayerDieHandler PlayerDieEvent; // 플레이어가 죽었을 때 생기는 이벤트

    public Image bloodScreen; // bloodScreen을 컨트롤 할 변수

    public Image hpBar;
    readonly Color initColor = new Vector4(0f, 1f, 0f, 1f);
    Color currColor;

    void Start()
    {
        // 게임을 시작하면 현재 hp를 초기화한다
        currHp = initHp;

        hpBar.color = initColor;
        currColor = initColor;
    }

    private void OnTriggerEnter(Collider other) // 플레이어에게 닿는 총알을 컨트롤 하는 OnTriggerEnter
    {
        if (other.CompareTag(BULLETTAG)) // 플레이어에게 닿는 오브젝트중 태그가 BULLETTAG 일 때
        {
            Destroy(other.gameObject); // 해당 오브젝트를 파괴한다

            // 출혈화면 효과 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());

            currHp -= 5f; // DamageScript 스크립가 들어가는 오브젝트에 Hp를 5 깎는다
            DisplayHpBar();

            // print(currentHp); // 현재 남은  Hp를 출력

            if (currHp <= 0f) // Hp가 0보다 작거나 같다면
            {
                PlayerDie(); // 플레이어 죽음 메서드 호출
            }
        }
    }

    IEnumerator ShowBloodScreen()
    {
        // 빨간색의 투명도는 2 ~ 30퍼 정도로 랜덤하게 설정한다 (1의 의미는 100%를 의미함)
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        // Blood Screen이 생기고 난 후 0.1초 동안 유지됨
        yield return new WaitForSeconds(0.1f);
        // 다시 색상도 0, 투명도 0
        bloodScreen.color = Color.clear;
    }

    void PlayerDie()
    {
        //print("플레이어 사망");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(ENEMYTAG);
        //foreach(GameObject enemy in enemies)
        //{
        //    // SendMessage("호출할 메서드 이름", 응답 모드);
        //    // SendMessage 메서드는 특정 스크립트를 지정하는 것이 아니라
        //    // 오브젝트에 포함된 모든 스크립트를 하나씩 검색하여
        //    // 해당 하는 이름이 존재하면 메서드를 호출 하는 방식이다
        //    enemy.SendMessage("E_PlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        // 이벤트 호출
        PlayerDieEvent();
        // 싱글톤 패턴을 이용하여 손쉽게 접근한다.
        GameManager.instance.isGameOver = true;
    }

    void DisplayHpBar()
    {
        // 현재 체력이 전체 체력중에 몇 퍼센트인지
        float currHpRate = currHp / initHp;

        if (currHpRate > 0.5f) // 체력이 50프로 보다 크다면
        {
            // 빨강색을 증가 = 녹색 + 빨강 = 노랑
            // 녹색 -> 노랑
            currColor.r = (1 - currHpRate) * 2f;
        }
        else // 체력이 50프로 이하일 때
        {
            // 녹색을 감소시킴
            // 노랑 -> 빨강
            currColor.g = currHpRate * 2f;
        }

        hpBar.color = currColor; // 체력바 색상 적용
        hpBar.fillAmount = currHpRate;
    }
}
