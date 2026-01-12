using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : Singleton<RoundManager>
{
    [SerializeField] private Image roundMessageImage;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite startSprite;
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
            roundMessageImage.sprite = readySprite;
            roundMessageImage.gameObject.SetActive(true);
            player.SetCanFire(false);
            yield return new WaitForSeconds(1.5f);

            // 2. Start 표시
            roundMessageImage.sprite = startSprite;
            yield return new WaitForSeconds(1.0f);
            roundMessageImage.gameObject.SetActive(false);

            // 3. 버블 배치
            BubbleManager.Instance.ClearAllBubbles();
            BubbleManager.Instance.SpawnRound(rounds[currentRound]);

            // 4. 발사 가능
            player.SetCanFire(true);

            // 5. 라운드 종료 대기
            yield return new WaitUntil(() => BubbleManager.Instance.AllCleared());

            // 라운드 클리어 → 다음 라운드
            currentRound++;
        }

        // 모든 라운드 클리어
        player.SetCanFire(false);
        roundMessageImage.sprite = readySprite; // 예: "Game Clear!" 이미지로 교체 가능
        roundMessageImage.gameObject.SetActive(true);
    }
}
