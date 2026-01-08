using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : Singleton<BubbleManager>
{
    [SerializeField] private GameObject[] bubblePrefab;
    [SerializeField] private int rows = 12;
    [SerializeField] private int cols = 8;
    [SerializeField] private float cellSize = 0.5f;

    // 그리드 원점 조정
    [SerializeField] private Vector2 gridOrigin = Vector2.zero;

    private Bubble[,] grid;

    protected override void Awake()
    {
        base.Awake();
        grid = new Bubble[rows, cols];
        foreach (var bubble in bubblePrefab)
        {
            PoolManager.Instance.CreatePool(bubble, 30);
        }
    }

    // 버블 등록
    public void RegisterBubble(Bubble bubble, int row, int col)
    {
        if (IsValidCell(row, col))
        {
            grid[row, col] = bubble;
            CheckMatch(row, col);
        }
    }

    // 월드 → 그리드 좌표 변환 (원점 기준 + Floor 안정화)
    public void WorldToGrid(Vector2 pos, out int row, out int col)
    {
        float localX = pos.x - gridOrigin.x;
        float localY = gridOrigin.y - pos.y; // 위에서 아래로 증가하도록 반전

        row = Mathf.FloorToInt(localY / cellSize + 0.5f);
        float offset = (row % 2 == 1) ? cellSize * 0.5f : 0f;
        col = Mathf.FloorToInt((localX - offset) / cellSize + 0.5f);

        row = Mathf.Clamp(row, 0, rows - 1);
        col = Mathf.Clamp(col, 0, cols - 1);

        Debug.Log($"WorldToGrid: pos={pos}, row={row}, col={col}");
    }

    // 그리드 → 월드 좌표 변환
    public Vector2 GridToWorld(int row, int col)
    {
        float offset = (row % 2 == 1) ? cellSize * 0.5f : 0f;
        float x = gridOrigin.x + col * cellSize + offset;
        float y = gridOrigin.y - row * cellSize;
        return new Vector2(x, y);
    }

    private bool IsValidCell(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }

    // DFS 방식으로 매칭 판정
    private void CheckMatch(int startRow, int startCol)
    {
        Bubble startBubble = grid[startRow, startCol];
        if (startBubble == null) return;

        string color = startBubble.Color;
        List<(int, int)> connected = new List<(int, int)>();
        bool[,] visited = new bool[rows, cols];

        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push((startRow, startCol));

        while (stack.Count > 0)
        {
            var (r, c) = stack.Pop();
            if (!IsValidCell(r, c) || visited[r, c]) continue;

            Bubble b = grid[r, c];
            if (b != null && b.Color == color)
            {
                visited[r, c] = true;
                connected.Add((r, c));

                foreach (var (nr, nc) in GetNeighbors(r, c))
                {
                    stack.Push((nr, nc));
                }
            }
        }

        if (connected.Count >= 3)
        {
            foreach (var (r, c) in connected)
            {
                Bubble bubble = grid[r, c];
                if (bubble != null)
                {
                    bubble.ReturnToPool();
                    grid[r, c] = null;
                }
            }
            Debug.Log($"매칭 성공: {connected.Count}개 제거");

            HandleFloatingBubbles();
        }
    }

    // 인접 6방향 탐색
    private List<(int, int)> GetNeighbors(int row, int col)
    {
        List<(int, int)> neighbors = new List<(int, int)>();

        if (row % 2 == 0) // 짝수 줄
        {
            neighbors.Add((row, col - 1));
            neighbors.Add((row, col + 1));
            neighbors.Add((row - 1, col));
            neighbors.Add((row - 1, col - 1));
            neighbors.Add((row + 1, col));
            neighbors.Add((row + 1, col - 1));
        }
        else // 홀수 줄
        {
            neighbors.Add((row, col - 1));
            neighbors.Add((row, col + 1));
            neighbors.Add((row - 1, col));
            neighbors.Add((row - 1, col + 1));
            neighbors.Add((row + 1, col));
            neighbors.Add((row + 1, col + 1));
        }

        Debug.Log($"GetNeighbors: row={row}, col={col}, count={neighbors.Count}");
        return neighbors;
    }

    // 매칭 후 낙하 처리 호출
    private void HandleFloatingBubbles()
    {
        bool[,] visited = new bool[rows, cols];

        // 천장(0번째 row)에 붙은 버블들부터 DFS 탐색
        for (int c = 0; c < cols; c++)
        {
            if (grid[0, c] != null)
            {
                MarkConnectedDFS(0, c, visited);
            }
        }

        // 방문되지 않은 버블은 떨어짐 처리
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] != null && !visited[r, c])
                {
                    Bubble bubble = grid[r, c];
                    grid[r, c] = null;
                    bubble.Fall();
                }
            }
        }
    }

    // DFS로 연결된 버블 표시
    private void MarkConnectedDFS(int row, int col, bool[,] visited)
    {
        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push((row, col));

        while (stack.Count > 0)
        {
            var (r, c) = stack.Pop();
            if (!IsValidCell(r, c) || visited[r, c]) continue;

            Bubble b = grid[r, c];
            if (b != null)
            {
                visited[r, c] = true;

                foreach (var (nr, nc) in GetNeighbors(r, c))
                {
                    stack.Push((nr, nc));
                }
            }
        }
    }

//========================================================================
    // Scene 뷰에서 그리드 시각화
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(gridOrigin, 0.05f); // 원점 표시

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector2 worldPos = GridToWorld(r, c);
                Gizmos.DrawWireSphere(worldPos, cellSize * 0.45f);

                if (grid[r, c] != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(worldPos, cellSize * 0.3f);
                    Gizmos.color = Color.gray;
                }
            }
        }
    }

    // 인접 관계 디버그
    public void DebugNeighbors(int row, int col)
    {
        foreach (var (nr, nc) in GetNeighbors(row, col))
        {
            if (IsValidCell(nr, nc))
            {
                Vector2 from = GridToWorld(row, col);
                Vector2 to = GridToWorld(nr, nc);
                Debug.DrawLine(from, to, Color.red, 1f);
            }
        }
    }

}
