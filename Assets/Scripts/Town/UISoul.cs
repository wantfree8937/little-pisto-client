using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISoul : MonoBehaviour
{
	[SerializeField] public TMP_Text txtSoulCount;

    public int soulCount;

	void Start()
	{
	}

    public void SetSouls(int amount)
    {
        soulCount = amount;
        UpdateSoulDisplay();
    }

    public void AddSouls(int amount)
	{
		soulCount += amount;
		UpdateSoulDisplay();
	}

    public void UpdateSoulDisplay()
	{
		txtSoulCount.text = $"{soulCount}";
	}

    public int GetSoulCount()
    {
        return soulCount;
    }

    public void ResetSouls()
	{
		soulCount = 0;
		UpdateSoulDisplay();
	}
}
