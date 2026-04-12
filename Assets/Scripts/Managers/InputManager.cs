using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class InputManager : MonoBehaviour
{
    private SceneController _sceneController;      
    private Controls.GameActions _controls;        

    private Coroutine _restartCoroutine;

    [SerializeField]
    private GameObject _restartUI;

    [SerializeField]
    private Image _restartFill;

    [SerializeField, Range(0.1f, 1f)]
    private float _restartPushInSec = 0.25f;

    private void OnRestartPerformed(InputAction.CallbackContext obj)
    {
        _restartUI.SetActive(true);
        
        _restartCoroutine = StartCoroutine(Restarter());
    }

    private void OnRestartCanceled(InputAction.CallbackContext obj)
    {
        StopCoroutine(_restartCoroutine);

        _restartFill.fillAmount = 0f;

        _restartUI.SetActive(false);
    }

    private IEnumerator Restarter()
    {
        float elapsed = 0f;
        while (elapsed < _restartPushInSec)
        {
            elapsed += Time.deltaTime;
            _restartFill.fillAmount = elapsed / _restartPushInSec;
            yield return null;
        }

        _sceneController.RestartScene();
    }

    private void Start()
    {
        _controls.Restart.performed += OnRestartPerformed;
        _controls.Restart.canceled += OnRestartCanceled;
        _restartFill.fillAmount = 0f;
        _restartUI.SetActive(false);
    }

    private void OnDestroy()
    {
        _controls.Restart.performed -= OnRestartPerformed;
        _controls.Restart.canceled -= OnRestartCanceled;
    }

    [Inject]
    private void Construct(SceneController sceneController, Controls.GameActions controls)
    {
        _sceneController = sceneController;
        _controls = controls;
    }
}
