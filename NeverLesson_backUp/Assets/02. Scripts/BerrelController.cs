using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerrelController : MonoBehaviour
{
    public GameObject expEffect; // 폭발 효과 프리팹

    

    int hitCount = 0; // 총알 맞은 횟수
    Rigidbody rb;
    // 찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;
    MeshFilter meshFilter;

    public Texture[] textures;
    MeshRenderer _renderer;

    public float expRadius = 10f;

    // 카메라를 흔드는 스크립트 가져오기
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
                // 폭발 메소드 호출
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // 폭발 이펙트를 드럼통의 위치 (transform.position)
        // 자체 회전값을 가지고 동적 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity); // 동적 생성된 프리팹을 제어하기 위해 변수로 저장

        Destroy(effect, 2f);
        // 드럼통이 잘 날라가도록 하기 위해서 질량값을 1로 설정
        //rb.mass = 1f;
        // 드럼통이 날라갈 방향과 힘을 줌
        // rb.AddForce(Vector3.up * 200f);
        IndirectDamage(transform.position);

        // 난수(랜덤값 발생)
        int index = Random.Range(0, meshes.Length);
        // 위에서 추출한 랜덤 값을 통해서 메쉬 배열에 있는 메쉬 랜덤하게 골라오기
        meshFilter.sharedMesh = meshes[index];

        // 드럼통 폭발의 경우 흔들리는 정도가 총알보다 크므로
        // 매개변수에 좀 더 큰 값을 지정한다
        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));

    }

    void IndirectDamage(Vector3 pos)
    {
        // OverlapSphere(시작위치, 반경, 검출 레이어)
        // 위치로부터 반경 사이의 검출레이어에 해당되는
        // 오브젝트의 충돌체 정보를 모두 가져옴
        // 1  << 8   1번 레이어를 좌측으로 8번 옮겨서 8번 레이어를 킨다는 의미
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8); // (2^8 = 256) 1 << 8 말고도 256으로 명시해줘도됨

        foreach(var coll in colls)
        {
            // 검출된 놈들로부터 리지드 바디 하나씩 뽑아오기
            var _rb = coll.GetComponent<Rigidbody>();
            // 검출된 리지드 바디에 있는 mass 값을 변경함
            _rb.mass = 1;

            // AddExplosionForce(횡 폭발력, 시작위치, 반경, 종 폭발력)
            // 횡 = 가로, 종 = 세로
            _rb.AddExplosionForce(600f, pos, expRadius, 500f);
        }
    }
}
