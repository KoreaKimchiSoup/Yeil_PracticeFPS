using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    // ����ũ  ȿ���� �� ī�޶��� ��ġ
    public Transform shakeCamera;

    public bool shakeRotete = false;

    Vector3 originPos;
    Quaternion originRotate;

    void Start()
    {
        // ���� �� ������ ��ġ���� ȸ���� �����صα�
        originPos = shakeCamera.localPosition;
        originRotate = shakeCamera.localRotation;
    }
    //                                                        ��鸮�� �ð�             ��鸲�� ũ��                       ��鸲�� ȸ��
    //                                                        0.05�ʵ��� ��鸮�� ��鸲�� ũ��� ȸ������ �ǹ��Ѵ�
    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRotation = 0.1f)
    {
        // passTime�� ������ ���ӽð��� �ǹ���
        // ��, duration�� ũ��� �����ϴ� �ð��� �ǹ���
        float passTime = 0f;

        // duration ���� ���� ���ؼ� while ���
        while (passTime < duration) 
        {
            // �������� 1�� ������ �� �ȿ���
            // ������ 3���� ��ǥ(x, y, z)�� ������
            Vector3 shakePos = Random.insideUnitSphere;

            // ������ ���� ������ġ�� �Ű������� ���Ͽ� ����
            //                     shakePos(0.5, 0.5, 0.5) * 0.03
            shakeCamera.localPosition = shakePos * magnitudePos;

            // �ұ�Ģ�� ȸ�� ��� ����
            // �ϴ��� ������� ����
            if (shakeRotete)
            {
                // ������ �޸������� �Լ��μ�
                // �ұ�Ģ������ �ұ�Ģ�� �ϰ��ǰ� �Ͽ�
                // ���� �� ����� ���� ���δ�
                float z = Mathf.PerlinNoise(Time.time * magnitudeRotation, 0f);
                Vector3 shakeRotate = new Vector3(0, 0, z);

                // ī�޶��� ȸ���� shakeRotate�� ����
                shakeCamera.localRotation = Quaternion.Euler(shakeRotate);
            }

            // �����ð��� ������Ŵ
            // �׷����� ������ �ð��� ���Ѵ�
            passTime += Time.deltaTime;

            // ���� �����ӱ��� ���
            yield return null;
        }

        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRotate;
    }
}
