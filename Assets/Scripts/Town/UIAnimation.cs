using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Button btnBattle;
    [SerializeField] private Button[] btnList;
    [SerializeField] private Button character_Select;
    private MyPlayer mPlayer;
    
    void Start()
    {
        for (int i = 0; i < btnList.Length; i++)
        {
            int idx = i;
            btnList[i].onClick.AddListener(() =>
            {
                PlayAnimation(idx);
            });
        }
        
        mPlayer = TownManager.Instance.myPlayer.mPlayer;

        character_Select.onClick.AddListener(OnCharacterSelect);
    }
    
    private void PlayAnimation(int idx)
    {
        if (mPlayer == null)
            return;

        mPlayer.AnimationExecute(idx);
    }

    private void OnCharacterSelect()
    {
        SceneManager.LoadScene(GameManager.TownScene);
    }
}
