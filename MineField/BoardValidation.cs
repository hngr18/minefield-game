namespace MineField;

public static class BoardValidation
{
    /// <summary>
    /// Whether it is possible to traverse from the start to the end of the board without hitting a mine
    /// </summary>
    public static bool BoardIsValid(IEnumerable<Cell[]> cells)
    {
        var safeCells = cells.SelectMany(row => 
            row.Where(cell => !cell.HasMine).Select(cell => cell.Location)
        ).ToArray();

        var traversalStarts = Range(0, DefaultColCount).Select(colIx => 
            (col :(char)(colIx + CharIntOffset), row: 0)
        ).ToArray();

        foreach (var start in traversalStarts)
        {
            var possibleLocations = safeCells.Where(CellIsAdjacent(start)).ToHashSet();
            
            foreach (var _ in Range(1, 1000))
            {
                foreach (var cell in possibleLocations.SelectMany(possibleLocation => safeCells.Where(CellIsAdjacent(possibleLocation))).ToArray())
                {
                    possibleLocations.Add(cell);
                }
            }

            // no winning tile locations accessible from start
            if (possibleLocations.All(location => location.row != DefaultRowCount - 1))
                return false;
        }

        return true;
    }
    
    public static Func<(char col, int row), bool> CellIsAdjacent((char col, int row) start)
    {
        return cell => 
            (cell.col == start.col && cell.row == start.row + 1) || // move row
            (cell.row == start.row && cell.col == start.col - 1) || // move left
            (cell.row == start.row && cell.col == start.col + 1); // move right
    }
}