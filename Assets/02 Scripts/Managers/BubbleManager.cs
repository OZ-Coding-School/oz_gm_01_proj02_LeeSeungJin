using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BubbleManager : Singleton<BubbleManager>
{
    protected override bool IsDDOL => false;

    [SerializeField] private GameObject[] bubblePrefabs;
    [SerializeField] private GameObject bubblePopPrefab;
    [SerializeField] private int rows = 12;
    [SerializeField] private int cols = 8;
    [SerializeField] private float cellSize = 0.5f;

    // 그리드 원점 조정
    [SerializeField] private Vector2 gridOrigin = Vector2.zero;

    private Bubble[,] grid;
    private int ceilingRow = 0;
    private bool bubbleAttachedThisTurn = false;
    private GameObject tmpBubblePop;

    protected override void Awake()
    {
        base.Awake();
        grid = new Bubble[rows, cols];
        foreach (var bubble in bubblePrefabs)
        {
            PoolManager.Instance.CreatePool(bubble, 30);
        }
        PoolManager.Instance.CreatePool(bubblePopPrefab, 10);
    }

    // 버블 등록
    public void RegisterBubble(Bubble bubble, int row, int col)
    {
        if (!IsValidCell(row, col)) return;

        int targetRow = row; // 같은 자리에 배치되는 버그 픽스
        while (targetRow < rows && grid[targetRow, col] != null)
        {
            targetRow++;
        }

        grid[targetRow, col] = bubble;

        bubble.transform.position = GridToWorld(targetRow, col); // 위치 스냅
        bubble.transform.SetParent(transform);

        bool matched = CheckMatch(targetRow, col);
        if (matched) HandleFloatingBubbles();

        bubbleAttachedThisTurn = true; // 이번 턴에 붙음 표시
    }
    public void EndTurn()
    {
        if (bubbleAttachedThisTurn)
        {
            WallPressureSystem.Instance.OnBubbleAttached();
            bubbleAttachedThisTurn = false;
        }
    }


    // 월드 → 그리드 좌표 변환
    public void WorldToGrid(Vector2 pos, out int row, out int col)
    {
        float localX = pos.x - gridOrigin.x;
        float localY = gridOrigin.y - pos.y;

        row = Mathf.RoundToInt(localY / cellSize);
        float offset = (row % 2 == 1) ? cellSize * 0.5f : 0f;
        col = Mathf.RoundToInt((localX - offset) / cellSize);

        row = Mathf.Clamp(row, 0, rows - 1);
        col = Mathf.Clamp(col, 0, cols - 1);
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
    private bool CheckMatch(int startRow, int startCol)
    {
        Bubble startBubble = grid[startRow, startCol];
        if (startBubble == null) return false;

        Bubble.BubbleColor color = startBubble.Color;
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
            //점수 추가
            UIManager.Instance.AddScore(connected.Count * connected.Count * 100);
            AudioManager.Instance.PlaySFX("SFX_BubblePop");
            foreach (var (r, c) in connected)
            {
                Bubble bubble = grid[r, c];
                if (bubble != null)
                {
                    tmpBubblePop = bubblePopPrefab;
                    BubblePopEffect effect = BubblePopEffect.CreateFromPool(tmpBubblePop, grid[r,c].transform.position, Quaternion.identity);
                    bubble.ReturnToPool();
                    grid[r, c] = null;
                }
            }
            return true;
        }
        return false;
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
        return neighbors;
    }

    // 매칭 후 낙하 처리 호출
    private void HandleFloatingBubbles()
    {
        bool[,] visited = new bool[rows, cols];

        for (int c = 0; c < cols; c++)
        {
            if (grid[ceilingRow, c] != null)
            {
                MarkConnectedDFS(ceilingRow, c, visited);
            }
        }
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] != null && !visited[r, c])
                {
                    Bubble bubble = grid[r, c];
                    grid[r, c] = null;
                    bubble.Fall();
                    //점수 추가
                    UIManager.Instance.AddScore(100);
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

    // 압박 시 그리드 전체 이동 (2칸 단위 + 원점 이동)
    public void ApplyPressure(int moveRows = 2)
    {
        for (int r = rows - 1; r >= 0; r--)
        {
            for (int c = 0; c < cols; c++)
            {
                Bubble bubble = grid[r, c];
                if (bubble != null)
                {
                    int newRow = r + moveRows;
                    if (newRow < rows)
                    {
                        grid[newRow, c] = bubble;
                        grid[r, c] = null;
                        bubble.transform.position = GridToWorld(newRow, c);
                        bubble.transform.SetParent(transform);
                    }
                    else
                    {
                       // 바닥에 닿으면 GameOver 처리
                    }
                }
            }
        }
        // 천장 기준 row 갱신
        ceilingRow += moveRows;
        if (ceilingRow >= rows) ceilingRow = rows - 1;
    }

    // 모든 버블 제거 및 초기화
    public void InitGrid()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] != null)
                {
                    grid[r, c].ReturnToPool();
                    grid[r, c] = null;
                }
            }
        }
        ceilingRow = 0;
    }

    // 라운드 데이터에 따라 버블 배치
    public void SpawnRound(RoundData roundData)
    {
        for (int i = 0; i < roundData.positions.Length; i++)
        {
            Vector2Int pos = roundData.positions[i];
            Bubble.BubbleColor color = roundData.colors[i];

            // 프리팹 선택 (색상에 맞는 프리팹을 가져오는 로직 필요)
            GameObject prefab = BubblePrefabLibrary.Instance.GetPrefab(color);

            Bubble bubble = Bubble.CreateFromPool(prefab, GridToWorld(pos.x, pos.y), Quaternion.identity);
            RegisterBubble(bubble, pos.x, pos.y);
        }
    }

    // 모든 버블이 제거되었는지 확인 (라운드 종료 조건)
    public bool AllCleared()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] != null)
                    return false;
            }
        }
        return true;
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
