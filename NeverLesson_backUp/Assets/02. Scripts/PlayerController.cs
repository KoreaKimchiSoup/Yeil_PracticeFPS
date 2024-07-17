using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클래스의 경우는 Serializable(직렬화)라는 속성을 반드시 명시해줘야 유니티 인스펙터에 표시됨
[Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runB;
    public AnimationClip runF;
    public AnimationClip runL;
    public AnimationClip runR;
}

public class PlayerController : MonoBehaviour
{
    private float h = 0f;
    private float v = 0f;
    private float r = 0f; // 회전값 저장할 변수

    Transform tr;
    public float moveSpeed = 10f;    // 이동 속도 계수
    public float rotateSpeed = 100f; // 회전 속도 계수

    // 애니메이션 관련 변수들
    public PlayerAnim playerAnim;
    public Animation anim;

    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        // 현재 재생해야될 애니메이션 클립을 idle로 설정
        anim.clip = playerAnim.runB;
        anim.Play();
    }
    
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        moveDir = moveDir.normalized; // 벡터 정규화

        // Translate(방향 * 속도, 기준좌표)
        // 기준좌표 = Local(Self) / Global
        // 플레이어 입장에서는 Local 좌표계를 기준으로
        // 앞뒤좌우로 움직이도록
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        // Rotate(축 방향 * 속도)
        // ex) 양꼬치 or 회오리감자
        tr.Rotate(Vector3.up * rotateSpeed * r * Time.deltaTime);

        // 애니메이션 전환
        if (v >= 0.1f) // 전진
        {
            anim.CrossFade(playerAnim.runF.name);
        }
        else if (v <= -0.1f) // 후진
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f) // 오른쪽
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f) // 왼쪽
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        else // idle
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
    }
}
