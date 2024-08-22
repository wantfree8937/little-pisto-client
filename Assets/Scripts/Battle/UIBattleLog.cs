using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class UIBattleLog : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLog;
    [SerializeField] private Button[] btns;
    [SerializeField] private TMP_Text[] btnTexts;
    [SerializeField] private Button clearBtn;

    [SerializeField] private GameObject BossClear;
    [SerializeField] private Image imgContinue;
    

    private BtnInfo[] btnInfos = null;
    private bool done = false;
    private string msg;
    
    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int idx = i+1;
            btns[i].onClick.AddListener(() => { OnClick(idx);});
        }

        if (clearBtn != null)
        {
            clearBtn.onClick.AddListener(() => OnClick(1));
        }
    }

    public void Set(BattleLog battleLog)
    {
        if (battleLog.Btns is { Count: > 0 })
            btnInfos = battleLog.Btns?.ToArray();
        else
            btnInfos = null;

        SetLog(battleLog.Msg, battleLog.TypingAnimation);
    }

    public void SetBossClear()
    {
        C_FinalCheck c_FinalCheck = new C_FinalCheck {};
        GameManager.Network.Send(c_FinalCheck);
        BossClear.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if(BattleManager.Instance.UiScreen.gameObject.activeSelf)
                return;
            
            if (done == false)
            {
                DOTween.KillAll();
                txtLog.text = msg;
                LogDone();
            }
            else
            {
                if (btnInfos == null)
                    Response(0);
            }
        }
    }
    
    public void LogDone()
    {
        done = true;

        if (btnInfos == null)
            imgContinue.DOFade(1, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
        else
            SetBtn(btnInfos);
    }
    
    public void SetLog(string log,  bool typing = true)
    {
        done = false;
        
        DOTween.KillAll();
        txtLog.text = String.Empty;
        imgContinue.color = new Color(imgContinue.color.r, imgContinue.color.g, imgContinue.color.b , 0);
        
        
        msg = log;
        StartCoroutine(playSound(msg));
        
        if(typing)
            txtLog.DOText(msg, msg.Length/20).SetEase(Ease.Linear).OnComplete(LogDone);
        else
        {
            txtLog.text = msg;
            LogDone();
        }
    }

    private IEnumerator playSound(string msg)
    {
        // BGM 설정
        switch(msg){
            case "전투 승리!":
            Managers.Sound.SetBgmVolume(0.04f);
            Managers.Sound.Play("stageClear", volume: 0.2f);
            yield return new WaitForSeconds(2.3f); //2.3초 기다림
            Managers.Sound.SetBgmVolume(0.1f);
            break;
            default:
            break;
         }

    }

    private void SetBtn(BtnInfo[] btnInfos)
    {
        foreach (var btn in btns)
            btn.gameObject.SetActive(false);

        for (int i = 0; i < btnInfos.Length; i++)
        {
            var btnInfo = btnInfos[i];
            btns[i].gameObject.SetActive(true);
            btns[i].interactable = btnInfo.Enable; 
            btnTexts[i].text = btnInfo.Msg;
        }
    }

    private void OnClick(int idx)
    {
        Response(idx);
    }

    void Response(int idx)
    {
        C_PlayerResponse response = new C_PlayerResponse() { ResponseCode = idx };
        GameManager.Network.Send(response);
    }
}