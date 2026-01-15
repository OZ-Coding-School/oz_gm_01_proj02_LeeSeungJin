using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuDotween : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject zubino;
    [SerializeField] private GameObject pressStart;
    private void Start()
    {
        Time.timeScale = 1f;
        // Title 위아래 반복
        title.transform.DOMoveY(title.transform.position.y + 0.5f, 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // Zubino 위아래 반복
        zubino.transform.DOMoveY(zubino.transform.position.y - 0.3f, 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // PressStart 깜빡임 반복
        pressStart.GetComponent<SpriteRenderer>().DOFade(0.1f, 0.7f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
