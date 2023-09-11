namespace MineField;

public record Game(string Username, (char Column, int Row) StartPosition, double MineProbability = 50, int Lives = DefaultLives)
{
    public Board Board { get; init; } = new(StartPosition, MineProbability);

    public (char Column, int Row) UserPosition { get; init; } = StartPosition;

    public bool InProgress { get; set; } = true;
    
    public bool Won { get; set; } = false;
}