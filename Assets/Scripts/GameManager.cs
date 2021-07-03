using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Tile m_TilePrefab;
    [SerializeField] Transform m_TileParent;
    [SerializeField] Player m_PlayerPrefab;

    private int m_GridSize = 2;
    private Tile[,] m_Grid;
    private Player m_Player;

    private void Awake()
    {
        Instance = this;
    }

    public void Generate(int gridSize)
    {
        ResetObjects();

        m_GridSize = gridSize;
        m_Grid = new Tile[m_GridSize, m_GridSize];
        float tileSize = m_TileParent.GetComponent<RectTransform>().rect.height / m_GridSize;

        for (int y = 0; y < m_GridSize; y++)
        {
            for (int x = 0; x < m_GridSize; x++)
            {
                Tile tile = Instantiate(m_TilePrefab, m_TileParent);
                tile.Init(tileSize, new Vector2Int(x, y));
                tile.OnTileSelected += TileSelected;
                m_Grid[x, y] = tile;
            }
        }

        m_Grid[0, 0].Open(Direction.Down);
        m_Grid[m_GridSize - 1, m_GridSize - 1].Open(Direction.Up);
        GenerateMaze();

        m_Player = Instantiate(m_PlayerPrefab, m_TileParent);
        m_Player.Init(tileSize, m_Grid);
    }

    private void ResetObjects()
    {
        if (m_Player == null) return;
        
        Destroy(m_Player);

        foreach (Transform item in m_TileParent)
        {
            Destroy(item.gameObject);
        }
    }

    private void GenerateMaze()
    {
        int x = 0;
        int y = 0;
        m_Grid[x, y].Visited = true;
        Stack<Tile> stack = new Stack<Tile>();
        stack.Push(m_Grid[x, y]);
        List<Direction> lstDirections = GetAllDirections();

        while (stack.Count > 0)
        {
            if (lstDirections.Count == 0)
            {
                stack.Pop();
                lstDirections = GetAllDirections();

                if (stack.Count > 0)
                {
                    x = stack.Peek().Coordinates.x;
                    y = stack.Peek().Coordinates.y;
                }
            }

            int randomIndex = Random.Range(0, lstDirections.Count);
            Direction randomDirection = lstDirections[randomIndex];
            lstDirections.RemoveAt(randomIndex);

            switch (randomDirection)
            {
                case Direction.Up:
                    if (y < m_GridSize - 1 && !m_Grid[x, y + 1].Visited)
                    {
                        m_Grid[x, y].Open(Direction.Up);
                        y++;
                        m_Grid[x, y].Open(Direction.Down);
                        m_Grid[x, y].Visited = true;
                        stack.Push(m_Grid[x, y]);
                        lstDirections = GetAllDirections();
                    }
                    break;
                case Direction.Down:
                    if (y > 0 && !m_Grid[x, y - 1].Visited)
                    {
                        m_Grid[x, y].Open(Direction.Down);
                        y--;
                        m_Grid[x, y].Open(Direction.Up);
                        m_Grid[x, y].Visited = true;
                        stack.Push(m_Grid[x, y]);
                        lstDirections = GetAllDirections();
                    }
                    break;
                case Direction.Left:
                    if (x > 0 && !m_Grid[x - 1, y].Visited)
                    {
                        m_Grid[x, y].Open(Direction.Left);
                        x--;
                        m_Grid[x, y].Open(Direction.Right);
                        m_Grid[x, y].Visited = true;
                        stack.Push(m_Grid[x, y]);
                        lstDirections = GetAllDirections();
                    }
                    break;
                case Direction.Right:
                    if (x < m_GridSize - 1 && !m_Grid[x + 1, y].Visited)
                    {
                        m_Grid[x, y].Open(Direction.Right);
                        x++;
                        m_Grid[x, y].Open(Direction.Left);
                        m_Grid[x, y].Visited = true;
                        stack.Push(m_Grid[x, y]);
                        lstDirections = GetAllDirections();
                    }
                    break;
            }
        }
    }

    private List<Direction> GetAllDirections()
    {
        return new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
    }

    private void TileSelected(Vector2Int coordinates)
    {
        m_Player.Move(coordinates);
    }
}
