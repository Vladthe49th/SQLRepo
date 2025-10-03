using System;
using System.Threading;

class Bank
{
    private static object lockObject = new object();
    private static int balance = 1000; // start balance

    public static void Withdraw(int amount, string atmName)
    {
        Console.WriteLine($"{atmName} attempts to withdraw {amount}...");

        bool lockTaken = false;
        try
        {
            Monitor.Enter(lockObject, ref lockTaken); // attempt to seize a lock

            if (balance >= amount)
            {
                Console.WriteLine($"{atmName}: Enough money! Withdrawing...");
                Thread.Sleep(100); 
                balance -= amount;
                Console.WriteLine($"{atmName}: Withdraw successful. Remains: {balance}");
            }
            else
            {
                Console.WriteLine($"{atmName}: Not enough money! Remains: {balance}");
            }
        }
        finally
        {
            if (lockTaken)
                Monitor.Exit(lockObject); // Ffree the lock
        }
    }
}

class Program
{
    static void Main()
    {
        Thread[] atms = new Thread[5];

        for (int i = 0; i < atms.Length; i++)
        {
            string atmName = $"Bankomat #{i + 1}";
            atms[i] = new Thread(() =>
            {
                for (int j = 0; j < 3; j++)
                {
                    Bank.Withdraw(300, atmName);
                    Thread.Sleep(new Random().Next(50, 200)); 
                }
            });
        }

        // Loading all bankomats
        foreach (var atm in atms)
            atm.Start();

       
        foreach (var atm in atms)
            atm.Join();

        Console.WriteLine("Work of bankomats is finished!");
    }
}
