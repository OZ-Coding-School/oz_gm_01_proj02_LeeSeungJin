using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject settingPanel;

    public event Action OnSettingClicked;

    private void Awake()
    {
        settingButton.onClick.AddListener(() => OnSettingClicked?.Invoke());
    }
    public bool OnOffSettingPanel()
    {
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
            return false;
        }
        else
        {
            settingPanel.SetActive(true);
            return true;
        }
    }
}
