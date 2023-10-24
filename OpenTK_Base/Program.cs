namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (Game game = new Game(320*2, 320*2, "Test"))
            {
                game.Run();
            }
    }
    }
}