using UnityEngine;

public class CancelCommand : IGameplayCommand
{
    private Battlefield _battlefield;

    public CancelCommand(Battlefield battlefield)
    {
        _battlefield = battlefield;
    }

    public bool CanExecute()
    {
        return _battlefield.GetSelectedUnit() != null ||
               _battlefield.GetAvailableMoves().Count > 0;
    }

    public void Execute()
    {
        if (!CanExecute()) return;
        _battlefield.ClearTargetSelection();
        _battlefield.ClearSelection();
        Debug.Log("Selection cancelled");
    }

    public void Undo() { }
}