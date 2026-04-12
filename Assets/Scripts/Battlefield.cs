using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Battlefield : MonoBehaviour
{
    [SerializeField]
    private CellPalletteSettings _settings;

    private Cell[] _allCells;
    private List<Unit> _allUnits = new List<Unit>();

    private Team _currentTurn = Team.White;
    private Unit _selectedUnit = null;
    private List<Cell> _availableMoves = new List<Cell>();

    public event Action<Unit> OnUnitSelected;
    public event Action<List<Cell>> OnAvailableMovesFound;
    public event Action OnSelectionCleared;

    public event Action<Cell> OnMoveExecuted;
    public event Action<Team> OnTurnChanged;
    public event Action<Cell> OnCellSelected;
    public event Action<string> OnGameOver;

    private Cell _selectedTargetCell = null;

    private void Awake()
    {
        FindAllCells();
        FindAllUnits();
        SetupNeighbours();
        SetupUnitsOnCells();
        SubscribeToCellClicks();
        SubscribeToUnitClicks();
    }

    private void FindAllCells()
    {
        _allCells = FindObjectsOfType<Cell>();
        Debug.Log($"Found {_allCells.Length} cells");

        foreach (Cell cell in _allCells)
        {
            cell.BoardX = Mathf.RoundToInt(cell.transform.position.x);
            cell.BoardZ = Mathf.RoundToInt(cell.transform.position.z);
            Debug.Log($"Cell {cell.name}: Board=({cell.BoardX},{cell.BoardZ})");
        }
    }

    private void FindAllUnits()
    {
        _allUnits = FindObjectsOfType<Unit>().ToList();
        Debug.Log($"Found {_allUnits.Count} units");
    }

    private void SetupNeighbours()
    {
        float step = 2f;
        float threshold = 2.1f;

        foreach (Cell cell in _allCells)
        {
            Vector3 pos = cell.transform.position;

            foreach (Cell other in _allCells)
            {
                if (cell == other) continue;

                Vector3 delta = other.transform.position - pos;

                if (Mathf.Abs(delta.x) <= threshold && Mathf.Abs(delta.z) <= threshold &&
                    Mathf.Abs(delta.x) > 0.1f && Mathf.Abs(delta.z) > 0.1f)
                {
                    NeighbourType type = GetNeighbourType(delta);
                    if (type != NeighbourType.None)
                    {
                        cell.SetNeighbour(type, other);
                    }
                }
            }
        }
    }

    private void SetupUnitsOnCells()
    {
        foreach (Unit unit in _allUnits)
        {
            bool found = false;

            foreach (Cell cell in _allCells)
            {
                Vector3 cellWorldPos = cell.transform.position;
                Vector3 unitWorldPos = unit.transform.position;

                float deltaX = Mathf.Abs(unitWorldPos.x - cellWorldPos.x);
                float deltaZ = Mathf.Abs(unitWorldPos.z - cellWorldPos.z);

                if (deltaX < 0.5f && deltaZ < 0.5f)
                {
                    unit.SetCurrentCell(cell);
                    cell.SetUnit(unit);

                    Vector3 newPos = unit.transform.position;
                    newPos.y = 1.5f;
                    unit.transform.position = newPos;

                    int boardX = Mathf.RoundToInt(cellWorldPos.x);
                    int boardZ = Mathf.RoundToInt(cellWorldPos.z);

                    Debug.Log($"{unit.name} assigned to cell at world position ({cellWorldPos.x}, {cellWorldPos.z}) -> Board ({boardX}, {boardZ})");
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogError($"{unit.name} at world position ({unit.transform.position.x}, {unit.transform.position.z}) has no matching cell!");
            }
        }
    }

    private NeighbourType GetNeighbourType(Vector3 delta)
    {
        int forward = (int)Mathf.Sign(delta.z);
        int right = (int)Mathf.Sign(delta.x);

        return (forward, right) switch
        {
            (1, 1) => NeighbourType.ForwardRight,
            (1, -1) => NeighbourType.ForwardLeft,
            (-1, 1) => NeighbourType.BackwardRight,
            (-1, -1) => NeighbourType.BackwardLeft,
            _ => NeighbourType.None
        };
    }

    public void SelectTargetCell(Cell cell)
    {
        if (_selectedUnit == null) return;

        if (_availableMoves.Contains(cell))
        {
            if (_selectedTargetCell != null)
            {
                _selectedTargetCell.SetSelect(_settings.MoveCell);
            }

            _selectedTargetCell = cell;
            _selectedTargetCell.SetSelect(_settings.SelectCell);
            Debug.Log($"Target cell selected: {cell.name}, press Space to confirm");
        }
    }

    public void ClearTargetSelection()
    {
        if (_selectedTargetCell != null)
        {
            if (_availableMoves.Contains(_selectedTargetCell))
            {
                _selectedTargetCell.SetSelect(_settings.MoveCell);
            }
            else
            {
                _selectedTargetCell.ResetSelect();
            }
            _selectedTargetCell = null;
        }
    }

    private void SubscribeToUnitClicks()
    {
        foreach (Unit unit in _allUnits)
        {
            unit.OnUnitClicked += OnUnitClicked;
        }
    }

    private void SubscribeToCellClicks()
    {
        foreach (Cell cell in _allCells)
        {
            cell.OnCellClicked += OnCellClicked;
        }
    }

    private void OnCellClicked(Cell cell)
    {
        OnCellSelected?.Invoke(cell);

        Debug.Log($"OnCellClicked: cell={cell.name}, Board=({cell.BoardX},{cell.BoardZ})");
        Debug.Log($"Available moves count: {_availableMoves.Count}");

        if (_selectedUnit != null && _selectedUnit.CurrentCell == cell)
        {
            Debug.Log("Clicked on selected unit's cell - ignoring");
            return;
        }

        if (_availableMoves.Contains(cell))
        {
            Debug.Log($"Cell {cell.name} is available move. Press Space to confirm.");
        }
        else if (_selectedUnit != null)
        {
            ClearSelection();
        }
    }

    private void OnUnitClicked(Unit unit)
    {
        Debug.Log($"=== OnUnitClicked called for {unit.name} ===");
        Debug.Log($"Unit Team: {unit.Team}, Current Turn: {_currentTurn}");

        if (unit.Team != _currentTurn) return;
        if (unit.IsMoving) return;
        
        if (_selectedUnit != null && _selectedUnit != unit)
        {
            Debug.Log($"Cannot change selected unit! {_selectedUnit.name} must move first.");
            return;
        }

        if (_selectedUnit == unit)
        {
            ClearSelection();
            return;
        }

        ClearAllHighlights();

        _selectedUnit = unit;
        _availableMoves = GetAvailableMoves(unit);

        OnUnitSelected?.Invoke(unit);
        OnAvailableMovesFound?.Invoke(_availableMoves);
    }

    public void ClearAllHighlights()
    {
        if (_selectedUnit != null && _selectedUnit.CurrentCell != null)
        {
            _selectedUnit.CurrentCell.ResetSelect();
        }

        foreach (Cell cell in _availableMoves)
        {
            cell.ResetSelect();
        }

        OnSelectionCleared?.Invoke();
    }

    public void ClearSelection()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.ResetSelect();
            _selectedUnit.CurrentCell.ResetSelect();
        }

        foreach (Cell cell in _availableMoves)
        {
            cell.ResetSelect();
        }

        ClearTargetSelection();
        ClearAllHighlights();
        _selectedUnit = null;
        _availableMoves.Clear();
    }

    private List<Cell> GetAvailableMoves(Unit unit)
    {
        List<Cell> moves = new List<Cell>();
        Cell currentCell = unit.CurrentCell;

        if (unit.Power == UnitPower.Queen)
        {
            NeighbourType[] directions = new NeighbourType[]
            {
            NeighbourType.ForwardRight,
            NeighbourType.ForwardLeft,
            NeighbourType.BackwardRight,
            NeighbourType.BackwardLeft
            };

            foreach (NeighbourType dir in directions)
            {
                Cell currentCheckCell = currentCell;
                bool blocked = false;

                while (!blocked)
                {
                    Cell nextCell = currentCheckCell.GetNeighbour(dir);
                    if (nextCell == null) break;

                    if (nextCell.Unit == null)
                    {
                        moves.Add(nextCell);
                        currentCheckCell = nextCell;
                    }
                    else if (nextCell.Unit.Team != unit.Team)
                    {
                        Cell jumpCell = nextCell.GetNeighbour(dir);
                        if (jumpCell != null && jumpCell.Unit == null)
                        {
                            moves.Add(jumpCell);
                        }
                        blocked = true;
                    }
                    else
                    {
                        blocked = true;
                    }
                }
            }
        }
        else
        {
            NeighbourType[] forwardDirections;
            NeighbourType[] backwardDirections;

            if (unit.Team == Team.White)
            {
                forwardDirections = new NeighbourType[] { NeighbourType.ForwardRight, NeighbourType.ForwardLeft };
                backwardDirections = new NeighbourType[] { NeighbourType.BackwardRight, NeighbourType.BackwardLeft };
            }
            else
            {
                forwardDirections = new NeighbourType[] { NeighbourType.BackwardRight, NeighbourType.BackwardLeft };
                backwardDirections = new NeighbourType[] { NeighbourType.ForwardRight, NeighbourType.ForwardLeft };
            }

            foreach (NeighbourType dir in forwardDirections)
            {
                Cell neighbour = currentCell.GetNeighbour(dir);
                if (neighbour == null) continue;

                if (neighbour.Unit == null)
                {
                    moves.Add(neighbour);
                }
                else if (neighbour.Unit.Team != unit.Team)
                {
                    Cell jumpCell = neighbour.GetNeighbour(dir);
                    if (jumpCell != null && jumpCell.Unit == null)
                    {
                        moves.Add(jumpCell);
                    }
                }
            }

            foreach (NeighbourType dir in backwardDirections)
            {
                Cell neighbour = currentCell.GetNeighbour(dir);
                if (neighbour == null) continue;

                if (neighbour.Unit != null && neighbour.Unit.Team != unit.Team)
                {
                    Cell jumpCell = neighbour.GetNeighbour(dir);
                    if (jumpCell != null && jumpCell.Unit == null)
                    {
                        moves.Add(jumpCell);
                        Debug.Log($"Backward attack move added for {unit.name} in direction {dir}");
                    }
                }
            }
        }

        Debug.Log($"Total moves found for {unit.name} (Power={unit.Power}): {moves.Count}");
        return moves;
    }
    public void ExecuteMove(Cell targetCell)
    {
        if (_selectedUnit == null) return;

        _selectedTargetCell = null;

        Cell startCell = _selectedUnit.CurrentCell;

        int deltaX = Mathf.Abs(targetCell.BoardX - startCell.BoardX);
        int deltaZ = Mathf.Abs(targetCell.BoardZ - startCell.BoardZ);

        bool isAttack = false;
        Cell middleCell = null;

        if (_selectedUnit.Power == UnitPower.Queen)
        {
            if (deltaX == deltaZ && deltaX > 2)
            {
                int stepX = (targetCell.BoardX - startCell.BoardX) / Mathf.Abs(targetCell.BoardX - startCell.BoardX);
                int stepZ = (targetCell.BoardZ - startCell.BoardZ) / Mathf.Abs(targetCell.BoardZ - startCell.BoardZ);

                int currentX = startCell.BoardX + stepX;
                int currentZ = startCell.BoardZ + stepZ;

                while (currentX != targetCell.BoardX || currentZ != targetCell.BoardZ)
                {
                    Cell checkCell = GetCellByBoardPosition(currentX, currentZ);
                    if (checkCell != null && checkCell.Unit != null && checkCell.Unit.Team != _selectedUnit.Team)
                    {
                        isAttack = true;
                        middleCell = checkCell;
                        break;
                    }
                    currentX += stepX;
                    currentZ += stepZ;
                }
            }
        }
        else
        {
            isAttack = deltaX == 4 && deltaZ == 4;
            if (isAttack)
            {
                int midX = (startCell.BoardX + targetCell.BoardX) / 2;
                int midZ = (startCell.BoardZ + targetCell.BoardZ) / 2;
                middleCell = GetCellByBoardPosition(midX, midZ);
            }
        }

        Debug.Log($"ExecuteMove: start=({startCell.BoardX},{startCell.BoardZ}), target=({targetCell.BoardX},{targetCell.BoardZ}), isAttack={isAttack}");

        _selectedUnit.Move(targetCell, () =>
        {
            if (isAttack && middleCell != null && middleCell.Unit != null)
            {
                Debug.Log($"Destroying enemy: {middleCell.Unit.name}");
                Destroy(middleCell.Unit.gameObject);
                _allUnits.Remove(middleCell.Unit);
                middleCell.SetUnit(null);
            }
            else if (isAttack)
            {
                Debug.LogWarning($"Attack move but no enemy at middle cell!");
            }

            if (_selectedUnit.Power == UnitPower.None)
            {
                if (_selectedUnit.Team == Team.White && targetCell.BoardZ == 7)
                {
                    _selectedUnit.PromoteToQueen(_settings.WhiteQueenMaterial);
                    Debug.Log($"{_selectedUnit.name} promoted to White Queen!");
                }
                else if (_selectedUnit.Team == Team.Black && targetCell.BoardZ == -7)
                {
                    _selectedUnit.PromoteToQueen(_settings.BlackQueenMaterial);
                    Debug.Log($"{_selectedUnit.name} promoted to Black Queen!");
                }
            }

            OnMoveExecuted?.Invoke(targetCell);
            EndTurn();
        });
    }
    private void EndTurn()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.ResetSelect();
        }

        ClearSelection();
        _currentTurn = _currentTurn == Team.White ? Team.Black : Team.White;
        OnTurnChanged?.Invoke(_currentTurn);
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        bool hasWhiteUnits = _allUnits.Any(u => u.Team == Team.White);
        bool hasBlackUnits = _allUnits.Any(u => u.Team == Team.Black);

        if (!hasWhiteUnits)
        {
            Debug.Log("BLACK WINS!");
            OnGameOver?.Invoke("BLACK");
        }
        else if (!hasBlackUnits)
        {
            Debug.Log("WHITE WINS!");
            OnGameOver?.Invoke("WHITE");
        }
    }

    private Cell GetCellByBoardPosition(int x, int z)
    {
        foreach (Cell cell in _allCells)
        {
            if (cell.BoardX == x && cell.BoardZ == z)
                return cell;
        }
        return null;
    }

    public Team GetCurrentTurn() => _currentTurn;
    public Unit GetSelectedUnit() => _selectedUnit;
    public List<Cell> GetAvailableMoves() => _availableMoves;
    public List<Cell> GetAvailableMovesForUnit(Unit unit) => GetAvailableMoves(unit);
}