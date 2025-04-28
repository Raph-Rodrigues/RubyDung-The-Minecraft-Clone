namespace RubyDung;

public class Program
{
    // Entry point of the program
    static void Main(string[] args)
    {
        // Creates game object and disposes of it after leaving the scope
        using Game game = new Game(1024, 700);
        game.Title = "Minecraft";
        // running the game
        game.Run();
    }
}