using UnityEngine;
using Zenject;

public class TurnIndicator : MonoBehaviour, ITurnIndicator
{
    [Header("Player Icons")]
    [SerializeField] private RectTransform _whiteIcon;
    [SerializeField] private RectTransform _blackIcon;

    [Header("Arrow")]
    [SerializeField] private RectTransform _arrow;

    [Header("Size Settings")]
    [SerializeField] private Vector2 _normalSize = new Vector2(100, 100);
    [SerializeField] private Vector2 _smallSize = new Vector2(80, 80);

    [Inject] private Battlefield _battlefield;

    private void Start()
    {
        if (_battlefield != null)
        {
            _battlefield.OnTurnChanged += SetTurn;
            SetTurn(Team.White);
        }
    }

    public void SetTurn(Team currentTurn)
    {
        if (currentTurn == Team.White)
        {
            HighlightWhite();
        }
        else
        {
            HighlightBlack();
        }
    }

    public void HighlightWhite()
    {
        _whiteIcon.sizeDelta = _normalSize;
        _blackIcon.sizeDelta = _smallSize;

        if (_arrow != null)
        {
            _arrow.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void HighlightBlack()
    {
        _whiteIcon.sizeDelta = _smallSize;
        _blackIcon.sizeDelta = _normalSize;

        if (_arrow != null)
        {
            _arrow.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void ResetHighlights()
    {
        _whiteIcon.sizeDelta = _normalSize;
        _blackIcon.sizeDelta = _normalSize;
    }

    private void OnDestroy()
    {
        if (_battlefield != null)
        {
            _battlefield.OnTurnChanged -= SetTurn;
        }
    }
}