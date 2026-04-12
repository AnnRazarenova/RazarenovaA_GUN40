public interface IGameplayCommand
{
    void Execute();
    bool CanExecute();
    void Undo();
}
