namespace telegram_bot_currency_rate.Scpripts;

public class Program
{
    public static void Main(string[] args)
    {
        var currencyBot = new CurrencyBot(ApiConstants.BOT_API);

        currencyBot.CreateCommands();
        currencyBot.StartReceiving();

        Console.ReadKey();
    }
}