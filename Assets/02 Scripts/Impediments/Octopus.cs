using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Octopus : MonoBehaviour
{
    [SerializeField] private GameObject octopus;
    [SerializeField] private GameObject[] blackInks;
    [SerializeField] private GameObject dangerImage;
    [SerializeField] private GameObject dangerPanel;

    private WaitForSeconds warningTime = new WaitForSeconds(4f);
    private WaitForSeconds spitCooldown = new WaitForSeconds(5f);

    // 보스 경고
    public IEnumerator WarningCo()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX("SFX_Warning");
        dangerPanel.SetActive(true);
        dangerImage.SetActive(true);
        dangerPanel.GetComponent<Image>().DOFade(0.05f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        yield return warningTime;
        AudioManager.Instance.PlayBGM("BGM_Boss");
        DOTween.Kill(dangerPanel);
        dangerPanel.SetActive(false);
        dangerImage.SetActive(false);
        octopus.SetActive(true);
        octopus.transform.DOMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBounce);
    }

    // 먹물 뿌리기 반복
    public IEnumerator SpitOutInkCo()
    {
        while (true)
        {
            int tmp = Random.Range(0, blackInks.Length);
            AudioManager.Instance.PlaySFX("SFX_SpitOutInk");
            yield return null;
            blackInks[tmp].SetActive(true);
            blackInks[tmp].GetComponent<Image>().DOFade(0.1f, 5f).SetEase(Ease.InOutSine);
            yield return spitCooldown;
            blackInks[tmp].SetActive(false);
            Color color = blackInks[tmp].GetComponent<Image>().color;
            color.a = Mathf.Clamp01(1f);
            blackInks[tmp].GetComponent<Image>().color = color;
        }
    }

    // 보스 클리어
    public void ClearBossRound()
    {
        StopAllCoroutines();
        foreach (var i in blackInks)
        {
            i.SetActive(false);
        }
        octopus.GetComponent<SpriteRenderer>().DOFade(0f, 3f).SetEase(Ease.InOutSine);
    }
}
