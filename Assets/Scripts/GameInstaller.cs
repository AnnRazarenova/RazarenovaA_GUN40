using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    private Controls _controls;

    [SerializeField]
    private CellManager _cellManager;
    [SerializeField]
    private SceneController _controller;

    [SerializeField]
    private CellPalletteSettings _cellPalletteSettings;

    public override void InstallBindings()
    {
        _controls = new Controls();
        _controls.Game.Enable();
        Container.Bind<Controls.GameActions>().FromInstance(_controls.Game).AsSingle();

        Container.Bind<CellManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SceneController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<CellPalletteSettings>().FromInstance(_cellPalletteSettings).AsSingle();

        _cellManager.OnCellClicked += CellManagerOnOnCellClick;
    }

    private void CellManagerOnOnCellClick(Cell obj)
    {
        obj.SetSelect(_cellPalletteSettings.SelectCell);
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }
}
