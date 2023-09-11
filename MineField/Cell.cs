namespace MineField;

public record Cell((char col, int row) Location, bool HasMine, bool HasUser = false, bool IsVisited = false, bool MineRevealed = false)
{
    public bool HasUser { get; set; } = HasUser;
    public bool IsVisited { get; set; } = IsVisited;
    
    public bool MineRevealed { get; set; } = MineRevealed;
}