using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingView : MonoBehaviour
{
    [SerializeField] private GameObject inputNamePanel;
    [SerializeField] private TextMeshProUGUI rankingText;

    public void ShowInputNamePanel()
    {
        inputNamePanel.SetActive(true);
    }
    public void UpdateRankingList(string txt)
    {
        rankingText.text = txt;
    }
}
