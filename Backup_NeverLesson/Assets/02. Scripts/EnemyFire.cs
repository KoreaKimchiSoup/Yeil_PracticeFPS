using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    AudioSource audio; // ����� �ҽ��� �����ϴ� ����
    Animator animator; // �ִϸ����͸� ��Ʈ���ϱ� ���� ����
    Transform playerTr; // Player�� ��ġ���� ��Ʈ�� �Ǵ� �����ϱ� ���� ����
    Transform enemyTr; // Enemy�� ��ġ���� ��Ʈ�� �Ǵ� �����ϱ� ���� ����

    // hashFire�� Animator�� �ִ� �Ķ���� ���� �����ϴ� �뵵�̸� �̸��� 'Fire' �� �Ķ���� ���� hashFire�� �����Ѵ�
    readonly int hashFire = Animator.StringToHash("Fire");
    // hashReload�� Animator�� �ִ� �Ķ���� ���� �����ϴ� �뵵�̸� �̸��� 'Reload' �� �Ķ���� ���� hashReload�� �����Ѵ�
    readonly int hashReload = Animator.StringToHash("Reload"); // �ִϸ����Ϳ��� ������ trigger�� Reload

    // �ڵ� ���� ���� ����
    [Header("�ڵ�����")]
    public float nextFire = 0f;
    readonly float fireRate = 0.1f; // �߻� ���� ������ ���� ����
    readonly float damping = 10f;  // 

    [Header("������")]
    int currentBullet = 10; // ������ ���۵Ǹ� Enemy�� ���� �Ѿ��� �ʱ�ȭ�� 
    readonly float reloadTime = 2f; // �������� �ɸ��� �ð� 2��
    readonly int maxBullet = 10; // Enemy�� ������ �ִ� ���� ��ź ��
    bool isReload = false;            // Reload�� �ϰų� ���� �ʱ� ���� bool ����
    WaitForSeconds wsReload;     // �ڷ�ƾ�� ���ð��� ��Ʈ�� �ϱ� ���� ����

    public bool isFire = false; // ���� �߻��ϰ� �ִ��� üũ�ϴ� bool ����
    public AudioClip fireSfx;  // ���� �߻��� ���� ����
    public AudioClip reloadSfx; // ������ ����

    // �Ѿ� �߻� ���� ����
    public GameObject bullet; // Bullet �������� ��Ʈ���ϱ� ���� ����
    public Transform firePos;    // �Ѿ��� �߻�Ǵ� ������ �����ϱ� ���� ����

    // muzzleFlash�� �̹����� �Ѿ��� �߻�� ���� �ѱ� ���� MeshRenderer ���� ����
    // �翬�ϰԵ� Enemy�� Player�� ó������ ���� �ƴ϶�� ���־� ��
    // �� ��, Player�� ���� ���� �߻��� ���� Ŵ
    public MeshRenderer muzzleFlash;


    void Start()
    {
        //�±װ� "PLAYER"�� ���ӿ�����Ʈ�� ã�� �ش� ������Ʈ�� Transform(��ġ��, ȸ����, ������)�� �����ϱ� ���� ����
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();

        // enemyTr(Transform)�� Transform ������Ʈ�� ������
        enemyTr = GetComponent<Transform>();

        // animator(Animator)�� Animator ������Ʈ�� ������
        animator = GetComponent<Animator>();

        // audio(AudioSource)�� AudioSource ������Ʈ�� ������
        audio = GetComponent<AudioSource>();

        wsReload = new WaitForSeconds(reloadTime); // �ڷ�ƾ�� ������� �ݳ��ϱ���� ��ٸ��� �ð�

        muzzleFlash.enabled = false; // ������ �����ϸ� �߻�Ǹ� �ȵǱ� ������ false
    }

    void Update()
    {
        // isFire�� �󵵼��� �� ���� ������ ��������� ���� isReload�� true�� �� ���� isFire�� üũ���� �ʱ� ���� (�ҷ��� ���� ���)
        if(!isReload && isFire) // �������� �ƴ� ��, ������ �� ��
        {
            // Time.time(�ð�)�� nextFire���� Ŭ ��
            if(Time.time > nextFire)
            {
                // �Ѿ� �߻� �Լ� ȣ��
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0f, 0.3f);
            }
            // A - B �� B�� A�� �ٶ󺻴�
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        // �ѱ�ȭ�� �ڷ�ƾ �Լ� ȣ��
        StartCoroutine(ShowMuzzleFlash());

        animator.SetTrigger(hashFire); // Ʈ���Ŵ� true, false���� ���� �ٷ� ����ϸ��
        audio.PlayOneShot(fireSfx, 1f); // fireSfx�� ������ 1�� ���� (1�� �ִ밪)

        GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3f); // 3���� �Ѿ� ����

        // �Ѿ��� �߻��� �� ���� �Ѿ��� �ϳ��� ���ҽ�Ŵ
        currentBullet--;

        // ���� �Ѿ� ������ ���Ͽ� �������� ������ �Ǵ���
        // = ������ ������ ���Ͽ� ���ϰ�� true �Ǵ� false;
        // �Ѿ��� �߻��� �� ���� ���� ������ �Ʒ��� ����
        // 9/10, 8/10, ~ ,1/10, 0/10 ( 10, 9, ~ ,2, 1 )
        // 0/10�� �� ��, isReload�� 0�� �Ǹ� bool ���� 
        isReload = (currentBullet % maxBullet == 0);

        // ��, currentBullet % maxBullet �� ���� 0�� �ƴ� ��쿣 ���� false
        //if (currentBullet % maxBullet == 0) // �ش� �ڵ�� �� isReload ���ǽİ� ���� �ڵ���
        //{
        //    isReload = false;
        //}
        //else
        //{
        //    isReload = true;
        //}


        if (isReload)
        {
            // ������ �ڷ�ƾ �Լ� ȣ����
            StartCoroutine(Reload());
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        // �߻��� �� ���̴� �ѱ�ȭ�� ����Ʈ�� �ʹ� ���� ������Ƿ� ����̳��� �ð��� �ֱ� ���ϸ� ���� �ڷ�ƾ�� �����
        muzzleFlash.enabled = true;

        // ȸ������ (0,0,1) * (0 ~ 360)�� �����ϰ�
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        // ������ ���� muzzleFlash�� localRotation(����ȸ����)�� ������
        muzzleFlash.transform.localRotation = rot;

        // muzzleFlash�� Scale ���� xyz ��� �����ϰ� 1 ~ 2�� ��ŭ �����ϰ� ����
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);

        // �ؽ�ó ������ �����ϱ�
        // Random Range�� ���ؼ� 0 �Ǵ� 1���� �����µ�
        // ���⿡ 5�� ���ؼ� 0 �Ǵ� 0.5 ���� �������� ���
        // ����� Random.Range(x, y) �� x���� y�������̴�
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;

        // muzzleFlash�� �����ϴ� Material�� offset ���� �����ϴµ�,
        // ��Ȯ���� Material�� �����ϴ� Shader�� ���� �����ϹǷ�  Shader�� ���� ������
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        // ���ð��� 0.05 ~ 0.2�� ���� �����ϰ� ������
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        // ��� ������ ���� �� muzzleFlash�� �� (muzzleFlash�� Mesh Renderer�� ���ٴ� �ǹ�)
        muzzleFlash.enabled = false;

    }

    IEnumerator Reload()
    {
        // �̹� hashReload = Animator.StringToHash("Reload"); �� ���������� �����ǿ� �����Ƿ�,
        // Animator�� �Ķ���� ���� Reload�� ã�Ƽ� Ʈ���Ÿ� true ������
        animator.SetTrigger(hashReload);
        // �������� �� �� ���� �Ҹ��� �� ���� �����
        audio.PlayOneShot(reloadSfx, 1f);

        // ������ �ִϸ��̼��� ��ٷ��ֱ� ����
        // ���� �ڵ尡 ������ �������� ���൵�� ���߿� �Ѿ� ���� ������
        yield return wsReload;

        // �������� �����ٸ� ���� �Ѿ˿� maxBullet�� �����Ͽ� �Ѿ��� ������
        currentBullet = maxBullet;

        // �������� �����ٸ� isReload�� false�� �ٲ�
        // �׷��� �Ѿ��� �߻�� �� ���� Fire �Լ��� ����Ǵµ�
        // Fire �Լ��� Reload �ڷ�ƾ�� �����Ű�� ������ �־�
        // �� �� �߻��� ������ �������� ���� �ʱ� ����
        // �׸��� Reload �ڷ�ƾ�� ���� �������� �Ѿ��� �� ��� ����Ǵ���
        isReload = false;
    }
}
