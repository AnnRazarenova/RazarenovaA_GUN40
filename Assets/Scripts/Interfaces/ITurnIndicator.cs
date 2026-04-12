public interface ITurnIndicator
{
    void SetTurn(Team currentTurn);
    void HighlightWhite();
    void HighlightBlack();
    void ResetHighlights();
}
