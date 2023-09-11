using MineField;

namespace Minefield.Tests.BoardTests;

public class MoveTests
{
    private static readonly (char Col, int Row) StartPosition = ('C', 0);
    
    [Theory]
    [InlineData(ConsoleKey.UpArrow)]
    [InlineData(ConsoleKey.LeftArrow), InlineData(ConsoleKey.RightArrow)]
    public void ArrowsAccepted(ConsoleKey key)
    {
        var board = new Board(StartPosition, 0);
        board.ProcessMove("", new Game("user", ('C', 0)), key);
    }
    
    [Fact]
    public void DownArrowRejectedFrom_Row0()
    {
        var board = new Board(StartPosition, 0);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            board.ProcessMove("", new Game("user", ('C', 0)), ConsoleKey.DownArrow)
        );
    }
    
    [Fact]
    public void LeftArrowRejectedFrom_ColA()
    {
        var position = ('A', 4);
        var board = new Board(position, 0);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            board.ProcessMove("", new Game("user", position), ConsoleKey.LeftArrow)
        );
    }
    
    [Fact]
    public void RightArrowRejectedFrom_ColH()
    {
        var position = ('H', 7);
        var board = new Board(position, 0);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            board.ProcessMove("", new Game("user", position), ConsoleKey.RightArrow)
        );
    }
    
    [Fact]
    public void UserCanMoveForwardsWithNoMines()
    {
        var game = new Game("user", StartPosition, 0);
        game = game.Board.ProcessMove("", game, ConsoleKey.UpArrow);
        Assert.Equal((StartPosition.Col, StartPosition.Row + 1), game.UserPosition);
    }

    [Fact]
    public void HittingMineSendsUserToStartPosition()
    {
        var game = new Game("user", StartPosition, 100);
        game = game.Board.ProcessMove("", game, ConsoleKey.UpArrow);
        Assert.Equal(StartPosition, game.UserPosition);
    }
    
    [Fact]
    public void HittingMineDeductsLife()
    {
        var game = new Game("user", StartPosition, 100);
        game = game.Board.ProcessMove("", game, ConsoleKey.UpArrow);
        Assert.Equal(DefaultLives - 1, game.Lives);
    }
}