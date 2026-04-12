using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isProcessingMove = false;

    public bool IsProcessingMove => _isProcessingMove;

    public void StartMove()
    {
        _isProcessingMove = true;
    }

    public void EndMove()
    {
        _isProcessingMove = false;
    }
}
