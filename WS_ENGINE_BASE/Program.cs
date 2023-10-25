using WS_ENGINE_BASE;

namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (WS_ENGINE_BASE.MainWindow game = new WS_ENGINE_BASE.MainWindow(320*2, 320*2, "Test"))
            {
                game.Run();
            }
    }
    }
}