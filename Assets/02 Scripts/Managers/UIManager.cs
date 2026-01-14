using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    protected override bool IsDDOL => false;

    [SerializeField] private ScoreView scoreView;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.Init();
        scoreView.UpdateScore(GameManager.Instance.GetScore());
    }
    public void AddScore(int amout)
    {
        GameManager.Instance.AddScore(amout);
        scoreView.UpdateScore(GameManager.Instance.GetScore());
    }
}
