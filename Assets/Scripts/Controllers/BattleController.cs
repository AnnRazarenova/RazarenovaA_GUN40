using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BattleController : MonoBehaviour
{
    [Inject] private Battlefield _battlefield;
    [Inject] private Controls.GameActions _controls;

    private IGameplayCommand _currentCommand;
    private Cell _lastSelectedCell;

    private void OnEnable()
    {
        _controls.Cansel.performed += OnCancelPerformed;
        _controls.Confirm.performed += OnConfirmPerformed;
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Cansel.performed -= OnCancelPerformed;
        _controls.Confirm.performed -= OnConfirmPerformed;
    }

    private void Start()
    {
        _battlefield.OnCellSelected += OnCellSelected;
    }

    private void OnCellSelected(Cell cell)
    {
        _lastSelectedCell = cell;
        Debug.Log($"Cell selected: {cell.name}");

        _battlefield.SelectTargetCell(cell);
    }

    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("Cancel - clear selection");
        _currentCommand = new CancelCommand(_battlefield);
        _currentCommand.Execute();
    }

    private void OnConfirmPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("Confirm - execute move");

        if (_battlefield.GetSelectedUnit() != null && _lastSelectedCell != null)
        {
            var availableMoves = _battlefield.GetAvailableMoves();
            if (availableMoves.Contains(_lastSelectedCell))
            {
                _currentCommand = new ConfirmCommand(_battlefield, _lastSelectedCell);
                _currentCommand.Execute();
            }
        }
    }


    private void OnDestroy()
    {
        if (_battlefield != null)
        {
            _battlefield.OnCellSelected -= OnCellSelected;
        }
    }
}