using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField]
    private MeshRenderer _focus;

    [SerializeField]
    private MeshRenderer _select;

    [SerializeField] 
    private MeshRenderer _cellRenderer;

    private Unit _unit;
    private Dictionary<NeighbourType, Cell> _neighbours = new Dictionary<NeighbourType, Cell>();

    public int BoardX { get; set; }
    public int BoardZ { get; set; }

    public bool IsBlackCell { get; private set; }

    public Unit Unit => _unit;

    public event Action<Cell> OnCellClicked;

    public void SetSelect(Material material)
    {
        
        _select.enabled = true;
        _select.sharedMaterial = material;
    }

    public void SetSelectMaterial(Material material)
    {
        if (_select == null) return;
        if (material == null) return;

        _select.sharedMaterial = material;
    }

    public void ResetSelect() 
        => _select.enabled = false;

    public void OnPointerClick(PointerEventData eventData)
        => OnCellClicked?.Invoke(this);

    public void OnPointerEnter(PointerEventData eventData)
    {
        _focus.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _focus.enabled = false;
    }

    public void SetUnit(Unit unit)
    {
        _unit = unit;
    }

    public void SetNeighbour(NeighbourType type, Cell cell)
    {
        if (cell == null) return;

        if (_neighbours.ContainsKey(type))
        {
            Debug.LogWarning($"Cell {name} already has neighbour {type} -> {_neighbours[type].name}. Overwriting with {cell.name}");
        }

        _neighbours[type] = cell;
    }

    public Cell GetNeighbour(NeighbourType type)
    {
        _neighbours.TryGetValue(type, out Cell cell);
        return cell;
    }

    public IEnumerable<KeyValuePair<NeighbourType, Cell>> GetAllNeighbours()
    {
        return _neighbours;
    }

    public void Initialize(int x, int z, bool isBlack, Material cellMaterial)
    {
        BoardX = x;
        BoardZ = z;
        IsBlackCell = isBlack;
        _cellRenderer.material = cellMaterial;
    }

}
