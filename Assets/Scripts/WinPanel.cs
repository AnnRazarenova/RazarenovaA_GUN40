using TMPro;
using UnityEngine;
using Zenject;

public class WinPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _winPanel;      
    [SerializeField] private TextMeshProUGUI _winText;  
                                                        

    [Header("Settings")]
    [SerializeField] private float _displayDuration = 5f; 

    [Inject] private Battlefield _battlefield;

    private void Start()
    {
        if (_winPanel != null)
            _winPanel.SetActive(false);

        _battlefield.OnGameOver += ShowWinner;
    }

    private void ShowWinner(string winner)
    {
        if (_winText != null)
        {
            _winText.text = $"{winner} WINS!";
        }

        if (_winPanel != null)
        {
            _winPanel.SetActive(true);
        }

        Debug.Log($"{winner} WINS! ");

    }

    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_displayDuration);
        if (_winPanel != null)
            _winPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_battlefield != null)
        {
            _battlefield.OnGameOver -= ShowWinner;
        }
    }
}
