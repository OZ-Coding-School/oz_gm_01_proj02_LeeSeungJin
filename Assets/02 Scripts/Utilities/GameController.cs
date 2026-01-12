using UnityEngine;

public class GameController : MonoBehaviour
{
    private void OnEnable()
    {
        Bubble.OnBubbleAttachedOnce += HandleBubbleAttached;
    }

    private void OnDisable()
    {
        Bubble.OnBubbleAttachedOnce -= HandleBubbleAttached;
    }

    private void HandleBubbleAttached(Bubble bubble)
    {
        // 버블이 붙으면 턴 종료 처리
        EndTurn();
    }

    private void EndTurn()
    {
        // BubbleManager에 턴 종료 알림 → 압박 카운트 증가
        BubbleManager.Instance.EndTurn();
    }
}