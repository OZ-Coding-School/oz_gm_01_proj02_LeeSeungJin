using DG.Tweening;
using UnityEngine;

public class WallPressureSystem : Singleton<WallPressureSystem>
{
    [SerializeField] private int attachPerPressure = 7; // x번 붙으면 압박
    [SerializeField] private float attachTimeLimit = 10f; // 제한 시간
    [SerializeField] private int moveRows = 2; // 압박 시 내려올 칸 수
    [SerializeField] private float cellSize = 0.65f;

    private int attachCount = 0;
    private float attachTimer;
    private bool pendingPressure = false;
    private bool timerOn = false;
    private Vector2 initPosition;

    protected override bool IsDDOL => false;
    public bool TimerOn { get { return timerOn; } set { timerOn = value; } }

    private void Start()
    {
        initPosition = transform.position;
        attachTimer = attachTimeLimit;
    }
    private void Update()
    {
        if (!timerOn) return;

        // 시간 제한 체크
        attachTimer -= Time.deltaTime;
        if (attachTimer <= 0f)
        {
            pendingPressure = true;
            attachTimer = attachTimeLimit;
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
        attachTimer = attachTimeLimit;
        if (attachCount == attachPerPressure - 2)
            Camera.main.transform.DOShakePosition(
                duration: 1f,
                strength: 0.05f,
                vibrato: 20,
                randomness: 90f
                ).SetLoops(-1, LoopType.Restart);
        if (attachCount == attachPerPressure - 1)
            Camera.main.transform.DOShakePosition(
                duration: 0.5f,
                strength: 0.15f,
                vibrato: 30,
                randomness: 90f
                ).SetLoops(-1, LoopType.Restart);

        if (attachCount >= attachPerPressure)
        {
            attachCount = 0;
            pendingPressure = true; // 다음 프레임에서 압박 실행
            InitCamera();
        }
    }

    //라운드 종료 후 초기화
    public void Init()
    {
        transform.position = initPosition;
        attachTimer = attachTimeLimit;
        attachCount = 0;
        InitCamera();
    }
    private void InitCamera()
    {
        DOTween.Kill(Camera.main.transform);
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }
}