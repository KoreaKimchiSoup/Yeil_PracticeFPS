using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ����ü�� �޸��� ���� ������ �Ҵ��
// ������ Ŭ�������� ����
// ���� ��� �� Ȱ���� �ʿ��� ��� ���� ����
[Serializable]
public struct PlayerSfx // ����ü
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireController : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN
    }
    // ���� ������� ���� Ȯ�ο� ����
    public WeaponType currentWeapon = WeaponType.RIFLE;

    public GameObject bulletPrefab;
    public Transform firePos; // FirePos�� ��ġ��
    public ParticleSystem cartridge; // ź�� ��ƼŬ�ý��ۿ� ����
    private ParticleSystem muzzleFlash;

    public PlayerSfx playerSfx;
    private AudioSource _audio;

    // ī�޶� ���� ��ũ��Ʈ�� ��������
    Shake shake;

    public Image magazineImg;
    public Text magazineText;

    public int maxBullet = 10;
    public int reamainingBullet = 10;
    public float reloadTime = 2f;
    bool isReloading = false;

    public Sprite[] weaponIcons;
    public Image weaponImage;

    [Header("�ڵ����� ����")]
    bool isFire = false;
    float nextFire;
    public float fireRate = 0.1f;

    int enemyLayer;
    int obstacleLayer;
    int layerMask;

    // ���̾�� �����ؼ� ����
    // ���� ENEMY ���̾�� 12���� �����Ƿ� 2 ^ 12 = 4096�� ���� ���´�
    // �ش� ���� �˰� ������ enemyLayer = 4096 ���� �ٷ� �����ص� �Ǳ���

    private void Start()
    {
        // firePos�� �ڽĿ�����Ʈ �߿��� ParticleSystem ������Ʈ ȹ��
        // ����Ƽ�� ��� ������Ʈ�� ��뼺�� ���ϹǷ�
        // ���� ��ũ��Ʈ�� ��ġ�� ������Ʈ�� ��ġ�� �ſ� �߿��ϴ�
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        // enemyLayer = 4906; // ���̾� 12��°�̹Ƿ� �����ϴ�
        // NameToLayer �Լ��� ���̾��� index�� �����Ѵ�
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        // �� ���̾ ������
        // ���̾ 2�� �̻� ������ ���� | (or ��Ʈ ������)�� �̿��Ѵ�
        layerMask = 1 << enemyLayer | 1 << obstacleLayer;
        // layerMask�� ��Ʈ
        // 10 0000 0000
        // 01 0000 0000
        // --------------| (or) ������
        // 11 0000 0000 (8,192 + 4,096 = 12,288)

    }

    private void Update()
    {
        // ����ĳ��Ʈ ����׿�
        Debug.DrawRay(firePos.position, firePos.forward * 20f, Color.red);

        // UI ���� Ŭ�� �Ǵ� ��ġ �ϰԵǸ� True �ƴϸ� False
        // ��, �̺�Ʈ �ý����� �̿��ϴ� ��ư ���� �͵���
        // Ŭ���ϰԵǸ� Update ������ �׻� return�� �ȴ�
        // �׸��� ���� BloodScreen ȭ�� ��ü�� ���� �ֱ� ������
        // ���� if�� ������ �׻� true�� ������ �����Ѵ�
        // �ذ����� BloodScreen�� Image ������Ʈ��
        // Raycast Target ������ üũ �����Ѵ�
        // Raycast Target �̶� ������ �ִ� Canvas��
        // ��ȣ�ۿ��� �Ұ��� �������� ���� bool �����̴�
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        RaycastHit hit;

        // Raycast�� �浹 ������ �Ǵ��ϰ� ���� �浹 ��ü ������
        // RaycastHit�� ���޵ȴ�
        // �� �� out���� ��µǴ� ���� ���޹ޱ� ���� ������ �̸� �����Ѵ�
        // ���� �߻� ��ġ, ���� �߻� ����, �浹�� ��ü ������ ��ȯ���� ����, ���� ��Ÿ�, ���� ���̾�
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))  
        {
            // enemyLayer�� ���ؼ� ������ �Ǹ�
            isFire = hit.collider.CompareTag("ENEMY");
        }
        else
        {
            isFire = false;
        }

        if (!isReloading && isFire)
        {
            if (Time.time > nextFire)
            {
                reamainingBullet--;
                Fire();

                if (reamainingBullet == 0)
                {
                    StartCoroutine(Reloading());
                }

                nextFire = Time.time + fireRate;
            }
        }


        // GetMouseButton�� ���콺 ������ �ִ� ���� ���� �߻�
        // GetMouseButtonDown�� ������ ���� 1���� �߻�
        // GetMouseButtonUp�� ���� ���� 1���� �߻�
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            // ���� �����ϰ� �ִ� �Ѿ��� �ϳ��� ����
            reamainingBullet--;
            // ���� �޼ҵ� ȣ��
            Fire();

            if (reamainingBullet == 0)
            {
                // ������ �ڷ�ƾ ȣ��
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());
        // �Ѿ� �������� �ѱ��� ��ġ�� ȸ������ ������ ���� ������
        //Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        // ���� ���������� ������� �ʰ� ������ƮǮ�� ����Ѵ�

        // �̱��� ����� Ȱ���Ͽ� ������ƮǮ�� ��� �ִ� �Ѿ��� �����´�
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            // firePos�� �����ǰ�
            _bullet.transform.position = firePos.position;
            // firePos�� ȸ������ �����Ѵ�
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }


        // �Ѿ��� �߻�Ǵ� ������ ź�ǰ� �������� ��ƼŬ ���
        cartridge.Play();
        muzzleFlash.Play();
        FireSfx();

        magazineImg.fillAmount = (float)reamainingBullet / (float)maxBullet;
        // ���� �Ѿ� �� �ؽ�Ʈ ���ſ� �Լ� ȣ��
        UpdateBulletText();
    }

    void FireSfx()
    {
        // ���� ��� �ִ� ������ enum ���� int �� ��ȯ�ؼ�
        // ����ϰ��� �ϴ� ������ ����� Ŭ���� ������
        var _sfx = playerSfx.fire[(int)currentWeapon];
        // ������ ������ 1(100%) �������� ���
        _audio.PlayOneShot(_sfx, 1f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currentWeapon], 1f);

        // ������ ������ ���� + 0.3�� ��ŭ �����
        yield return new WaitForSeconds(playerSfx.reload[(int)currentWeapon].length + 0.3f);

        isReloading = false;
        magazineImg.fillAmount = 1f;
        reamainingBullet = maxBullet;
        // ���� �Ѿ� �� �ؽ�Ʈ ���ſ� �Լ� ȣ��
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        string str = $"<color=#ff0000>{reamainingBullet}</color>/{maxBullet}";
        magazineText.text = str;
    }

    public void OnChangeWeapon()
    {
        currentWeapon++;
        currentWeapon = (WeaponType)((int)currentWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currentWeapon];
    }
}
