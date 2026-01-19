using System;
using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public enum BubbleState { Idle, Fired, Falling }
    public enum BubbleColor { Red, Orange, Yellow, Green, Blue, Navy, Violet }

    public BubbleState State { get; private set; }
    public BubbleColor Color => bubbleColor;

    [SerializeField] private BubbleColor bubbleColor;
    private Rigidbody2D _rb;
    private Collider2D _col;

    public GameObject PrefabReference { get; private set; }

    // 한 발 당 한 번만 붙도록
    private bool _hasAttached = false;

    // 턴 종료 트리거용, 붙는 순간 알림
    public static event Action<Bubble> OnBubbleAttachedOnce;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    // 팩토리 메서드: 풀에서 꺼낼 때 초기화
    public static Bubble CreateFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = PoolManager.Instance.GetFromPool(prefab, position, rotation);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.PrefabReference = prefab;
        bubble.ResetBubble();
        return bubble;
    }

    private void ResetBubble()
    {
        _hasAttached = false;
        SetState(BubbleState.Idle);
        transform.SetParent(null);
    }

    // 상태 전환
    public void SetState(BubbleState newState)
    {
        State = newState;

        switch (State)
        {
            case BubbleState.Idle:
                _rb.bodyType = RigidbodyType2D.Kinematic;
                _rb.velocity = Vector2.zero;
                _rb.gravityScale = 0f;
                if (_col != null) _col.enabled = true;
                break;

            case BubbleState.Fired:
                _rb.bodyType = RigidbodyType2D.Dynamic;
                _rb.gravityScale = 0f;
                if (_col != null) _col.enabled = true;
                break;

            case BubbleState.Falling:
                _rb.bodyType = RigidbodyType2D.Dynamic;
                _rb.gravityScale = 1f;
                if (_col != null) _col.enabled = false;
                Invoke(nameof(ReturnToPool), 2f);
                break;
        }
    }

    // 발사 처리
    public void Fire(Vector2 direction, float speed)
    {
        _hasAttached = false;
        SetState(BubbleState.Fired);
        _rb.velocity = direction.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State != BubbleState.Fired || _hasAttached) return;

        if (collision.gameObject.CompareTag("TopWall") || collision.gameObject.CompareTag("Bubble"))
        {
            StartCoroutine(AttachNextFrame());
        }
    }

    private IEnumerator AttachNextFrame()
    {
        _hasAttached = true;

        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0f;

        // 충돌 재발 방지
        if (_col != null) _col.enabled = false;

        yield return new WaitForEndOfFrame();

        AttachToGrid();

        SetState(BubbleState.Idle);
        if (_col != null) _col.enabled = true;

        OnBubbleAttachedOnce?.Invoke(this);
    }

    private void AttachToGrid()
    {
        // 현재 위치를 그리드 좌표로 변환
        Vector2 pos = transform.position;
        BubbleManager.Instance.WorldToGrid(pos, out int row, out int col);

        // 등록 및 매치/낙하 처리
        BubbleManager.Instance.RegisterBubble(this, row, col);
    }

    public void Fall()
    {
        SetState(BubbleState.Falling);
    }

    // 풀 반환
    public void ReturnToPool()
    {
        PoolManager.Instance.ReturnToPool(PrefabReference, gameObject);
    }
}