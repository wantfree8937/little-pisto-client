using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImagePulse : MonoBehaviour
{
    [SerializeField] private Image[] targetImages;  // 대상 이미지 배열
    [SerializeField] private float pulseDuration = 1.0f;  // 한 번의 펄스가 걸리는 시간, 증가시켜서 느리게 설정
    [SerializeField] private float maxScale = 1.2f;  // 최대 크기
    [SerializeField] private float minScale = 0.8f;  // 최소 크기

    private bool isPulsing = false;

    void Start()
    {
        StartPulsing();
    }

    void OnEnable()
    {
        StartPulsing();
    }

    void OnDisable()
    {
        StopPulsing();
    }

    public void StartPulsing()
    {
        if (!isPulsing)
        {
            isPulsing = true;
            StartCoroutine(PulseCoroutine());
        }
    }

    public void StopPulsing()
    {
        isPulsing = false;
    }

    private IEnumerator PulseCoroutine()
    {
        while (isPulsing)
        {
            // 커졌다가 작아지는 애니메이션
            yield return StartCoroutine(ScaleTo(maxScale, pulseDuration));
            yield return StartCoroutine(ScaleTo(minScale, pulseDuration));
        }
    }

    private IEnumerator ScaleTo(float targetScale, float duration)
    {
        float time = 0;

        // 초기 크기 가져오기
        float initialScale = targetImages[0].rectTransform.localScale.x;

        while (time < duration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(initialScale, targetScale, time / duration);

            // 배열의 모든 이미지에 대해 크기 변경
            foreach (Image img in targetImages)
            {
                if (img != null)
                {
                    img.rectTransform.localScale = new Vector3(scale, scale, 1f);
                }
            }
            yield return null;
        }

        // 최종 크기 설정
        foreach (Image img in targetImages)
        {
            if (img != null)
            {
                img.rectTransform.localScale = new Vector3(targetScale, targetScale, 1f);
            }
        }
    }
}
