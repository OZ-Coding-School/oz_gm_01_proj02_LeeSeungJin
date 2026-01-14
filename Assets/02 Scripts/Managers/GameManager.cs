using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int score;

    // 초기화
    public void Init()
    {
        score = 0;
    }

    // 게임 시작
    public void StartGame()
    {
        score = 0;
    }

    // 게임 일시정지
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    // 게임 재개
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    // 게임 종료
    public void EndGame(bool gameClear)
    {
        Time.timeScale = 0f;
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
