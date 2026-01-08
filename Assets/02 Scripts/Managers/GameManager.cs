using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState { None, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.None;

    private int score;

    // 초기화
    public void Init()
    {
        score = 0;
        CurrentState = GameState.None;
    }

    // 게임 시작
    public void StartGame()
    {
        score = 0;
        CurrentState = GameState.Playing;
    }

    // 게임 일시정지
    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
        }
    }

    // 게임 재개
    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
        }
    }

    // 게임 종료
    public void EndGame()
    {
        CurrentState = GameState.GameOver;
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
