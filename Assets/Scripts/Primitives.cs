public enum NeighbourType
{
    None = 0,
    Left = 1,
    Right = 2,
    Forward = 4,
    Backward = 8,
    ForwardLeft = Left | Forward,
    BackwardLeft = Left | Backward,
    ForwardRight = Right | Forward,
    BackwardRight = Right | Backward
}

/// <summary>
/// Игровые команды
/// </summary>
public enum Team
{
    None = 0,
    Player1 = 1,
    Player2 = 2
}
