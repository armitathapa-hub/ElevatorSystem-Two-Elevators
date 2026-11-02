namespace ElevatorSystem
{
    internal static class Program
    {
        // Starting point of the Elevator System application.
        [STAThread]
        static void Main()
        {
            // Sets up the application before opening the main form.
            ApplicationConfiguration.Initialize();

            // Opens Form1 and starts the application.
            Application.Run(new Form1());
        }
    }
}