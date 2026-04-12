using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField]
    private Battlefield _battlefield;
    [SerializeField]
    private SceneController _sceneController;
    [SerializeField]
    private InputManager _inputManager;
    [SerializeField]
    private BattleController _battleController;
    [SerializeField]
    private PlayerController _playerController;
    [SerializeField]
    private CellPalletteSettings _cellPalletteSettings;

    public override void InstallBindings()
    {
        Controls controls = new Controls();
        controls.Game.Enable();
        Container.Bind<Controls.GameActions>().FromInstance(controls.Game).AsSingle();

        Container.Bind<Battlefield>().FromInstance(_battlefield).AsSingle();
        Container.Bind<SceneController>().FromInstance(_sceneController).AsSingle();
        Container.Bind<InputManager>().FromInstance(_inputManager).AsSingle();
        Container.Bind<BattleController>().FromInstance(_battleController).AsSingle();
        Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
        Container.Bind<CellPalletteSettings>().FromInstance(_cellPalletteSettings).AsSingle();

        _battlefield.OnUnitSelected += OnUnitSelected;
        _battlefield.OnAvailableMovesFound += OnAvailableMovesFound;
        _battlefield.OnSelectionCleared += OnSelectionCleared;
    }

    private void OnUnitSelected(Unit unit)
    {
        Debug.Log($"Unit selected: {unit.name}, highlighting unit");
        unit.SetSelect(_cellPalletteSettings.SelectUnit);  
    }

    private void OnAvailableMovesFound(List<Cell> availableMoves)
    {
        Debug.Log($"Found {availableMoves.Count} available moves, highlighting them");
        foreach (Cell cell in availableMoves)
        {
            cell.SetSelect(_cellPalletteSettings.MoveCell);
        }
    }

    private void OnSelectionCleared()
    {
        Debug.Log("Clearing all highlights");
    }

    private void OnDestroy()
    {
        if (_battlefield != null)
        {
            _battlefield.OnUnitSelected -= OnUnitSelected;
            _battlefield.OnAvailableMovesFound -= OnAvailableMovesFound;
            _battlefield.OnSelectionCleared -= OnSelectionCleared;
        }
    }
}