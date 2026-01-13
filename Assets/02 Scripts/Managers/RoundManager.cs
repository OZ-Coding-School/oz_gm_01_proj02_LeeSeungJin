using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class RoundManager : Singleton<RoundManager>
{
    [SerializeField] private TextMeshProUGUI currentRoundText;
    [SerializeField] private Text roundClearText;
    [SerializeField] private GameObject readySprite;
    [SerializeField] private GameObject startSprite;
    [SerializeField] private PlayerController player;
    [SerializeField] private RoundData[] rounds; // 라운드별 버블 배치 데이터

    private int currentRound = 0;
    public int CurrentRound => currentRound;
    protected override bool IsDDOL => false;

    private void Start()
    {
        StartCoroutine(RunRoundRoutine());
    }

    private IEnumerator RunRoundRoutine()
    {
        while (currentRound < rounds.Length)
        {
            // 1. Ready 표시
            currentRoundText.text = $"ROUND {currentRound+1}";
            player.SetCanFire(false);
            readySprite.SetActive(true);
            readySprite.transform.DOMove(new Vector3(0,0.3f,0), 1f).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(2.0f);
            readySprite.SetActive(false);
            readySprite.transform.position += Vector3.up * 5f;

            // 2. Start 표시
            startSprite.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            startSprite.SetActive(false);

            // 3. 버블 배치
            BubbleManager.Instance.SpawnRound(rounds[currentRound]);

            // 4. 발사 가능
            player.SetCanFire(true);
            WallPressureSystem.Instance.TimerOn = true;

            // 5. 라운드 종료 대기
            yield return new WaitUntil(() => BubbleManager.Instance.AllCleared());

            // 라운드 클리어 -> 초기화 -> 다음 라운드 준비
            roundClearText.gameObject.SetActive(true);
            string tmp = $"ROUND CLEAR\n\nBONUS POINTS\n{(currentRound + 1) * 7000}";
            roundClearText.DOText(tmp, 2f, false, ScrambleMode.None);

            WallPressureSystem.Instance.TimerOn = false;
            BubbleManager.Instance.InitGrid();
            WallPressureSystem.Instance.Init();
            currentRound++;
            yield return new WaitForSeconds(3.0f);
            roundClearText.gameObject.SetActive(false);
        }

        // 모든 라운드 클리어
        player.SetCanFire(false);
        GameManager.Instance.EndGame();
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
