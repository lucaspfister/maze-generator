using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image m_BorderUp;
    [SerializeField] Image m_BorderDown;
    [SerializeField] Image m_BorderLeft;
    [SerializeField] Image m_BorderRight;

    public Vector2Int Coordinates { get; private set; }
    public bool Visited { get; set; }

    public event Action<Vector2Int> OnTileSelected;

    private const float BORDER_SCALE = 0.20f;

    public void Init(float size, Vector2Int coordinates)
    {
        Coordinates = coordinates;
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = new Vector2(size * coordinates.x, size * coordinates.y);

        float borderWidth = size * BORDER_SCALE;
        float borderSize = size + borderWidth;
        m_BorderUp.GetComponent<RectTransform>().sizeDelta = new Vector2(borderSize, borderWidth);
        m_BorderDown.GetComponent<RectTransform>().sizeDelta = new Vector2(borderSize, borderWidth);
        m_BorderLeft.GetComponent<RectTransform>().sizeDelta = new Vector2(borderWidth, borderSize);
        m_BorderRight.GetComponent<RectTransform>().sizeDelta = new Vector2(borderWidth, borderSize);
    }

    public void Open(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                m_BorderUp.enabled = false;
                break;
            case Direction.Down:
                m_BorderDown.enabled = false;
                break;
            case Direction.Left:
                m_BorderLeft.enabled = false;
                break;
            case Direction.Right:
                m_BorderRight.enabled = false;
                break;
        }
    }

    public bool IsOpen(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return !m_BorderUp.enabled;
            case Direction.Down:
                return !m_BorderDown.enabled;
            case Direction.Left:
                return !m_BorderLeft.enabled;
            case Direction.Right:
                return !m_BorderRight.enabled;
        }

        return false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTileSelected?.Invoke(Coordinates);
    }
}
