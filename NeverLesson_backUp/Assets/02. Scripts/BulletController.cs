using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 20; // 총알 공격력
    public float speed = 2000f; // 총알 이동 속도

    Transform tr;
    Rigidbody rb;
    TrailRenderer trail;

    private void OnEnable()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

        // AddForce는 월드 좌표계로 전진함
        //GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        // AddRelativeForce는 로컬 좌표계로 전진함
        //GetComponent<Rigidbody>().AddRelativeForce(transform.forward * speed);

        rb.AddForce(transform.forward * speed);
        // rb.AddForce(transform.forward * speed); 는 로컬좌표계
        // rb.AddForce(Vector3.forward * speed); 는 월드좌표계
    }

    private void OnDisable()
    {
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }
}
