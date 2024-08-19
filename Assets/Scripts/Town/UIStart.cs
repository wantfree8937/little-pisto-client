using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterInfo
{
    public string Name;
    public int UnlockCost;

    public GameObject LockedImage;
    public GameObject UnlockedImage;

    public CharacterInfo(string name, int unlockCost, GameObject lockedImage, GameObject unlockedImage)
    {
        Name = name;
        UnlockCost = unlockCost;
        LockedImage = lockedImage;
        UnlockedImage = unlockedImage;
    }
}


public class UIStart : MonoBehaviour
{
    [SerializeField] private GameObject charList;
    [SerializeField] private Button[] charBtns;

    [SerializeField] private Button btnConfirm1;
    [SerializeField] private Button btnConfirm2;
    [SerializeField] private Button btnBack1;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private TMP_InputField inputPort;
    [SerializeField] private TMP_Text txtMessage;
    private TMP_Text placeHolder;

    [SerializeField] private GameObject popupPanel;
    [SerializeField] private GameObject popupWindow;
    [SerializeField] private TMP_Text popupMessage;
    [SerializeField] private Button popupConfirmButton;
    [SerializeField] private Button popupCancelButton;

    [SerializeField] private GameObject insufficientCoinsPopupWindow;
    [SerializeField] private TMP_Text insufficientCoinsMessage;
    [SerializeField] private Button insufficientCoinsConfirmButton;

    private int classIdx = 0;

    private string serverUrl;
    private string nickname;
    private string port;

    private bool[] isCharacterUnlocked;
    private Dictionary<int, CharacterInfo> characterInfos;
    private Action onPopupConfirm;

    void Start()
    {
        Debug.Log("UIStart: Start() called");

        placeHolder = inputNickname.placeholder.GetComponent<TMP_Text>();
        btnBack1.onClick.AddListener(SetServerUI);

        InitializeCharacterInfos();

        for (int i = 0; i < charBtns.Length; i++)
        {
            int idx = i;
            charBtns[i].onClick.AddListener(() => OnCharacterButtonClick(idx));

            // 초기 이미지 설정
            if (isCharacterUnlocked[i])
            {
                characterInfos[idx].LockedImage.SetActive(false);
                characterInfos[idx].UnlockedImage.SetActive(true);
            }
            else
            {
                characterInfos[idx].LockedImage.SetActive(true);
                characterInfos[idx].UnlockedImage.SetActive(false);
            }
        }

        Debug.Log("UIStart: Character buttons initialized");

        popupConfirmButton.onClick.AddListener(() =>
        {
            onPopupConfirm?.Invoke();
            ClosePopup();
        });

        popupCancelButton.onClick.AddListener(ClosePopup);

        insufficientCoinsConfirmButton.onClick.AddListener(CloseInsufficientCoinsPopup);

        SetServerUI();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputNickname.IsActive())
                btnConfirm1.onClick.Invoke();
        }
    }

    void InitializeCharacterInfos()
    {
        characterInfos = new Dictionary<int, CharacterInfo>
        {
            { 0, new CharacterInfo("케르베", 0, charBtns[0].transform.Find("LockedImage").gameObject, charBtns[0].transform.Find("UnlockedImage").gameObject) },
            { 1, new CharacterInfo("유니", 0, charBtns[1].transform.Find("LockedImage").gameObject, charBtns[1].transform.Find("UnlockedImage").gameObject) },
            { 2, new CharacterInfo("닉스", 0, charBtns[2].transform.Find("LockedImage").gameObject, charBtns[2].transform.Find("UnlockedImage").gameObject) },
            { 3, new CharacterInfo("차드", 100, charBtns[3].transform.Find("LockedImage").gameObject, charBtns[3].transform.Find("UnlockedImage").gameObject) },
            { 4, new CharacterInfo("미호", 100, charBtns[4].transform.Find("LockedImage").gameObject, charBtns[4].transform.Find("UnlockedImage").gameObject) },
            { 5, new CharacterInfo("레비", 300, charBtns[5].transform.Find("LockedImage").gameObject, charBtns[5].transform.Find("UnlockedImage").gameObject) },
            { 6, new CharacterInfo("와이브", 300, charBtns[6].transform.Find("LockedImage").gameObject, charBtns[6].transform.Find("UnlockedImage").gameObject) },
            { 7, new CharacterInfo("드라고", 500,charBtns[7].transform.Find("LockedImage").gameObject, charBtns[7].transform.Find("UnlockedImage").gameObject) },
            { 8, new CharacterInfo("키리", 500, charBtns[8].transform.Find("LockedImage").gameObject, charBtns[8].transform.Find("UnlockedImage").gameObject) },
            // 나머지 캐릭터들도 동일하게 설정
        };

        isCharacterUnlocked = new bool[charBtns.Length];
        for (int i = 0; i < isCharacterUnlocked.Length; i++)
        {
            isCharacterUnlocked[i] = (i < 3);
        }

        Debug.Log("UIStart: Initial character unlock status and infos set");
    }

    void OnCharacterButtonClick(int idx)
    {
        Debug.Log($"UIStart: OnCharacterButtonClick({idx}) called");

        if (isCharacterUnlocked[idx])
        {
            SelectCharacter(idx);
        }
        else
        {
            ConfirmUnlockCharacter(idx);
        }
    }

    void SelectCharacter(int idx)
    {
        Debug.Log($"UIStart: SelectCharacter({idx}) called");

        // 이전에 선택된 캐릭터의 테두리를 비활성화
        charBtns[classIdx].transform.Find("Border").gameObject.SetActive(false);

        // 새로운 캐릭터의 인덱스를 저장
        classIdx = idx;

        // 현재 선택된 캐릭터의 테두리를 활성화
        charBtns[classIdx].transform.Find("Border").gameObject.SetActive(true);
    }


    void ConfirmUnlockCharacter(int idx)
    {
        Debug.Log($"UIStart: ConfirmUnlockCharacter({idx}) called");

        if (characterInfos.TryGetValue(idx, out CharacterInfo characterInfo))
        {
            ShowPopup(
                $"{characterInfo.Name}을(를) {characterInfo.UnlockCost} 코인으로 잠금 해제하시겠습니까?",
                () => {
                    if (TownManager.Instance.coinDisplay.GetCoinCount() >= characterInfo.UnlockCost)
                    {
                        Debug.Log($"UIStart: Character {idx} unlocked with {characterInfo.UnlockCost} coins");
                        TownManager.Instance.coinDisplay.SpendCoins(characterInfo.UnlockCost);
                        UnlockCharacter(idx);
                    }
                    else
                    {
                        ShowInsufficientCoinsPopup("코인이 부족합니다!");
                    }
                }
            );
        }
    }

    void UnlockCharacter(int idx)
    {
        Debug.Log($"UIStart: UnlockCharacter({idx}) called");

        isCharacterUnlocked[idx] = true;
        charBtns[idx].interactable = true;

        if (characterInfos.TryGetValue(idx, out CharacterInfo characterInfo))
        {
            characterInfo.LockedImage.SetActive(false);
            characterInfo.UnlockedImage.SetActive(true);
        }

        SelectCharacter(idx);
        Debug.Log($"UIStart: Character {idx} unlocked and selected");
    }

    void SetServerUI()
    {
        Debug.Log("UIStart: SetServerUI() called");

        txtMessage.color = Color.white;
        txtMessage.text = "Welcome!";

        inputNickname.text = string.Empty;
        placeHolder.text = "서버주소를 입력해주세요!";

        charList.gameObject.SetActive(false);
        btnBack1.gameObject.SetActive(false);
        inputPort.gameObject.SetActive(true);

        btnConfirm1.onClick.RemoveAllListeners();
        btnConfirm1.onClick.AddListener(ConfirmServer);
    }

    void SetNicknameUI()
    {
        Debug.Log("UIStart: SetNicknameUI() called");

        txtMessage.color = Color.white;
        txtMessage.text = "닉네임을 설정해주세요";

        inputNickname.text = string.Empty;
        placeHolder.text = "닉네임을 입력해주세요 (2~10글자)";

        charList.gameObject.SetActive(false);
        btnBack1.gameObject.SetActive(true);
        inputPort.gameObject.SetActive(false);

        btnConfirm1.onClick.RemoveAllListeners();
        btnConfirm1.onClick.AddListener(ConfirmNickname);
    }

    void SetCharacterSelectionUI()
    {
        Debug.Log("UIStart: SetCharacterSelectionUI() called");

        txtMessage.color = Color.white;
        txtMessage.text = "캐릭터를 선택해주세요";

        charList.gameObject.SetActive(true);
        btnBack1.gameObject.SetActive(false);

        inputNickname.gameObject.SetActive(false);
        inputPort.gameObject.SetActive(false);

        btnConfirm2.onClick.RemoveAllListeners();
        btnConfirm2.onClick.AddListener(StartGame);
    }

    void ConfirmServer()
    {
        Debug.Log("UIStart: ConfirmServer() called");

        serverUrl = inputNickname.text;
        port = inputPort.text;
        SetNicknameUI();
    }

    void ConfirmNickname()
    {
        Debug.Log("UIStart: ConfirmNickname() called");

        txtMessage.color = Color.red;

        if (inputNickname.text.Length < 2)
        {
            txtMessage.text = "이름을 2글자 이상 입력해주세요!";
            Debug.Log("UIStart: Nickname is too short");
            return;
        }

        if (inputNickname.text.Length > 10)
        {
            txtMessage.text = "이름을 10글자 이하로 입력해주세요!";
            Debug.Log("UIStart: Nickname is too long");
            return;
        }

        nickname = inputNickname.text;
        Debug.Log($"UIStart: Nickname confirmed: {nickname}");

        SetCharacterSelectionUI();
    }

    void StartGame()
    {
        Debug.Log($"UIStart: StartGame() called with classIdx = {classIdx}");

        TownManager.Instance.GameStart(serverUrl, port, nickname, classIdx);
        gameObject.SetActive(false);
    }

    void ShowInsufficientCoinsPopup(string message)
    {
        insufficientCoinsMessage.text = message;
        insufficientCoinsPopupWindow.SetActive(true);
    }

    void CloseInsufficientCoinsPopup()
    {
        insufficientCoinsPopupWindow.SetActive(false);
    }

    void ShowPopup(string message, Action confirmAction)
    {
        popupMessage.text = message;
        onPopupConfirm = confirmAction;
        popupPanel.SetActive(true);
        popupWindow.SetActive(true);
    }

    void ClosePopup()
    {
        popupPanel.SetActive(false);
        popupWindow.SetActive(false);
    }
}
