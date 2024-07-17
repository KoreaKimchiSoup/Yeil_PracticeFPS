using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ŭ������ ���� Serializable(����ȭ)��� �Ӽ��� �ݵ�� �������� ����Ƽ �ν����Ϳ� ǥ�õ�
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
    private float r = 0f; // ȸ���� ������ ����

    Transform tr;
    public float moveSpeed = 10f;    // �̵� �ӵ� ���
    public float rotateSpeed = 100f; // ȸ�� �ӵ� ���

    // �ִϸ��̼� ���� ������
    public PlayerAnim playerAnim;
    public Animation anim;

    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        // ���� ����ؾߵ� �ִϸ��̼� Ŭ���� idle�� ����
        anim.clip = playerAnim.runB;
        anim.Play();
    }
    
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        moveDir = moveDir.normalized; // ���� ����ȭ

        // Translate(���� * �ӵ�, ������ǥ)
        // ������ǥ = Local(Self) / Global
        // �÷��̾� ���忡���� Local ��ǥ�踦 ��������
        // �յ��¿�� �����̵���
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        // Rotate(�� ���� * �ӵ�)
        // ex) �粿ġ or ȸ��������
        tr.Rotate(Vector3.up * rotateSpeed * r * Time.deltaTime);

        // �ִϸ��̼� ��ȯ
        if (v >= 0.1f) // ����
        {
            anim.CrossFade(playerAnim.runF.name);
        }
        else if (v <= -0.1f) // ����
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f) // ������
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f) // ����
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        else // idle
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
    }
}
