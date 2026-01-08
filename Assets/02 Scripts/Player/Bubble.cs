using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private string bubbleColor;
    private Rigidbody2D _rb;
    private Collider2D _col;

    public string Color => bubbleColor;

    public GameObject PrefabReference { get; private set; }

    // 팩토리 메서드: 풀에서 꺼내올 때 자동 초기화
    public static Bubble CreateFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = PoolManager.Instance.GetFromPool(prefab, position, rotation);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.PrefabReference = prefab;

        // Rigidbody 초기화
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;

        // Collider 초기화
        Collider2D col = bubble.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        return bubble;
    }
    // 풀로 반환
    public void ReturnToPool()
    {
        PoolManager.Instance.ReturnToPool(PrefabReference, gameObject);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
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
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;

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
        // 실제 낙하 연출
        _rb.isKinematic = false;
        _rb.gravityScale = 1f;
        _col.enabled = false;

        // 일정 시간 후 풀 반환
        Invoke(nameof(ReturnToPool), 2f);
    }
}
