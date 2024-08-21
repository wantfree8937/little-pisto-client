using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    [SerializeField] private Slider slider; // 슬라이더 컴포넌트

    public int gaugeStart = 0;

    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>(); // 슬라이더 컴포넌트를 가져옴
        }

        // 슬라이더의 최소값과 최대값 설정
        slider.minValue = 0; // 최소값
        slider.maxValue = 50; // 최대값

        slider.interactable = false; // 슬라이더를 클릭할 수 없게 설정

        // 초기 슬라이더 값 설정
        slider.value = gaugeStart;
    }

    // 게이지 값을 설정하는 함수
    public void SetGauge(int value)
    {
        // 값을 0~50 사이로 제한하고 슬라이더에 설정
        slider.value = Mathf.Clamp(value, (int)slider.minValue, (int)slider.maxValue);
    }

    // 게이지 값을 증가시키는 함수
    public void IncreaseGauge(int amount)
    {
        SetGauge((int)slider.value + amount);
    }

    // 게이지 값을 감소시키는 함수 *예비용
    public void DecreaseGauge(int amount)
    {
        SetGauge((int)slider.value - amount);
    }
}
