using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectModeView : MonoBehaviour
{
    [SerializeField] private GameObject selectModePanel;
    [SerializeField] private Button normalModeButton;
    [SerializeField] private Button noTrajectoryModeButton;

    public event Action OnNormalModeClicked;
    public event Action OnNoTrajectoryModeClicked;

    private void Awake()
    {
        normalModeButton.onClick.AddListener(() => OnNormalModeClicked?.Invoke());
        noTrajectoryModeButton.onClick.AddListener(() => OnNoTrajectoryModeClicked?.Invoke());
    }

    public void OnOffSelectMode()
    {
        if (selectModePanel.activeSelf)
        {
            selectModePanel.SetActive(false);
        }
        else
        {
            selectModePanel.SetActive(true);
        }
    }
}
