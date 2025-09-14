class Program
{
    static void SecondFunc()
    {
        Console.WriteLine("START");
        Thread.Sleep(2000); // головний потік блокується на 2 секунди
        Console.WriteLine("END");
    }
    // є один головний потік, і 2 функціі працюють по черзі (послідовно)
    static void Main()
    {
        Console.WriteLine("main 1");
        SecondFunc(); // синхронний виклик
        Console.WriteLine("main 2");
    }
}