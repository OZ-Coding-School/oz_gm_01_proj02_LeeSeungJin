using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int score;
    private bool isGameEnd = false;

    // 초기화
    public void Init()
    {
        score = 0;
    }

    // 게임 시작
    public void StartGame()
    {
        Time.timeScale = 1f;
        //게임 모드 선택
    }

    // 게임 일시정지
    public void PauseGame()
    {
        Time.timeScale = 0f;
        RoundManager.Instance.PlayerKeyEnable(false);
    }

    // 게임 재개
    public void ResumeGame()
    {
        if (isGameEnd) return;

        Time.timeScale = 1f;
        RoundManager.Instance.PlayerKeyEnable(true);
    }

    // 게임 종료
    public string EndGame(bool gameClear)
    {
        isGameEnd = true;
        Time.timeScale = 0f;
        if (gameClear) return "Game Clear";
        else return "Game Over";
    }

    // 점수 추가
    public void AddScore(int amount)
    {
        score += amount;
    }

    // 현재 점수 가져오기
    public int GetScore()
    {
        return score;
    }

}
