using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance { get; private set; }

    [SerializeField] private int rows = 12;
    [SerializeField] private int cols = 8;
    [SerializeField] private float cellSize = 0.5f;

    private Bubble[,] grid;

    private void Awake()
    {
        Instance = this;
        grid = new Bubble[rows, cols];
    }

    public void RegisterBubble(Bubble bubble, int row, int col)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            grid[row, col] = bubble;
        }
    }

    public void WorldToGrid(Vector2 pos, out int row, out int col)
    {
        // y좌표 -> 행
        row = Mathf.RoundToInt(pos.y / cellSize);

        // x좌표 -> 열 (홀수 줄은 반지름만큼 오프셋)
        float offset = (row % 2 == 1) ? cellSize / 2f : 0f;
        col = Mathf.RoundToInt((pos.x - offset) / cellSize);
    }
    public Vector2 GridToWorld(int row, int col)
    {
        float offset = (row % 2 == 1) ? cellSize / 2f : 0f;
        float x = col * cellSize + offset;
        float y = row * cellSize;
        return new Vector2(x, y);
    }
    private bool IsValidCell(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }


    public void Init() { }
}
