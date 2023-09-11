// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices.Marshalling;
using MineField;

const string mode = ""; // dev|user

if (!string.IsNullOrWhiteSpace(mode))
    WriteLine($"Running in {mode} mode");

WriteLine("Write your name:");
var username = ReadLine();
if (string.IsNullOrWhiteSpace(username))
{
    WriteLine("no username supplied");
    throw new ArgumentException(nameof(username));
}

while (true)
{
    var (colStart, colEnd) = ((char)65, (char)(DefaultColCount + 65));
    WriteLine($"Select a start position ({colStart} - {colEnd})");
    var position = ReadLine()?.ToUpperInvariant(); //'C';
    if (string.IsNullOrWhiteSpace(position))
        throw new ArgumentException(nameof(position));

    var game = new Game(username, (position.ToCharArray().Single(), 0));

    Clear();

    while (game.InProgress)
    {
        WriteLine($"Welcome {game.Username}");
        DisplayBoard(game, mode);

        var userMove = ReadKey();

        var livesBeforeMove = game.Lives;

        game = game.Board.ProcessMove(mode, game, userMove.Key);

        if (game.Lives < livesBeforeMove)
        {
            Clear();
            WriteLine("Whoops!\n\nBack to the start you go!");
            Thread.Sleep(2000);
        }

        Clear();

        if (game.Won)
        {
            WriteLine("\u263a  Winner! \u263a\n");
            Thread.Sleep(2000);
            Clear();
            WriteLine("Play again? (Y/N)");
            var againKey = ReadKey();
            if (string.Equals("Y", againKey.Key.ToString(), StringComparison.InvariantCultureIgnoreCase))
                continue;
            break;
        }

        if (game.Lives == 0)
        {
            WriteLine("\u2639  Better Luck Next Time! \u2639\n");
            Thread.Sleep(2000);
            Clear();
            WriteLine("Play again? (Y/N)");
            var againKey = ReadKey();
            if (string.Equals("Y", againKey.Key.ToString(), StringComparison.InvariantCultureIgnoreCase))
                continue;
            break;
        }
    }
}

return;

void DisplayBoard(Game game1, string s)
{
    WriteLine($"lives: {game1.Lives}");
    WriteTop();
    foreach (var row in game1.Board.RenderLines(s))
    {
        WriteLine(row);
    }

    void WriteTop()
    {
        WriteLine(new string(' ', 4) + string.Join(new string(' ', 4),
            Range(CharIntOffset, DefaultColCount).Select(c => new string(' ', 4) + (char)c)
        ));
    }
}

//WriteLine("Hello, World!");