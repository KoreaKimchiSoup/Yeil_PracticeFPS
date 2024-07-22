using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    // 쉐이크  효과를 줄 카메라의 위치
    public Transform shakeCamera;

    public bool shakeRotete = false;

    Vector3 originPos;
    Quaternion originRotate;

    void Start()
    {
        // 흔들기 전 원래의 위치값과 회전값 저장해두기
        originPos = shakeCamera.localPosition;
        originRotate = shakeCamera.localRotation;
    }
    //                                                        흔들리는 시간             흔들림의 크기                       흔들림의 회전
    //                                                        0.05초동안 흔들리며 흔들림의 크기와 회전값을 의미한다
    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRotation = 0.1f)
    {
        // passTime은 진동의 지속시간을 의미함
        // 즉, duration의 크기는 진동하는 시간을 의미함
        float passTime = 0f;

        // duration 동안 흔들기 위해서 while 사용
        while (passTime < duration) 
        {
            // 반지름이 1인 구형의 공 안에서
            // 랜덤한 3개의 좌표(x, y, z)를 추출함
            Vector3 shakePos = Random.insideUnitSphere;

            // 위에서 뽑은 랜덤위치의 매개변수를 통하여 흔든다
            //                     shakePos(0.5, 0.5, 0.5) * 0.03
            shakeCamera.localPosition = shakePos * magnitudePos;

            // 불규칙한 회전 사용 유무
            // 일단은 사용하지 않음
            if (shakeRotete)
            {
                // 수학의 펄린노이즈 함수로서
                // 불규칙하지만 불규칙이 일관되게 하여
                // 랜덤 맵 생성등에 많이 쓰인다
                float z = Mathf.PerlinNoise(Time.time * magnitudeRotation, 0f);
                Vector3 shakeRotate = new Vector3(0, 0, z);

                // 카메라의 회전에 shakeRotate값 전달
                shakeCamera.localRotation = Quaternion.Euler(shakeRotate);
            }

            // 진동시간을 누적시킴
            // 그로인해 진동의 시간을 정한다
            passTime += Time.deltaTime;

            // 다음 프레임까지 대기
            yield return null;
        }

        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRotate;
    }
}
