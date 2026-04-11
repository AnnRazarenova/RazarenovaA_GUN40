using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CellManager : MonoBehaviour
{
    private Cell[] _cells;
    private Unit[] _units;

    public event Action<Cell> OnCellClicked;

    private void Awake()
    {
        FindAllCells();
        FindAllUnits();
        SetupNeighbours();
        SubscribeToCellClicks();
        SetupUnitsOnCells();
    }

    private void FindAllCells()
    {
        _cells = FindObjectsOfType<Cell>();
        Debug.Log($"Found {_cells.Length} cells");
    }

    private void FindAllUnits()
    {
        _units = FindObjectsOfType<Unit>();
        Debug.Log($"Found {_units.Length} units");
    }

    private void SetupNeighbours()
    {
        foreach (Cell cell in _cells)
        {
            Vector3 pos = cell.transform.position;

            foreach (Cell other in _cells)
            {
                if (cell == other) continue;

                Vector3 otherPos = other.transform.position;
                Vector3 delta = otherPos - pos;

                // Проверяем соседство (расстояние 1 по X и Z)
                if (Mathf.Abs(delta.x) <= 1.1f && Mathf.Abs(delta.z) <= 1.1f)
                {
                    NeighbourType type = GetNeighbourType(delta);
                    if (type != NeighbourType.None)
                    {
                        cell.SetNeighbour(type, other);
                    }
                }
            }
        }
    }

    private NeighbourType GetNeighbourType(Vector3 delta)
    {
        int forward = (int)Mathf.Sign(delta.z);
        int right = (int)Mathf.Sign(delta.x);

        return (forward, right) switch
        {
            (1, 1) => NeighbourType.ForwardRight,
            (1, 0) => NeighbourType.Forward,
            (1, -1) => NeighbourType.ForwardLeft,
            (0, 1) => NeighbourType.Right,
            (0, -1) => NeighbourType.Left,
            (-1, 1) => NeighbourType.BackwardRight,
            (-1, 0) => NeighbourType.Backward,
            (-1, -1) => NeighbourType.BackwardLeft,
            _ => NeighbourType.None
        };
    }

    private void SubscribeToCellClicks()
    {
        foreach (Cell cell in _cells)
        {
            cell.OnPointerClickEvent += OnCellClicked;
        }
    }

    private void SetupUnitsOnCells()
    {
        foreach (Unit unit in _units)
        {
            // Находим клетку, на которой стоит юнит
            foreach (Cell cell in _cells)
            {
                if (Vector3.Distance(unit.transform.position, cell.transform.position) < 0.1f)
                {
                    unit.SetCurrentCell(cell);
                    cell.SetUnit(unit);
                    break;
                }
            }
        }
    }
}