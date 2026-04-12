public class ConfirmCommand : IGameplayCommand
{
    private Battlefield _battlefield;
    private Cell _selectedCell;

    public ConfirmCommand(Battlefield battlefield, Cell selectedCell)
    {
        _battlefield = battlefield;
        _selectedCell = selectedCell;
    }

    public bool CanExecute()
    {
        if (_battlefield.GetSelectedUnit() == null) return false;
        if (_selectedCell == null) return false;
        return _battlefield.GetAvailableMoves().Contains(_selectedCell);
    }

    public void Execute()
    {
        if (!CanExecute()) return;
        _battlefield.ExecuteMove(_selectedCell);
    }

    public void Undo() { }
}