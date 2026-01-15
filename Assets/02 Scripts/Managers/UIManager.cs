using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    protected override bool IsDDOL => false;

    [SerializeField] private ScoreView scoreView;
    [SerializeField] private SettingView settingView;
    [SerializeField] private ResultView resultView;
    [SerializeField] private GameEndView gameEndView;

    protected override void Awake()
    {
        // 초기화
        base.Awake();
        GameManager.Instance.Init();
        scoreView.UpdateScore(GameManager.Instance.GetScore());

        // 이벤트 구독
        settingView.OnSettingClicked += SettingButtonClick;
        resultView.OnRestartClicked += RestartButtonClick;
        resultView.OnExitClicked += ExitButtonClick;
    }

    // 점수 추가
    public void AddScore(int amout)
    {
        GameManager.Instance.AddScore(amout);
        scoreView.UpdateScore(GameManager.Instance.GetScore());
    }

    // 설정 버튼 클릭
    public void SettingButtonClick()
    {
        if (settingView.OnOffSettingPanel()) GameManager.Instance.PauseGame();
        else GameManager.Instance.ResumeGame();
    }

    // 다시 시작 버튼 클릭
    public void RestartButtonClick()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    // 나가기 버튼 클릭
    public void ExitButtonClick()
    {
        Application.Quit();
        EditorApplication.ExitPlaymode();
    }

    // 게임 결과 창 보여주기 + 연출
    public void ShowGameResult(bool gameClear)
    {
        RoundManager.Instance.PlayerKeyEnable(false);
        WallPressureSystem.Instance.InitCamera();
        StartCoroutine(GameEndCo(gameClear));
    }
    private IEnumerator GameEndCo(bool gameClear)
    {
        if (!gameClear) 
        {
            gameEndView.ApplyGameOverEffect();
            yield return new WaitForSeconds(2f);
        }
        else 
        {
            gameEndView.ApplyGameClearEffect();
            yield return new WaitForSeconds(4f);
        }

        resultView.ShowResult(
            GameManager.Instance.EndGame(gameClear),
            GameManager.Instance.GetScore()
            );
    }
    private void OnDestroy()
    {
        settingView.OnSettingClicked -= SettingButtonClick;
        resultView.OnRestartClicked -= RestartButtonClick;
        resultView.OnRestartClicked -= ExitButtonClick;
        StopAllCoroutines();
    }
}
