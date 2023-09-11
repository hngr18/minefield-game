using static System.ConsoleKey;

namespace MineField;

public class Board
{
    private readonly int _rowCount, _colCount;

    public readonly Cell[][] Cells;

    /// <summary />
    /// <param name="startPosition"></param>
    /// <param name="mineProbability">Probability any given cell has a mine, expressed as a percentage</param>
    /// <param name="rowCount">Number of rows, linear distance "Y" from start to finish</param>
    /// <param name="colCount">Number of column, width "X", possible </param>
    public Board((char Column, int Row) startPosition, double mineProbability = 35, int rowCount = DefaultRowCount,
        int colCount = DefaultColCount)
    {
        if (mineProbability is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(mineProbability), "0% - 100% expected");
        
        if (startPosition.Column < 64 || startPosition.Column > 64 + colCount)
            throw new ArgumentOutOfRangeException(nameof(startPosition.Column), $"{(char)CharIntOffset}-{(char)(CharIntOffset + colCount)} expected");
        
        var mineGenerator = new MineGenerator(mineProbability);
        
        (_colCount, _rowCount) = (colCount, rowCount);
        
        Cells = InitCells(startPosition, mineGenerator, mineProbability);
    }

    private Cell[][] InitCells((char Column, int Row) startPosition, MineGenerator mineGenerator, double mineProbability)
    {
        var (isValid, attempts) = (false, 0);

        while (!isValid && attempts++ < 20)
        {
            var cells = Range(0, _rowCount).Select((_, rowIx) =>
                Range(0, _colCount).Select((_, cellIx) => new Cell(
                    Location: ((char)(cellIx + CharIntOffset), rowIx),
                    HasMine: rowIx != 0 && mineGenerator.Generate(),
                    HasUser: startPosition.Row == rowIx && startPosition.Column - CharIntOffset == cellIx
                )).ToArray()
            ).ToArray();
            
            // threshold between hard and possible
            if (mineProbability > 80 || BoardValidation.BoardIsValid(cells))
                return cells;
        }

        throw new Exception("Something has gone horribly wrong!");
    }

    public Game ProcessMove(string mode, Game game, ConsoleKey key)
    {
        var position = game.UserPosition;

        if (position.Row == 0 && key == DownArrow)
            throw new ArgumentOutOfRangeException(nameof(key), "Can't move down from Row 0");
        
        if (position.Column == CharIntOffset && key == LeftArrow)
            throw new ArgumentOutOfRangeException(nameof(key), "Can't move left from left Column");
        
        if (position.Column == CharIntOffset + _colCount - 1 && key == RightArrow)
            throw new ArgumentOutOfRangeException(nameof(key), "Can't move right from right Column");

        var oldCell = Cells[position.Row][position.Column - CharIntOffset];
        oldCell.HasUser = false;
        oldCell.IsVisited = true;
        
        position = NewUserPosition(game.UserPosition, key);
        
        if (position.Row == _rowCount)
            return game with
            {
                InProgress = false,
                Won = true
            };
        
        var newCell = Cells[position.Row][position.Column - CharIntOffset];
        newCell.HasUser = true;
        
        var userHasHitMine = newCell is { HasUser: true, HasMine: true };

        if (position.Row == 0 && userHasHitMine)
            throw new Exception("User unable to join board");

        if (userHasHitMine)
        {
            var startCell = Cells[game.StartPosition.Row][game.StartPosition.Column - CharIntOffset];
            startCell.HasUser = true;
        }

        var lives = userHasHitMine ? game.Lives - 1 : game.Lives;
        
        if (lives == 0)
            return game with
            {
                InProgress = false
            };

        return game with
        {
            Lives = lives,
            UserPosition = userHasHitMine ? game.StartPosition : position
        };
    }

    private static (char Column, int Row) NewUserPosition((char Column, int Row) position, ConsoleKey movement)
    {
        return movement switch
        {
            LeftArrow => ((char)(position.Column - 1), position.Row),
            UpArrow => (position.Column, position.Row + 1),
            RightArrow => ((char)(position.Column + 1), position.Row),
            DownArrow => (position.Column, position.Row - 1),
            
            _ => throw new ArgumentException(movement + " is not a valid movement", nameof(movement))
        };
    }
    
    public IEnumerable<string> RenderLines(string mode)
    {
        foreach (var line in (
            from r in Cells.Select((c, ix) => (cells: c, ix)).Reverse()
            let row = (r.ix, name: (char)(_rowCount + 64 - r.ix))
            let colLength = Cells.First().Length
            select PrintBorder(colLength) +
            $" {PrintContent((r.ix).ToString(), r.cells, mode)}"
        ))
            yield return line;
    }

    private static string PrintBorder(int colLength) => $"{new string(' ', 6)}{new string('-', colLength * 8 + (colLength - 1 * 4))}\n";
    private static string PrintContent(string rowName, IEnumerable<Cell> cells, string mode) => $"{rowName} ~ {string.Join("  ", cells.Select(PrintCell(mode)))} ~";

    private static Func<Cell, string> PrintCell(string mode) => cell =>
    {
        if (mode == "dev" && cell.HasMine)
        {
            return CellWithMine;
        }

        if (cell.HasUser)
            return CellWithUser;

        return cell.IsVisited ? CellNoMine : CellIndescript;
    };

    private const string 
        // dev
        CellWithMine = "|  #  |", CellNoMine = "|  \u263a  |",
        // user
        CellWithUser = "|  U  |", CellIndescript = "|  ?  |";
}

public record MineGenerator(double MineProbability)
{
    private static readonly Random MineRandom = new();

    public bool Generate() => MineRandom.NextDouble() <= MineProbability / 100;
}