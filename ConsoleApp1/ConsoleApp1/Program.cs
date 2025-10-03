using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        // Initial array
        int[] array = { 5, 3, 8, 3, 9, 1, 5, 2, 8 };
        int valueToSearch = 8;

        Console.WriteLine("Initial array:");
        Console.WriteLine(string.Join(", ", array));

        // Task 1: no dublicates
        var removeDuplicatesTask = Task.Run(() =>
        {
            Console.WriteLine("\nDeleting dublicates...");
            var distinctArray = array.Distinct().ToArray();
            Console.WriteLine("Without doubles: " + string.Join(", ", distinctArray));
            return distinctArray;
        });

        // Task 2: sorting
        var sortTask = removeDuplicatesTask.ContinueWith(previousTask =>
        {
            Console.WriteLine("\nSorting...");
            var sortedArray = previousTask.Result.OrderBy(x => x).ToArray();
            Console.WriteLine("Sorted: " + string.Join(", ", sortedArray));
            return sortedArray;
        });

        // Task 3: binary search
        var searchTask = sortTask.ContinueWith(previousTask =>
        {
            Console.WriteLine("\nBinary value search: " + valueToSearch);
            var sortedArray = previousTask.Result;
            int index = Array.BinarySearch(sortedArray, valueToSearch);

            if (index >= 0)
                Console.WriteLine($"Value {valueToSearch} found on position {index}.");
            else
                Console.WriteLine($"Value {valueToSearch} not found.");
        });

        // Waiting the final task
        searchTask.Wait();
        Console.WriteLine("\nAll tasks complete.");
    }
}
