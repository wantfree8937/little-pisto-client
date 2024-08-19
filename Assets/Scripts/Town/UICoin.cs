using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICoin : MonoBehaviour
{
	[SerializeField] public TMP_Text txtCoinCount;

	public int coinCount;

	void Start()
	{
	}

    public void SetCoins(int amount)
    {
        coinCount = amount;
        UpdateCoinDisplay();
    }

    public void AddCoins(int amount)
	{
		coinCount += amount;
		UpdateCoinDisplay();
	}

    public void SpendCoins(int amount)
    {
        coinCount -= amount;
        UpdateCoinDisplay();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void UpdateCoinDisplay()
	{
		txtCoinCount.text = $"{coinCount}";
	}
}
