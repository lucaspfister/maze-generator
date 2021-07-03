using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] float m_AnimDuration = 0.3f;
    [SerializeField] RectTransform m_PlayerImg;

    private RectTransform m_RectTransform;
    private Vector2Int m_Coordinates;
    private bool m_IsMoving = false;
    private Tile[,] m_Grid;

    private const float PLAYER_SCALE = 0.6f;

    public void Init(float size, Tile[,] grid)
    {
        m_Grid = grid;
        m_RectTransform = GetComponent<RectTransform>();
        m_RectTransform.sizeDelta = new Vector2(size, size);
        float playerSize = size * PLAYER_SCALE;
        m_PlayerImg.sizeDelta = new Vector2(playerSize, playerSize);
        m_Coordinates = new Vector2Int(0, 0);
    }

    public void Move(Vector2Int coordinates)
    {
        if (m_IsMoving || coordinates == m_Coordinates) return;

        m_IsMoving = true;
        Direction[] directions = PathFind(m_Coordinates, ref coordinates);
        int x = m_Coordinates.x;
        int y = m_Coordinates.y;
        float size = m_RectTransform.sizeDelta.x;
        Sequence sequence = DOTween.Sequence();

        for (int i = directions.Length - 1; i >= 0; i--)
        {
            switch (directions[i])
            {
                case Direction.Up:
                    y++;
                    break;
                case Direction.Down:
                    y--;
                    break;
                case Direction.Left:
                    x--;
                    break;
                case Direction.Right:
                    x++;
                    break;
            }

            var anim = m_RectTransform.DOAnchorPos(new Vector2(size * x, size * y), m_AnimDuration).SetEase(Ease.Linear);

            if (i == 0)
            {
                anim.OnComplete(() => m_IsMoving = false);
            }

            sequence.Append(anim);
        }

        sequence.Play();
        m_Coordinates = coordinates;
    }

    private Direction[] PathFind(Vector2Int start, ref Vector2Int end)
    {
        int gridLength = m_Grid.GetLength(0);
        bool[,] visited = new bool[gridLength, gridLength];
        visited[start.x, start.y] = true;
        Stack<Direction> directions = new Stack<Direction>();
        NextDirection(start, ref end, visited, directions);

        return directions.ToArray();
    }

    private bool NextDirection(Vector2Int start, ref Vector2Int end, bool[,] visited, Stack<Direction> directions)
    {
        if (start == end) return true;

        //Up
        start.y++;
        if (start.y < visited.GetLength(0) && m_Grid[start.x, start.y].IsOpen(Direction.Down) && !visited[start.x, start.y])
        {
            visited[start.x, start.y] = true;
            directions.Push(Direction.Up);

            if (NextDirection(start, ref end, visited, directions)) return true;
        }
        start.y--;

        //Down
        start.y--;
        if (start.y >= 0 && m_Grid[start.x, start.y].IsOpen(Direction.Up) && !visited[start.x, start.y])
        {
            visited[start.x, start.y] = true;
            directions.Push(Direction.Down);

            if (NextDirection(start, ref end, visited, directions)) return true;
        }
        start.y++;

        //Left
        start.x--;
        if (start.x >= 0 && m_Grid[start.x, start.y].IsOpen(Direction.Right) && !visited[start.x, start.y])
        {
            visited[start.x, start.y] = true;
            directions.Push(Direction.Left);

            if (NextDirection(start, ref end, visited, directions)) return true;
        }
        start.x++;

        //Right
        start.x++;
        if (start.x < visited.GetLength(0) && m_Grid[start.x, start.y].IsOpen(Direction.Left) && !visited[start.x, start.y])
        {
            visited[start.x, start.y] = true;
            directions.Push(Direction.Right);

            if (NextDirection(start, ref end, visited, directions)) return true;
        }
        start.x--;

        directions.Pop();
        return false;
    }
}
