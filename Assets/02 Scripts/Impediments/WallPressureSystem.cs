using DG.Tweening;
using TMPro;
using UnityEngine;

public class WallPressureSystem : Singleton<WallPressureSystem>
{
    [SerializeField] private int attachPerPressure = 7; // x번 붙으면 압박
    [SerializeField] private float attachTimeLimit = 10f; // 제한 시간
    [SerializeField] private int moveRows = 2; // 압박 시 내려올 칸 수
    [SerializeField] private float cellSize = 0.65f;
    [SerializeField] private RectTransform LimitTimer;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject hurryUp;

    private int attachCount = 0;
    private float attachTimer;
    private bool pendingPressure = false;
    private bool timerOn = false;
    private bool timerDOT = false;
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

        // 타이머 동작
        if (attachTimer < 4f)
        {
            if (!timerDOT)
            {
                timerDOT = true;
                RotateTimer();
            }
        }
        else
        {
            if (timerDOT)
            {
                DOTween.Kill(LimitTimer);
                LimitTimer.rotation = Quaternion.identity;
                hurryUp.SetActive(false);
            }
            timerDOT = false;
        }

        // 시간 제한 체크
        attachTimer -= Time.deltaTime;
        timerText.text = attachTimer.ToString("F0");
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
        }
    }

    // 버블이 붙을 때 호출
    public void OnBubbleAttached()
    {
        attachCount++;
        attachTimer = attachTimeLimit;

        // 벽 내려오기 전 진동
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
    public void InitCamera()
    {
        DOTween.Kill(Camera.main.transform);
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }
    private void RotateTimer()
    {
        hurryUp.SetActive(true);
        LimitTimer.DORotate(new Vector3(0, 0, 20), 0.1f, RotateMode.Fast)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }
}