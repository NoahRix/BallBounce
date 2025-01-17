using System;

public static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        string arg = args.Length > 0 ? args[0].ToLowerInvariant() : "/s";

        switch (arg)
        {
            case "/s": // Run the screensaver
                using (var game = new BallBounce.Game1())
                    game.Run();
                break;

            case "/p": // Preview mode (not fully implemented here)
                // Typically, you would create a tiny window in the parent HWND.
                break;

            case "/c": // Configuration (optional)
                ShowConfiguration();
                break;

            default:
                Console.WriteLine("Invalid argument");
                break;
        }
    }

    static void ShowConfiguration()
    {
        // Display a simple configuration window (optional)
        System.Windows.Forms.MessageBox.Show("No configuration available.", "Screen Saver Config");
    }
}
