using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    // 십자선이 원래 위치로 복구하는 속도를 지정하는 커브
    // 최대 확장 크기 지정
    // 마우스 좌클릭 할 때마다 조금씩 확장, 시작 지나면 원래 위치로 돌아옴

    /// <summary>
    /// 회복 속도 조절 커브
    /// </summary>
    public AnimationCurve revoveryCurve;

    /// <summary>
    /// 최대 확장 크기
    /// </summary>
    public float maxExpend = 100.0f;

   
    /// <summary>
    /// 기본적으로 확장되어있는 길이
    /// </summary>
    public float defaultExpend = 10.0f;

    /// <summary>
    /// 현재 확장되어있는길이(defaultExpend에서 얼마나 확장되었는지)
    /// </summary>
    float current = 0.0f;

    /// <summary>
    /// 크로스 이미지
    /// </summary>
    public Sprite[] cross;

    /// <summary>
    /// 복구 되기전에 기다리는 시간
    /// </summary>
    const float recoveryWaitTime = 0.1f;

    /// <summary>
    /// 복구 되는데 걸리는 시간
    /// </summary>
    const float recoveryDuration = 0.5f;

    /// <summary>
    /// recoveryDuration 나누기 연산 최소화
    /// </summary>
    const float divPerCompute = 1 / recoveryDuration;

    /// <summary>
    /// 4방향 크로스헤어의 이미지
    /// </summary>
    Image[] crossImages; 

    /// <summary>
    /// 이동할 방향들
    /// </summary>
    readonly Vector2[] direction = {Vector2.up, Vector2.left, Vector2.right, Vector2.down};

    private void Awake()
    {
        crossImages = GetComponentsInChildren<Image>();

        for(int i = 0; i < crossImages.Length; i++) 
        {
            crossImages[i].sprite = cross[i];
            crossImages[i].rectTransform.anchoredPosition = defaultExpend * direction[i];

        }
        
    }

    /// <summary>
    /// 조준선을 확장시키는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void Expend(float amount)
    {

        current = Mathf.Min(current+amount, maxExpend); // 최대치를 넘지 않게 조절
        for (int i = 0; i < crossImages.Length; i++)
        {
            crossImages[i].rectTransform.anchoredPosition = (current + defaultExpend) * direction[i];   // defaultExpend에서 current만큼 확장

            StopAllCoroutines();                                // 코루틴 중복실행 방지
            StartCoroutine(DelayRecovery(recoveryWaitTime));    // defaultExpend로 복구시키는 코루틴
        }
    }

    /// <summary>
    /// defaultExpend로 복구시키는 코루틴
    /// </summary>
    /// <param name="wait"></param>
    /// <returns></returns>
    IEnumerator DelayRecovery(float wait)
    {
        yield return new WaitForSeconds (wait); // wait만큼 기다리고

        float startExpend = current;            // current를 이용해 현재 확장 정도 기록
        float curveProcess = 0.0f;              // 현재 커브 진행도 (0~1)

        while (curveProcess < 1)                // 커브가 1이 될때 까지 실행
        {
            curveProcess += Time.deltaTime * divPerCompute; // recoveryDuration만큼 커브 배율조정
            current = revoveryCurve.Evaluate(curveProcess) * startExpend;   // current계산
            for (int i = 0; i < crossImages.Length; i++)
            {
                crossImages[i].rectTransform.anchoredPosition = (current+defaultExpend) * direction[i]; // current에 맞게 축소
            }
            yield return null;
        }

        // 정확하게 크로스헤어가 멈추지 않으므로 위치재조정마무리
        for (int i = 0; i < crossImages.Length; i++)
        {
            crossImages[i].rectTransform.anchoredPosition = defaultExpend * direction[i];
        }
        current = 0;    // current 초기화
    }
}
