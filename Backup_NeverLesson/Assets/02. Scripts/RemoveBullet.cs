using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            // 스파크 이펙트 생성하는 메소드 호출
            ShowEffect(collision);

            // collider.tag == "" 은 사양을 좀 타기때문에 CompareTag 해도됨
            //Destroy(collision.gameObject);

            // 오브젝트들의 총알을 비활성화 상태로 돌린다
            // 비활성을 해야 오브젝트 풀에서 다시 비활성화 된 총알을 true로 돌릴 수 있다
            collision.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision collision)
    {
        // 충돌 지점이 하나가 아닐 수 있으므로 그 중 하나를 받아온다는 의미
        ContactPoint contactPoint = collision.contacts[0];

        // 법선 벡터가 이루는 각도를 추출함 (법선벡터란 충돌지점의 직각을 이루는 반대방향임)
        // 그러므로 Vector3.forward의 마이너가 들어간다
        // FromToRotation(A, B) = A의 방향을 B 방향으로 돌린다는 의미
        // 즉, 면의 수직이 되는 노말벡터의 방향으로 돌려준다는 의미
        Quaternion rotation = Quaternion.FromToRotation(-Vector3.forward, contactPoint.normal);

        // 충돌 지점에서 스파크 이펙트가 생성됨(Z축 방향이 법선벡터쪽)를 동적생성함
        GameObject effect = Instantiate(sparkEffect, contactPoint.point, rotation);

        // 동적생성된 탄흔 효과의 부모를 설정해주어
        // 같이 움직이도록 함
        effect.transform.SetParent(this.transform);
    }
}
