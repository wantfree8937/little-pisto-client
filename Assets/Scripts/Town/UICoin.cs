using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICoin : MonoBehaviour
{
	[SerializeField] private TMP_Text txtCoinCount;

	private int coinCount;

	void Start()
	{
	}

	public void AddCoins(int amount)
	{
		coinCount += amount;
		UpdateCoinDisplay();
	}

	private void UpdateCoinDisplay()
	{
		txtCoinCount.text = $"{coinCount}";
	}

	public void ResetCoins()
	{
		coinCount = 0;
		UpdateCoinDisplay();
	}
}
