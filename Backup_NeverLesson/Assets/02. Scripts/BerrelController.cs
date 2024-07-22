using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerrelController : MonoBehaviour
{
    public GameObject expEffect; // ���� ȿ�� ������

    

    int hitCount = 0; // �Ѿ� ���� Ƚ��
    Rigidbody rb;
    // ��׷��� �巳���� �޽��� ������ �迭
    public Mesh[] meshes;
    MeshFilter meshFilter;

    public Texture[] textures;
    MeshRenderer _renderer;

    public float expRadius = 10f;

    // ī�޶� ���� ��ũ��Ʈ ��������
    Shake shake;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            hitCount++;
            if (hitCount == 3)
            {
                // ���� �޼ҵ� ȣ��
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // ���� ����Ʈ�� �巳���� ��ġ (transform.position)
        // ��ü ȸ������ ������ ���� ����
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity); // ���� ������ �������� �����ϱ� ���� ������ ����

        Destroy(effect, 2f);
        // �巳���� �� ���󰡵��� �ϱ� ���ؼ� �������� 1�� ����
        //rb.mass = 1f;
        // �巳���� ���� ����� ���� ��
        // rb.AddForce(Vector3.up * 200f);
        IndirectDamage(transform.position);

        // ����(������ �߻�)
        int index = Random.Range(0, meshes.Length);
        // ������ ������ ���� ���� ���ؼ� �޽� �迭�� �ִ� �޽� �����ϰ� ������
        meshFilter.sharedMesh = meshes[index];

        // �巳�� ������ ��� ��鸮�� ������ �Ѿ˺��� ũ�Ƿ�
        // �Ű������� �� �� ū ���� �����Ѵ�
        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));

    }

    void IndirectDamage(Vector3 pos)
    {
        // OverlapSphere(������ġ, �ݰ�, ���� ���̾�)
        // ��ġ�κ��� �ݰ� ������ ���ⷹ�̾ �ش�Ǵ�
        // ������Ʈ�� �浹ü ������ ��� ������
        // 1  << 8   1�� ���̾ �������� 8�� �Űܼ� 8�� ���̾ Ų�ٴ� �ǹ�
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8); // (2^8 = 256) 1 << 8 ���� 256���� ������൵��

        foreach(var coll in colls)
        {
            // ����� ���κ��� ������ �ٵ� �ϳ��� �̾ƿ���
            var _rb = coll.GetComponent<Rigidbody>();
            // ����� ������ �ٵ� �ִ� mass ���� ������
            _rb.mass = 1;

            // AddExplosionForce(Ⱦ ���߷�, ������ġ, �ݰ�, �� ���߷�)
            // Ⱦ = ����, �� = ����
            _rb.AddExplosionForce(600f, pos, expRadius, 500f);
        }
    }
}
