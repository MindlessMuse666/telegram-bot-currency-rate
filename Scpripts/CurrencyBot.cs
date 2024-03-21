using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegram_bot_currency_rate.Scpripts;

public class CurrencyBot
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly List<string> _currencyCodes =
    [
        CurrencyCode.BTC, CurrencyCode.ETH, CurrencyCode.BNB, CurrencyCode.DOGE
    ];

    public CurrencyBot(string token) => _telegramBotClient = new TelegramBotClient(token);

    public void CreateCommands()
    {
        _telegramBotClient.SetMyCommandsAsync(new List<BotCommand>()
        {
            new()
            {
                Command = CustomBotCommands.START,
                Description = "Запуск бота."
            },
            new()
            {
                Command = CustomBotCommands.SHOW_CURRENSIES,
                Description = "Вывод сообщения с выбором 1 из 4 валют, для получения ее цены в данный момент"
			}
        });
    }

    public void StartReceiving()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new UpdateType[]
            {
                UpdateType.Message, UpdateType.CallbackQuery
            }
        };

        _telegramBotClient.StartReceiving(
            HandleUpdateAsync,
            HandleError,
            receiverOptions,
            cancellationToken);
    }

    private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient telegramBot, Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                await HandleMessageAsync(update, cancellationToken);
                break;
            case UpdateType.CallbackQuery:
                await HandleCallbackQueryAsync(update, cancellationToken);
                break;
        }
    }

    private async Task HandleMessageAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message == null)
            return;

        var chatId = update.Message.Chat.Id;
        await DeleteMessage(chatId, update.Message.MessageId, cancellationToken);

        if (update.Message.Text == null)
        { 
            await _telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Бот принимает только команды из меню.",
                cancellationToken: cancellationToken);
            return;
        }

        var messageText = update.Message?.Text;

        if (IsStartCommand(messageText))
        {
            await SendStartMessageAsync(chatId, cancellationToken);
            return;
        }
		if (IsShowCommand(messageText))
		{
            await ShowCurrencySelectionAsync(chatId, cancellationToken);
		}
	}

    private async Task DeleteMessage(long chatId, int messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _telegramBotClient.DeleteMessageAsync(chatId, messageId, cancellationToken);
        }
        catch (ApiRequestException exception)
        {
            if (exception.ErrorCode == 400)
            {
                Console.WriteLine("User deleted message");
            }
        }
    }

    private bool IsStartCommand(string messageText) => messageText.ToLower() == CustomBotCommands.START;

	private bool IsShowCommand(string messageText) => messageText.ToLower() == CustomBotCommands.SHOW_CURRENSIES;

    private async Task SendStartMessageAsync(long? chatId, CancellationToken cancellationToken)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Выбрать валюту.", CustomCallbackData.SHOW_CURRENCIES_MENU)
            }
        });

        await _telegramBotClient.SendTextMessageAsync(
            chatId, "Привет!\nДанный бот показывает текущий курс выбранной валюты.\n",
            replyMarkup: inlineKeyboard, 
            cancellationToken: cancellationToken);
    }

    private async Task ShowCurrencySelectionAsync(long? chatId, CancellationToken cancellationToken)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Bitcoin", CurrencyCode.BTC),
                InlineKeyboardButton.WithCallbackData("Ethereum", CurrencyCode.ETH),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("BNB", CurrencyCode.BNB),
                InlineKeyboardButton.WithCallbackData("Dogecoin", CurrencyCode.DOGE),
            },
        });

        await _telegramBotClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Выберите валюту:",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private async Task HandleCallbackQueryAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.CallbackQuery?.Message == null)
            return;

        var chatId = update.CallbackQuery.Message.Chat.Id;
        var callbackData = update.CallbackQuery.Data;
        var messageId = update.CallbackQuery.Message.MessageId;

        if (callbackData == CustomCallbackData.SHOW_CURRENCIES_MENU)
        {
            await DeleteMessage(chatId, messageId, cancellationToken);
            await ShowCurrencySelectionAsync(chatId, cancellationToken);
            return;
        }

        if (_currencyCodes.Contains(callbackData))
        {
            await DeleteMessage(chatId, messageId, cancellationToken);
            await SendCurrencyPriceAsync(chatId, callbackData, cancellationToken);
            return;
        }

        if (callbackData == CustomCallbackData.RETURN_TO_CURRENCIES_MENU)
        {
            await ShowCurrencySelectionAsync(chatId, cancellationToken);
        }
    }

    private async Task SendCurrencyPriceAsync(long? chatId, string currencyCode, CancellationToken cancellationToken)
    {
        var price = await CoinMarketCap.GetPriceAsync(currencyCode);

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Выбрать другую валюту.",
                    CustomCallbackData.RETURN_TO_CURRENCIES_MENU)
            }
        });

        await _telegramBotClient.SendTextMessageAsync(
            chatId,
            text: $"Валюта: {currencyCode}, стоимость: {Math.Round(price, 3)}$",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
}