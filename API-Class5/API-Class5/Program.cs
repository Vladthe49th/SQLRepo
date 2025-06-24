using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using System;
using System.Threading;

class Program
{
    static int[] numbers;
    static int sum = 0;
    static double average = 0.0;
    static object lockObj = new object();
    static ManualResetEvent sumCalculated = new ManualResetEvent(false); 
    static void Main()
    {
        // 1. The array
        numbers = new int[100];
        Random rand = new Random();
        for (int i = 0; i < numbers.Length; i++)
            numbers[i] = rand.Next(1, 101); 

        // 2. The threads
        Thread sumThread = new Thread(CalculateSum);
        Thread avgThread = new Thread(CalculateAverage);

        sumThread.Start();
        avgThread.Start();

        sumThread.Join();
        avgThread.Join();

        Console.WriteLine("Job is done!");
    }

    static void CalculateSum()
    {
        int localSum = 0;

        foreach (int num in numbers)
            localSum += num;

        lock (lockObj)
        {
            sum = localSum;
            Console.WriteLine($"Array sum: {sum}");
        }

        sumCalculated.Set()
    }

    static void CalculateAverage()
    {
        sumCalculated.WaitOne(); 

        lock (lockObj)
        {
            average = (double)sum / numbers.Length;
            Console.WriteLine($"Averagessss: {average:F2}");
        }
    }
}
