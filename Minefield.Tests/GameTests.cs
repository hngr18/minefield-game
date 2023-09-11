using MineField;

namespace Minefield.Tests;

public class GameTests
{
    private static readonly (char Col, int Row) StartPosition = ('C', 0);
    
    [Fact]
    public void NoMinesGameWinnable()
    {
        var game = new Game("user", StartPosition, 0);
        foreach (var _ in Range(0, DefaultRowCount))
        {
            game = game.Board.ProcessMove("", game, ConsoleKey.UpArrow);
        }
        Assert.True(game.Won);
        Assert.False(game.InProgress);
    }
    
    [Fact]
    public void AllMinesGameUnwinnable()
    {
        var game = new Game("user", StartPosition, 100);
        foreach (var _ in Range(0, DefaultLives))
        {
            game = game.Board.ProcessMove("", game, ConsoleKey.UpArrow);
        }
        Assert.False(game.Won);
        Assert.False(game.InProgress);
    }

    [Fact]
    public void NoImpassibleRows()
    {
        foreach (var iteration in Range(0, 10))
        {
            try
            {
                var game = new Game("user", StartPosition);
                Assert.All(game.Board.Cells, row => Assert.Contains(row, cell => !cell.HasMine));
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(iteration)} {iteration}", ex);
            }
        }
    }
    
    [Fact]
    public void AllGamesAreWinnable()
    {
        var game = new Game("user", StartPosition, 30);
        var safeCells = game.Board.Cells.SelectMany(row => 
            row.Where(cell => !cell.HasMine).Select(cell => cell.Location)
        ).ToArray();

        var traversalStarts = Range(0, DefaultColCount).Select(colIx => 
            (col :(char)(colIx + CharIntOffset), row: 0)
        ).ToArray();

        foreach (var start in traversalStarts)
        {
            var possibleLocations = safeCells.Where(BoardValidation.CellIsAdjacent(start)).ToHashSet();
            
            foreach (var _ in Range(1, 100))
            {
                foreach (var cell in possibleLocations.SelectMany(possibleLocation => 
                             safeCells.Where(BoardValidation.CellIsAdjacent(possibleLocation))).ToArray())
                {
                    possibleLocations.Add(cell);
                }
            }
            Assert.Contains(possibleLocations, location => location.row == DefaultRowCount - 1);
        }
    }
}