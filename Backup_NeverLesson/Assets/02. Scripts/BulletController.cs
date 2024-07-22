using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 20; // �Ѿ� ���ݷ�
    public float speed = 2000f; // �Ѿ� �̵� �ӵ�

    Transform tr;
    Rigidbody rb;
    TrailRenderer trail;

    private void OnEnable()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

        // AddForce�� ���� ��ǥ��� ������
        //GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        // AddRelativeForce�� ���� ��ǥ��� ������
        //GetComponent<Rigidbody>().AddRelativeForce(transform.forward * speed);

        rb.AddForce(transform.forward * speed);
        // rb.AddForce(transform.forward * speed); �� ������ǥ��
        // rb.AddForce(Vector3.forward * speed); �� ������ǥ��
    }

    private void OnDisable()
    {
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }
}
