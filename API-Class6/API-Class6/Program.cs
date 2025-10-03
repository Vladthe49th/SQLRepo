

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Task 1
        var task1 = Task.Run(() =>
        {
            int sum = Enumerable.Range(1, 100).Sum();
            Console.WriteLine($"Sum from 1 to 100: {sum}");
        });

        // Task 2
        string file1Path = "numbers.txt";
        var task2 = Task.Run(() =>
        {
            Random rnd = new Random();
            var numbers = Enumerable.Range(0, 20).Select(_ => rnd.Next(1, 101)).ToList();
            File.WriteAllLines(file1Path, numbers.Select(n => n.ToString()));
            Console.WriteLine("Random nums written in file №1.");
        });

        await Task.WhenAll(task1, task2); 

        // Task 3
        string file2Path = "binary_numbers.txt";
        var task3 = Task.Run(() =>
        {
            var lines = File.ReadAllLines(file1Path);
            var binaryLines = lines
                .Select(line => int.TryParse(line, out int num) ? Convert.ToString(num, 2) : "Ошибка")
                .ToList();

            File.WriteAllLines(file2Path, binaryLines);
            Console.WriteLine("Binaries written in file №2.");
        });

        await task3;

        Console.WriteLine("Tasks complete!");
    }
}
