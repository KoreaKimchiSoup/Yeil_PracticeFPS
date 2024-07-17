using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    const string bulletTag = "BULLET"; // 총알의 태그

    float initHp = 100f; // 초기 피통
    float hp = 100f;
    GameObject bloodEffect;

    // 생명 게이지 프리팹을 저장할 변수
    public GameObject hpBarPrefab;
    // 생명 게이지의 위치를 보정할 오프셋
    public Vector3 hpBarOffset = new Vector3(0f, 2.2f, 0f);
    // 부모가 될 Canvas 객체
    Canvas uiCanvas;
    // 생명 수치에 따라 fillAmount 속성을 변경할 Image
    Image hpBarImage;

    void Start()
    {

        // Resources.Load<GameObject>("파일경로");
        // "Resources" 폴더부터 시작하여 제네릭을 통해 Load 함
        bloodEffect = Resources.Load<GameObject>("Blood");
        SetHpBar();
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>();

        // hpBar를 동적생성하면서 캔버스의 자식으로 넣어준다
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);

        // hpBarImage = 빨간 hpBar
        // 부모 기준 Image가 들어간것을 기준으로 인덱싱이 이루어짐 
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        // 체력바가 따라가야할 대상과 오프셋 설정
        var enemyHpBar = hpBar.GetComponent<EnemyHpBar>();
        enemyHpBar.targetTr = this.gameObject.transform;
        enemyHpBar.offset = hpBarOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(bulletTag))
        {
            // 혈흔효과 생성 함수 호출
            ShowBloodEffect(collision);

            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);

            // BulletController에 작성한 damage 변수의 값을 가져와서
            // 체력을 빼줌
            hp -= collision.gameObject.GetComponent<BulletController>().damage;
            // 체력바의 빨간색 게이지 줄이기
            hpBarImage.fillAmount = hp / initHp;

            // hp가 0보다 작거나 같을때
            if (hp <= 0f)
            {// EnemyAI에 존재하는 State(상태)를 DIE로 변경함
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                // 게임 매니저의 킬 카운트 증가 함수 호출
                GameManager.instance.IncresementKillCount();
                // 사망 후 콜라이더 비활성화
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }

        void ShowBloodEffect(Collision collision)
        {
            // 충돌지점의 위치 포인트를 하나 가져옴
            Vector3 pos = collision.contacts[0].point;

            // 충돌 지점의 법선벡터 구하기
            // 충돌의 첫 번째 점의 법선을 인쇄합니다.
            //Debug.Log("첫 번째 점의 법선: " + collision.contacts[0].normal);
            Vector3 _nomal = collision.contacts[0].normal;

            // 총알의 탄두와 마주보는 방향값 구하기
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _nomal);

            // 혈흔 효과를 
            GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
            Destroy(blood, 1f);
        }
    }
}
