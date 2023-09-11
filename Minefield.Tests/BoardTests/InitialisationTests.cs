using MineField;
using Snapper;

namespace Minefield.Tests.BoardTests;

public class InitialisationTests
{
    private static readonly (char Col, int Row) StartPosition = ('C', 0);
    
    [Theory, InlineData(1), InlineData(88.88)]
    public void BoardAcceptsValidMineProbabilityPercentage(double validPercentage)
    {
        var board = new Board(StartPosition, validPercentage);
    }

    [Theory, InlineData(-1), InlineData(101)]
    public void BoardRejectsInvalidMineProbabilityPercentage(double invalidPercentage)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Board(StartPosition, invalidPercentage));
    }

    [Fact]
    public void BoardRendersToDesignatedSize()
    {
        var board = new Board(StartPosition, 50);
        Assert.Equal(DefaultRowCount, board.Cells.Length);
        Assert.Equal(DefaultRowCount * DefaultColCount, board.Cells.SelectMany(c => c).Count());
    }
    
    [Fact]
    public void EmptyBoardRenderMatchesSnapshot()
    {
        var board = new Board(StartPosition, 0);
        board.RenderLines("dev").ShouldMatchSnapshot();
    }
    
    [Fact]
    public void FullBoardRenderMatchesSnapshot()
    {
        var board = new Board(StartPosition, 100);
        board.RenderLines("dev").ShouldMatchSnapshot();
    }

    [Fact]
    public void CanAlwaysJoinBoard()
    {
        foreach (char colChar in Range('A', DefaultColCount))
        {
            var board = new Board((colChar, 0), 100);
        }
    }

    [Fact]
    public void CannotJoinOffBoard()
    {
        foreach (char colChar in Range('A', DefaultColCount))
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Board(((char)(colChar * 2), 0), 0)
            );
        }
    }
}