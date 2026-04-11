using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField]
    private float _speed = 2f;

    private Cell _currentCell;
    private bool _isMoving = false;

    public Cell CurrentCell => _currentCell;
    public event Action<Unit> OnMoveEndCallback;

    public void SetCurrentCell(Cell cell)
    {
        _currentCell = cell;
        transform.position = cell.transform.position;
    }

    public void Move(Cell cell)
    {
        if (_isMoving) return;
        if (cell == null) return;
        if (cell == _currentCell) return;

        StartCoroutine(MoveRoutine(cell));
    }

    private IEnumerator MoveRoutine(Cell targetCell)
    {
        _isMoving = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = targetCell.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float timeNeeded = distance / _speed;
        float time = 0f;

        // Очищаем связь со старой клеткой
        if (_currentCell != null)
        {
            _currentCell.SetUnit(null);
        }

        // Плавное перемещение
        while (time < timeNeeded)
        {
            float t = time / timeNeeded;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }

        // Фиксируем конечную позицию
        transform.position = targetPosition;

        // Устанавливаем связь с новой клеткой
        _currentCell = targetCell;
        _currentCell.SetUnit(this);

        _isMoving = false;

        // Вызываем событие окончания движения
        OnMoveEndCallback?.Invoke(this);
    }

    // Прокидываем вызовы в клетку
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentCell != null)
        {
            _currentCell.OnPointerClick(eventData);
        }
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