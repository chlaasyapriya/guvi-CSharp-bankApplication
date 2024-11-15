using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingApp
{
    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
    }

    class Account
    {
        public static int accountCounter = 1001;
        public int AccountNumber { get; }
        public string HolderName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public DateTime LastInterestDate { get; set; } = DateTime.Today;

        public Account(string holderName, string accountType, decimal initialDeposit)
        {
            AccountNumber = accountCounter++;
            HolderName = holderName;
            AccountType = accountType;
            Balance = initialDeposit;
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            Transactions.Add(new Transaction("Deposit", amount));
        }

        public bool Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                Transactions.Add(new Transaction("Withdrawal", amount));
                return true;
            }
            return false; 
        }

        public void CalculateInterest(decimal interestRate)
        {
            if (AccountType == "Savings")
            {
                decimal interest = Balance * interestRate;
                Console.WriteLine($"Interest depositted after one month with the current balance will be {interest}");
            }
            else
            {
                Console.WriteLine("Checking Accounts do not add interests.");
            }
        }

        public void PrintStatement()
        {
            Console.WriteLine($"\nStatement for Account {AccountNumber}:");
            Console.WriteLine("Date\t\tType\tAmount");
            foreach (var transaction in Transactions)
            {
                Console.WriteLine($"{transaction.Date.ToShortDateString()}\t{transaction.Type}\t{transaction.Amount}");
            }
            Console.WriteLine($"\nCurrent Balance: {Balance}");
        }
    }

    class Transaction
    {
        public string Type { get; }
        public decimal Amount { get; }
        public DateTime Date { get; }

        public Transaction(string type, decimal amount)
        {
            Type = type;
            Amount = amount;
            Date = DateTime.Now;
        }
    }

    class Program
    {
        static List<User> users = new List<User>();
        const decimal InterestRate = 0.02m; 

        static void Main()
        {
            Console.WriteLine("Welcome to Console Bank");
            while (true)
            {
                Console.WriteLine("************************");
                Console.WriteLine("1. Register\n2. Login\n3. Exit");
                Console.Write("Select an option: ");
                int choice = int.Parse(Console.ReadLine());
                Console.WriteLine("************************");
                switch (choice)
                {
                    case 1: Register();
                            break;
                    case 2: Login();
                            break;
                    case 3: Console.WriteLine("Thank you");
                            Console.WriteLine("************************");
                            return;
                    default: Console.WriteLine("Invalid option. Please try again.");
                             break;
                }
            }
        }

        static void Register()
        {
            Console.Write("Enter a username: ");
            string username = Console.ReadLine();
            Console.Write("Enter a password: ");
            string password = Console.ReadLine();

            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists. Try again.");
                return;
            }

            users.Add(new User { Username = username, Password = password });
            Console.WriteLine("Registration successful.");
        }

        static void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                Console.WriteLine("Invalid credentials.");
                return;
            }

            Console.WriteLine("Login successful.");
            UserMenu(user);
        }

        static void UserMenu(User user)
        {
            while (true)
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine("1. Open Account\n2. Deposit\n3. Withdraw\n4. Print Statement\n5. Check Balance\n6. Calculate Interest\n7. Logout");
                Console.Write("Select an option: ");
                int choice = int.Parse(Console.ReadLine());
                Console.WriteLine("--------------------------");
                switch (choice)
                {
                    case 1: OpenAccount(user);
                            break;
                    case 2: Deposit(user);
                            break;
                    case 3: Withdraw(user);
                            break;
                    case 4: PrintStatement(user);
                            break;
                    case 5: CheckBalance(user);
                            break;
                    case 6: CalculateInterest(user);
                            break;
                    case 7: Console.WriteLine("User logged Out");
                            Console.WriteLine("--------------------------");
                            return;
                    default: Console.WriteLine("Invalid option. Please try again.");
                             break;
                }
            }
        }

        static void OpenAccount(User user)
        {
            Console.Write("Enter account holder name: ");
            string holderName = Console.ReadLine();
            Console.Write("Enter account type (Savings/Checking): ");
            string accountType = Console.ReadLine();
            Console.Write("Enter initial deposit amount: ");
            decimal initialDeposit = decimal.Parse(Console.ReadLine());

            user.Accounts.Add(new Account(holderName, accountType, initialDeposit));
            Console.WriteLine($"Account Number : {Account.accountCounter-1}");
            Console.WriteLine("Account created successfully.");
        }

        static void Deposit(User user)
        {
            Account account = SelectAccount(user);
            if (account == null) 
                return;

            Console.Write("Enter deposit amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            account.Deposit(amount);
            Console.WriteLine("Deposit successful.");
        }

        static void Withdraw(User user)
        {
            Account account = SelectAccount(user);
            if (account == null)
                return;

            Console.Write("Enter withdrawal amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            if (account.Withdraw(amount))
                Console.WriteLine("Withdrawal successful.");
            else
                Console.WriteLine("Insufficient balance.");
        }

        static void PrintStatement(User user)
        {
            Account account = SelectAccount(user);
            if (account == null)
                return;

            account.PrintStatement();
        }

        static void CheckBalance(User user)
        {
            Account account = SelectAccount(user);
            if (account == null)
                return;

            Console.WriteLine($"Current balance: {account.Balance}");
        }

        static void CalculateInterest(User user)
        {
            Account account = SelectAccount(user);
            if (account == null)
                return;

            account.CalculateInterest(InterestRate);
            //Console.WriteLine("Interest calculated and added to balance.");
        }

        static Account SelectAccount(User user)
        {
            if (user.Accounts.Count == 0)
            {
                Console.WriteLine("No accounts available.");
                return null;
            }

            Console.WriteLine("Select account:");
            for (int i = 0; i < user.Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {user.Accounts[i].AccountNumber} ({user.Accounts[i].AccountType})");

            int choice = int.Parse(Console.ReadLine()) - 1;

            if (choice >= 0 && choice < user.Accounts.Count)
                return user.Accounts[choice];

            Console.WriteLine("Invalid selection.");
            return null;
        }
    }
}

