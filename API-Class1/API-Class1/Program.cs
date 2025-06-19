using System;
using System.Threading;

class Program
{
    static int[] array1;
    static int[] array2;

    static void Main()
    {
        const int size = 100_000_000;

        Console.WriteLine("Creating the array...");
        Random random = new Random();

        // Creating two copies of the array
        array1 = new int[size];
        array2 = new int[size];
        for (int i = 0; i < size; i++)
        {
            int value = random.Next();
            array1[i] = value;
            array2[i] = value;
        }

        // Two threads
        Thread thread1 = new Thread(SortWithArraySort);
        Thread thread2 = new Thread(SortWithBubbleSort);

        Console.WriteLine("Loading threads...");
        thread1.Start();
        thread2.Start();

        
        thread1.Join();
        thread2.Join();

        Console.WriteLine("Both sortings finished!");
    }

    static void SortWithArraySort()
    {
        Array.Sort(array1);
        Console.WriteLine("[1] Sorting with method Array.Sort finished!");
    }

    static void SortWithBubbleSort()
    {
        
        int length = 100_000_000;
        for (int i = 0; i < length - 1; i++)
        {
            for (int j = 0; j < length - i - 1; j++)
            {
                if (array2[j] > array2[j + 1])
                {
                    int temp = array2[j];
                    array2[j] = array2[j + 1];
                    array2[j + 1] = temp;
                }
            }
        }
        Console.WriteLine("[2] Sorting with method BubbleSort finished!");
    }
}
