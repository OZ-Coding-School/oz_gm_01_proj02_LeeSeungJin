using UnityEngine;

public class WallPressureSystem : Singleton<WallPressureSystem>
{
    [SerializeField] private int attachPerPressure = 7; // x번 붙으면 압박
    [SerializeField] private float attachTimeLimit = 10f; // 제한 시간
    [SerializeField] private int moveRows = 2; // 압박 시 내려올 칸 수
    [SerializeField] private float cellSize = 0.65f;

    private int attachCount = 0;
    private float attachTimer = 0f;
    private bool pendingPressure = false;

    protected override bool IsDDOL => false;

    private void Update()
    {
        // 시간 제한 체크
        attachTimer += Time.deltaTime;
        if (attachTimer >= attachTimeLimit)
        {
            pendingPressure = true;
            attachTimer = 0f;
        }

        // 예약된 압박 실행
        if (pendingPressure)
        {
            pendingPressure = false;
            BubbleManager.Instance.ApplyPressure(moveRows);
            transform.position += cellSize * moveRows * Vector3.down;
            Debug.Log("벽 압박 발생!");
        }
    }

    // 버블이 붙을 때 호출
    public void OnBubbleAttached()
    {
        attachCount++;
        attachTimer = 0f;

        if (attachCount >= attachPerPressure)
        {
            attachCount = 0;
            pendingPressure = true; // 다음 프레임에서 압박 실행
        }
    }
}