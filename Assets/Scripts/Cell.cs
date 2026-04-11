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

    private Unit _unit;
    private Dictionary<NeighbourType, Cell> _neighbours = new Dictionary<NeighbourType, Cell>();

    public Unit Unit => _unit;

    public Action<Cell> OnPointerClickEvent;

    public void SetSelect(Material material) 
        => (_select.enabled, _select.sharedMaterial) = (true, material);

    public void ResetSelect() 
        => _select.enabled = false;

    public void OnPointerClick(PointerEventData eventData)
        => OnPointerClickEvent.Invoke(this);

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


}
