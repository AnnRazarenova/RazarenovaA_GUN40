using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField]
    private float _speed = 2f;

    [SerializeField]
    private MeshRenderer _unitRenderer;

    private Cell _currentCell;
    [SerializeField]
    private Team _team;
    private UnitPower _power = UnitPower.None;
    private bool _isMoving = false;

    public Cell CurrentCell => _currentCell;
    public Team Team => _team;
    public UnitPower Power => _power;
    public bool IsMoving => _isMoving;

    private Material _originalMaterial;

    public event Action<Unit> OnMoveEnd;
    public event Action<Unit> OnUnitClicked;

    public int CurrentCellX { get; private set; }
    public int CurrentCellZ { get; private set; }


    private void Awake()
    {
        _originalMaterial = _unitRenderer.material;
        _power = UnitPower.None;
    }

    public void SetSelect(Material material)
    {
        Debug.Log($"Unit SetSelect called for {name}, material: {material?.name}");

        if (_unitRenderer == null)
        {
            Debug.LogError($"Unit {name} has no _unitRenderer");
            return;
        }

        _unitRenderer.material = material;
    }

    public void ResetSelect()
    {
        if (_unitRenderer != null && _originalMaterial != null)
        {
            _unitRenderer.material = _originalMaterial;
        }
    }

    public void PromoteToQueen(Material queenMaterial)
    {
        _power = UnitPower.Queen;
        _unitRenderer.material = queenMaterial;
        _originalMaterial = queenMaterial;
    }

    public void SetCurrentCell(Cell cell)
    {
        _currentCell = cell;
        CurrentCellX = cell.BoardX;
        CurrentCellZ = cell.BoardZ;

        Vector3 newPos = cell.transform.position;
        newPos.y = 1.5f;
        transform.position = newPos;
    }

    public void Move(Cell targetCell, Action onComplete = null)
    {
        if (_isMoving) return;
        if (targetCell == null) return;
        if (targetCell == _currentCell) return;

        StartCoroutine(MoveRoutine(targetCell, onComplete));
    }

    private IEnumerator MoveRoutine(Cell targetCell, Action onComplete)
    {
        _isMoving = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = targetCell.transform.position;
        targetPosition.y = 1.5f;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float timeNeeded = distance / _speed;
        float time = 0f;

        if (_currentCell != null)
        {
            _currentCell.SetUnit(null);
        }

        while (time < timeNeeded)
        {
            float t = time / timeNeeded;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        _currentCell = targetCell; 
        CurrentCellX = targetCell.BoardX;
        CurrentCellZ = targetCell.BoardZ;
        _currentCell.SetUnit(this);

        _isMoving = false;

        OnMoveEnd?.Invoke(this);
        onComplete?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnUnitClicked?.Invoke(this);
        if (_currentCell != null)
            _currentCell.OnPointerClick(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentCell != null)
        {
            _currentCell.OnPointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentCell != null)
        {
            _currentCell.OnPointerExit(eventData);
        }
    }
}