namespace API_Home4
{

    public class Account
    {
        private object balanceLock = new object();
        public int Id { get; }
        public decimal Balance { get; private set; }

        public Account(int id, decimal initialBalance = 0)
        {
            Id = id;
            Balance = initialBalance;
        }

        public void Deposit(decimal amount)
        {
            lock (balanceLock)
            {
                Balance += amount;
                Console.WriteLine($"[Balance: {Id}] Deposit: +{amount:C}. New balance: {Balance:C}");
            }
        }

        public void Withdraw(decimal amount)
        {
            lock (balanceLock)
            {
                if (Balance >= amount)
                {
                    Balance -= amount;
                    Console.WriteLine($"[Balamce: {Id}] Withdraw: -{amount:C}. New balance: {Balance:C}");
                }
                else
                {
                    Console.WriteLine($"[Balance: {Id}] Not enough money to withdraw! {amount:C}. The balance stays the same");
                }
            }
        }

        public decimal GetBalance()
        {
            lock (balanceLock)
            {
                return Balance;
            }
        }
    }


    public class Client
    {
        private readonly Random random = new Random();
        private readonly Account account;
        private readonly int clientId;

        public Client(int clientId, Account account)
        {
            this.clientId = clientId;
            this.account = account;
        }

        public void Start()
        {
            while (true)
            {
                int delay = random.Next(1000, 3000); 
                Thread.Sleep(delay);

                decimal amount = random.Next(10, 201); 
                bool deposit = random.Next(2) == 0;

                if (deposit)
                {
                    account.Deposit(amount);
                }
                else
                {
                    account.Withdraw(amount);
                }
            }
        }
    }




    class Program
    {
        static void Main()
        {
            int clientCount = 5;
            List<Thread> threads = new List<Thread>();
            List<Account> accounts = new List<Account>();

            // Client accounts
            for (int i = 0; i < clientCount; i++)
            {
                Account account = new Account(i + 1, 1000);
                accounts.Add(account);

                Client client = new Client(i + 1, account);
                Thread thread = new Thread(new ThreadStart(client.Start));
                thread.IsBackground = true; 
                threads.Add(thread);
            }

            // Starting threads
            foreach (var thread in threads)
            {
                thread.Start();
            }

            // 30 seconds
            Thread.Sleep(30000);

            Console.WriteLine("\nTHE FINAL BALANCE STATE IS: ");
            foreach (var account in accounts)
            {
                Console.WriteLine($"Balance {account.Id}: {account.GetBalance():C}");
            }

            Console.WriteLine("\nProgram finished!");
        }
    }

}
