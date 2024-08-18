using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISoul : MonoBehaviour
{
	[SerializeField] private TMP_Text txtSoulCount;

	private int soulCount;

	void Start()
	{
	}

	public void AddSouls(int amount)
	{
		soulCount += amount;
		UpdateSoulDisplay();
	}

	private void UpdateSoulDisplay()
	{
		txtSoulCount.text = $"{soulCount}";
	}

	public void ResetSouls()
	{
		soulCount = 0;
		UpdateSoulDisplay();
	}
}
