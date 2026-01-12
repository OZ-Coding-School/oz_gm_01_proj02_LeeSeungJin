using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    // 팩토리 메서드: 풀에서 꺼내올 때 자동 초기화
    public static Bubble CreateFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = PoolManager.Instance.GetFromPool(prefab, position, rotation);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.PrefabReference = prefab;

        bubble.SetState(BubbleState.Idle);

        return bubble;
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
        SetState(BubbleState.Fired);
        _rb.velocity = direction.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TopWall") || collision.gameObject.CompareTag("Bubble"))
        {
            AttachToGrid();
        }
    }

    private void AttachToGrid()
    {
        SetState(BubbleState.Idle);

        // 그리드 좌표 변환
        Vector2 pos = transform.position;
        BubbleManager.Instance.WorldToGrid(pos, out int row, out int col);

        // 등록
        BubbleManager.Instance.RegisterBubble(this, row, col);

        // 위치 스냅
        transform.position = BubbleManager.Instance.GridToWorld(row, col);
        transform.SetParent(BubbleManager.Instance.transform);
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