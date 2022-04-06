using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI ammoText;
    public GameObject crosshair;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RefreshAmmoUI(int ammoCount)
    {
        ammoText.text = ammoCount.ToString();
    }

    public void ToggleCrosshair(bool toggle)
    {
        crosshair.SetActive(toggle);
    }
}
