using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] string bubbleColor;
    private Rigidbody2D rb;

    public string Color => bubbleColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (collision.gameObject.CompareTag("EdgeWall"))
        {
            Vector2 inDirection = rb.velocity;
            Debug.Log($"{inDirection}");
            if (inDirection.sqrMagnitude < 0.0001f) return;
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflectDir = Vector2.Reflect(inDirection, normal);
            rb.velocity = reflectDir.normalized * inDirection.magnitude;
        }
        PhysicsMaterial2D 넣는 방식으로 해결(friction 0 bounciness 1) */
        
        if (collision.gameObject.CompareTag("TopWall") || collision.gameObject.CompareTag("Bubble"))
        {
            AttachToGrid();
        }
    }
    private void AttachToGrid()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // 그리드 좌표 변환
        Vector2 pos = transform.position;
        BubbleManager.Instance.WorldToGrid(pos, out int row, out int col);

        // 등록
        BubbleManager.Instance.RegisterBubble(this, row, col);

        // 위치 스냅
        transform.position = BubbleManager.Instance.GridToWorld(row, col);
        transform.SetParent(BubbleManager.Instance.transform);
    }

}
