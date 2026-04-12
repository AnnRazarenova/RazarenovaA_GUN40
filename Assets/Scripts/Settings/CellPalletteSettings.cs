using UnityEngine;

[CreateAssetMenu(fileName = "New CellPalletteSettings", menuName = "Settings/CellPalletteSettings", order = 52)]
public class CellPalletteSettings : ScriptableObject
{
    [field: SerializeField, Space(20f)]
    [field: Tooltip("Клетка под выбранным юнитом")]
    public Material SelectCell {  get; private set; }

    [field: SerializeField]
    [field: Tooltip("Клетка доступная для передвижения")]
    public Material MoveCell { get; private set; }
    
    [field: SerializeField]
    [field: Tooltip("Выбранная фишка")]
    public Material SelectUnit { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Простая белая фишка")]
    public Material WhiteUnitMaterial;

    [field: SerializeField]
    [field: Tooltip("Простая чёрная фишка")]
    public Material BlackUnitMaterial;

    [field: SerializeField]
    [field: Tooltip("Дамка белая")]
    public Material WhiteQueenMaterial;

    [field: SerializeField]
    [field: Tooltip("Дамка чёрная")]
    public Material BlackQueenMaterial;
}
