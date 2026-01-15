using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private TextMeshProUGUI gameResultText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    public event Action OnRestartClicked;
    public event Action OnExitClicked;

    private void Awake()
    {
        restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
        exitButton.onClick.AddListener(() => OnExitClicked?.Invoke());
    }

    public void ShowResult(string gameResult, int finalScore)
    {
        resultCanvas.SetActive(true);
        gameResultText.text = gameResult;
        finalScoreText.text = finalScore.ToString();
    }
}
