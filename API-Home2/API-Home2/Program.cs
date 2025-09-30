using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Load the app ===");
            Console.WriteLine("1. Load the notepad");
            Console.WriteLine("2. Load the calculator");
            Console.WriteLine("3. Load Paint");
            Console.WriteLine("4. Load something different (Enter path)");
            Console.WriteLine("5. Exit");
            Console.Write("Choose your option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    StartApp("notepad.exe");
                    break;
                case "2":
                    StartApp("calc.exe");
                    break;
                case "3":
                    StartApp("mspaint.exe");
                    break;
                case "4":
                    Console.Write("Enter a path to your exe: ");
                    string customPath = Console.ReadLine();
                    StartApp(customPath);
                    break;
                case "5":
                    Console.WriteLine("See you soon, comrade!");
                    return;
                default:
                    Console.WriteLine("Wrong choice...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void StartApp(string path)
    {
        try
        {
            Process.Start(path);
            Console.WriteLine("The app has been loaded! Press any key now");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error!: {ex.Message}");
        }

        Console.ReadKey();
    }
}
