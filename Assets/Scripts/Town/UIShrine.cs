using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;

public class UIShrine : MonoBehaviour
{
    // StatInfo 구조체 정의
    [Serializable]
    public struct StatInfo
    {
        public int Level;
        public float MaxHp;
        public float Atk;
        public float Magic;
    }

    [SerializeField] private TMP_Text txtCurrStat;
    [SerializeField] private TMP_Text txtNextStat;
    [SerializeField] private TMP_Text txtUpGradeSoul;
    [SerializeField] private TMP_Text txtSoulMessage;

    [SerializeField] private Slider ritualGaugeBar;
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Button lastStageBtn;
    [SerializeField] private GameObject arrowGroup;

    private StatInfo currStat;  // 현재 스탯
    private StatInfo nextStat;  // 다음 스탯
    private int soulAmount;
    private int upgradeCost;
    private int ritualLevel;
    private ImagePulse arrowGroupPulse;
    public bool finalStageCheck;

    void Start()
    {
        // 초기화
        upgradeBtn.onClick.AddListener(OnUpgradeButtonClicked);
        lastStageBtn.onClick.AddListener(OnLastStageButtonClicked);

        TownManager.Instance.gaugeBar.gaugeStart = ritualLevel;

        // 슬라이더의 최소값과 최대값 설정
        ritualGaugeBar.minValue = 0; // 최소값
        ritualGaugeBar.maxValue = 40; // 최대값

        arrowGroupPulse = arrowGroup.GetComponent<ImagePulse>();

        // 마을에 입장할 때 한번 갱신
        UpdateShrineUI();
    }

    void OnEnable()
    {
        // UIShrine이 활성화될 때 애니메이션 시작
        if (arrowGroup.activeSelf && arrowGroupPulse != null)
        {
            arrowGroupPulse.StartPulsing(); // arrowGroup이 활성화된 상태라면 애니메이션 시작
        }
    }

    void OnDisable()
    {
        // UIShrine이 비활성화될 때 애니메이션 중지
        if (arrowGroupPulse != null)
        {
            arrowGroupPulse.StopPulsing(); // 창이 닫히면 애니메이션 중지
        }
    }

    // UI 갱신 함수
    public void UpdateShrineUI()
    {
        // 현재 스탯과 다음 스탯을 텍스트로 표시
        txtCurrStat.text = $"LV: {currStat.Level}\n\nHP: {currStat.MaxHp}\n\nATK: {currStat.Atk}\n\nMAG: {currStat.Magic}";
        txtNextStat.text = $"LV: {nextStat.Level}\n\nHP: {nextStat.MaxHp}\n\nATK: {nextStat.Atk}\n\nMAG: {nextStat.Magic}";

        txtUpGradeSoul.text = $"{upgradeCost}";
        txtSoulMessage.text = $"{soulAmount}";

        // 소울 갱신
        TownManager.Instance.soulDisplay.SetSouls(soulAmount);

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

    public void UpdateSoulAmount(int amount)
    {
        soulAmount = amount;
        UpdateShrineUI();
    }

    // 업그레이드 버튼 클릭시 호출
    private void OnUpgradeButtonClicked()
    {
        if (soulAmount >= upgradeCost)
        {
            // 업데이트된 정보를 서버로 전송
            C_PlayerUpgrade soulPacket = new C_PlayerUpgrade {};

            GameManager.Network.Send(soulPacket);
        }
    }

    private void OnLastStageButtonClicked()
    {
        if (lastStageBtn.gameObject.activeSelf)
        {
            // 서버로 특정 패킷을 전송
            C_EnterFinal lastStagePacket = new C_EnterFinal
            {
                DungeonCode = 5,
            };

            GameManager.Network.Send(lastStagePacket);
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
            bool isMaxLevel = ritualGaugeBar.value >= ritualGaugeBar.maxValue;

            if (finalStageCheck)
            {
                lastStageBtn.gameObject.SetActive(false);
                arrowGroup.SetActive(false);
            }
            else
            {
                lastStageBtn.gameObject.SetActive(isMaxLevel);
                arrowGroup.SetActive(isMaxLevel);
            }
        }
    }

    // 서버에서 받은 패킷을 통해 현재 스탯과 다음 스탯을 업데이트하는 함수
    public void UpdateStatOnServer(S_PlayerUpgrade packet)
    {
        // 현재 스탯을 업데이트
        currStat = new StatInfo
        {
            Level = packet.Player.StatInfo.Level,
            MaxHp = packet.Player.StatInfo.MaxHp,
            Atk = packet.Player.StatInfo.Atk,
            Magic = packet.Player.StatInfo.Magic
        };

        // 다음 스탯을 업데이트
        nextStat = new StatInfo
        {
            Level = packet.Next.Level,
            MaxHp = packet.Next.Hp,
            Atk = packet.Next.Atk,
            Magic = packet.Next.Mag
        };


        soulAmount = packet.Soul;
        upgradeCost = packet.UpgradeCost;
        ritualLevel = packet.RitualLevel;
       
        // UI 업데이트
        UpdateShrineUI();
    }

    public void UpdateFinalCheck(bool finalcheck)
    {
        Debug.Log(finalcheck);
        finalStageCheck = finalcheck;
        UpdateRitualGauge();
    }
}
