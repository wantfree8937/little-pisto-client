using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;

public class UIShrine : MonoBehaviour
{
    [SerializeField] private TMP_Text txtCurrStat;
    [SerializeField] private TMP_Text txtNextStat;
    [SerializeField] private TMP_Text txtUpGradeSoul;
    [SerializeField] private TMP_Text txtSoulMessage;

    [SerializeField] private Slider ritualGaugeBar;
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Button lastStageBtn;
    [SerializeField] private GameObject arrowGroup;

    private int currStat;
    private int nextStat;
    private int soulAmount;
    private int upgradeCost = 2;
    private int ritualLevel = 10;

    void Start()
    {
        // 초기화
        upgradeBtn.onClick.AddListener(OnUpgradeButtonClicked);

        soulAmount = TownManager.Instance.soulDisplay.GetSoulCount();
        TownManager.Instance.gaugeBar.gaugeStart = ritualLevel;

        // 슬라이더의 최소값과 최대값 설정
        ritualGaugeBar.minValue = 0; // 최소값
        ritualGaugeBar.maxValue = 50; // 최대값

        // 게이지 바 갱신
        UpdateRitualGauge();

        // 마을에 입장할 때 한번 갱신
        UpdateShrineUI();
    }

    // UI 갱신 함수
    public void UpdateShrineUI()
    {
        txtCurrStat.text = $"{currStat}";
        txtNextStat.text = $"{nextStat}";
        txtUpGradeSoul.text = $"{upgradeCost}";
        txtSoulMessage.text = $"{soulAmount}";

        // 게이지 바 갱신
        UpdateRitualGauge();

        // 버튼 활성화/비활성화
        bool isInteractable = soulAmount >= upgradeCost;
        upgradeBtn.interactable = isInteractable;

        // 버튼의 텍스트 색상 조정
        TMP_Text btnText = upgradeBtn.GetComponentInChildren<TMP_Text>();
        if (btnText != null)
        {
            btnText.color = isInteractable ? UnityEngine.Color.white : UnityEngine.Color.gray;
        }
    }

    // 업그레이드 버튼 클릭시 호출
    private void OnUpgradeButtonClicked()
    {
        if (soulAmount >= upgradeCost)
        {
            // 소울을 사용하여 스탯을 업그레이드
            soulAmount -= upgradeCost;

            ritualLevel++;

            /* 

             현재 스탯, 다음 스탯 바꾸는 부분

             */

            // 소울 갱신
            TownManager.Instance.soulDisplay.SetSouls(soulAmount);

            /* 업데이트된 정보를 서버로 전송
            
            이곳에 작성
            
             */

            // UI 갱신은 받은 패킷핸들러에서 사용
            UpdateShrineUI();
        }
        else
        {
            Debug.Log("영혼이 부족합니다");
        }
    }

    // 게이지 바 갱신 함수
    private void UpdateRitualGauge()
    {
        if (ritualGaugeBar != null)
        {
            // ritualLevel을 슬라이더의 값으로 설정
            ritualGaugeBar.value = ritualLevel;

            // 게이지가 최대값에 도달하면 라스트 스테이지 버튼을 활성화
            if (ritualGaugeBar.value >= ritualGaugeBar.maxValue)
            {
                lastStageBtn.gameObject.SetActive(true);
                arrowGroup.gameObject.SetActive(true);
            }
            else
            {
                lastStageBtn.gameObject.SetActive(false);
                arrowGroup.gameObject.SetActive(false);
            }
        }
    }

    // 현재 스탯 가져오기
    private int GetCurrentStatFromServer()
    {
        return currStat;
    }

    // 다음 스탯 가져오기
    private int GetNextStatFromServer()
    {
        return nextStat;
    }

    // 소울 코스트 가져오기
    private int GetUpgradeCostFromServer()
    {
        return upgradeCost;
    }

    // 현재 소울 가져오기
    private int GetSoulAmountFromServer()
    {
        return soulAmount;
    }

    // 서버에서 가져온 정보를 업데이트 하는 함수
    private void UpdateStatOnServer()
    {

    }
}
