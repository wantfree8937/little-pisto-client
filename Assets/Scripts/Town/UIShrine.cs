using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;

public class UIShrine : MonoBehaviour
{
    [SerializeField] private TMP_Text prevStat;
    [SerializeField] private TMP_Text nextStat;
    [SerializeField] private TMP_Text upGradeSoul;
    [SerializeField] private TMP_Text soulMessage;

    [SerializeField] private Button upgradeBtn;

    private int currentStat;
    private int soulAmount;
    private int upgradeCost;

    void Start()
    {
        // 초기화
        upgradeBtn.onClick.AddListener(OnUpgradeButtonClicked);

        // 마을에 입장할 때 한번 갱신
        UpdateShrineUI();
    }

    // UI 갱신 함수
    public void UpdateShrineUI()
    {
        // 서버나 데이터베이스로부터 현재 상태를 가져온다고 가정
        currentStat = GetCurrentStatFromServer();
        soulAmount = GetSoulAmountFromServer();
        upgradeCost = CalculateUpgradeCost(currentStat);

        prevStat.text = $"Current Stat: {currentStat}";
        nextStat.text = $"Next Stat: {currentStat + 1}";
        upGradeSoul.text = $"Upgrade Cost: {upgradeCost} Souls";
        soulMessage.text = $"Souls: {soulAmount}";
    }

    // 업그레이드 버튼 클릭시 호출
    private void OnUpgradeButtonClicked()
    {
        if (soulAmount >= upgradeCost)
        {
            // 소울을 소비하고 스탯을 업그레이드
            soulAmount -= upgradeCost;
            currentStat += 1;

            // 업데이트된 정보를 서버에 반영
            UpdateStatOnServer(currentStat, soulAmount);

            // UI 갱신
            UpdateShrineUI();
        }
        else
        {
            Debug.Log("Not enough souls to upgrade.");
        }
    }

    // 서버로부터 현재 스탯을 가져오는 가상 함수
    private int GetCurrentStatFromServer()
    {
        // 예시: 서버에서 현재 스탯을 가져오는 코드
        return 10;
    }

    // 서버로부터 현재 소울 양을 가져오는 가상 함수
    private int GetSoulAmountFromServer()
    {
        // 예시: 서버에서 소울 양을 가져오는 코드
        return 100;
    }

    // 스탯 업그레이드에 필요한 소울 양 계산
    private int CalculateUpgradeCost(int currentStat)
    {
        // 예시: 스탯에 따라 업그레이드 비용이 달라짐
        return 10 + (currentStat * 5);
    }

    // 서버에 업데이트된 스탯과 소울 양을 반영하는 가상 함수
    private void UpdateStatOnServer(int updatedStat, int updatedSoulAmount)
    {
        // 예시: 서버에 데이터를 전송하는 코드
        Debug.Log($"Stat updated to: {updatedStat}, Souls remaining: {updatedSoulAmount}");
    }
}
