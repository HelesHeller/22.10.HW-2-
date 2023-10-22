using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
class CreditCard
{
    public string CardNumber { get; set; }
    public string OwnerName { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int Pin { get; set; }
    public double CreditLimit { get; set; }
    public double Balance { get; set; }

    // Події
    public event Action<double> AccountReplenished;
    public event Action<double> MoneySpent;
    public event Action CreditStarted;
    public event Action<double> LimitReached;
    public event Action PinChanged;

    // Метод для поповнення рахунку
    public void ReplenishAccount(double amount)
    {
        Balance += amount;
        AccountReplenished?.Invoke(amount);
    }

    // Метод для витрати коштів з рахунку
    public void SpendMoney(double amount)
    {
        if (amount > Balance)
        {
            Console.WriteLine("Недостатньо коштів на рахунку!");
        }
        else
        {
            Balance -= amount;
            MoneySpent?.Invoke(amount);
        }
    }

    // Метод для старту використання кредитних коштів
    public void StartCredit()
    {
        CreditStarted?.Invoke();
    }

    // Метод для зміни PIN
    public void ChangePin(int newPin)
    {
        Pin = newPin;
        PinChanged?.Invoke();
    }

    // Метод для перевірки досягнення ліміту
    public void CheckLimit()
    {
        if (Balance >= CreditLimit)
        {
            LimitReached?.Invoke(Balance);
        }
    }
}

class Program
{
    static void Main()
    {
        // Створюємо об'єкт кредитної картки
        CreditCard card = new CreditCard
        {
            CardNumber = "1234567890123456",
            OwnerName = "John Doe",
            ExpiryDate = new DateTime(2025, 12, 31),
            Pin = 1234,
            CreditLimit = 1000,
            Balance = 500
        };

        // Підписуємось на події
        card.AccountReplenished += amount => Console.WriteLine($"Рахунок поповнено на {amount} грн");
        card.MoneySpent += amount => Console.WriteLine($"Витрачено {amount} грн");
        card.CreditStarted += () => Console.WriteLine("Кредит активовано!");
        card.LimitReached += balance => Console.WriteLine($"Досягнуто ліміт: {balance} грн");
        card.PinChanged += () => Console.WriteLine("PIN успішно змінено!");

        // Викликаємо події та змінюємо PIN
        card.CheckLimit();
        card.ReplenishAccount(200);
        card.SpendMoney(700);
        card.CheckLimit();
        card.ChangePin(4321);

        // Записуємо об'єкт у файл
        SerializeToFile(card, "credit_card.dat");

        // Читаємо об'єкт з файлу
        CreditCard loadedCard = DeserializeFromFile("credit_card.dat");
        Console.WriteLine($"Зчитано з файлу: Номер картки: {loadedCard.CardNumber}, Власник: {loadedCard.OwnerName}");
    }

    // Метод для серіалізації об'єкта у файл
    static void SerializeToFile(CreditCard card, string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
        {
            formatter.Serialize(stream, card);
        }
        Console.WriteLine("Об'єкт записано в файл.");
    }

    // Метод для десеріалізації об'єкта з файлу
    static CreditCard DeserializeFromFile(string fileName)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            return (CreditCard)formatter.Deserialize(stream);
        }
    }
}
