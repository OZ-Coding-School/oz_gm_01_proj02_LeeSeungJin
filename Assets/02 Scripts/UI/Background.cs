using UnityEngine;
using DG.Tweening;

public class Background : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] backgrounds;

    private static SpriteRenderer[] backgroundSprites;

    private void Start()
    {
        backgroundSprites = backgrounds;
    }
    public static void FadeOutBG(int i)
    {
        backgroundSprites[i].DOFade(0f, 2f);
    }
}
