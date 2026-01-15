using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameEndView : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private GameObject fireWorksEffect;
    [SerializeField] private GameObject curBubble;
    [SerializeField] private GameObject timer;

    private ColorGrading colorGrading;

    private void Awake()
    {
        postProcessVolume.profile.TryGetSettings(out colorGrading);
    }

    public void ApplyGameOverEffect()
    {
        curBubble.SetActive(false);
        timer.SetActive(false);
        colorGrading.enabled.value = true;

        DOTween.To(() => colorGrading.saturation.value,
                   x => colorGrading.saturation.value = x,
                   -100f, 2f);

    }
    public void ApplyGameClearEffect()
    {
        curBubble.SetActive(false);
        timer.SetActive(false);
        fireWorksEffect.SetActive(true);
    }
}
