# Телеграм-бот для получения курса валют с **Coinmarketcap** (RU)

В этом проекте я реализовал телеграмм-бота на языке C# с помощью библиотеки **"Telegram.Bot"**. Этот бот присылает текущий курс выбранной по запросу пользователя криптовалюты (BTC, ETH, BNB, DOGE), используя API Coinmarketcap. Бот округляет стоимость валюты до трёх знаков после запятой.

## Для самостоятельного запуска бота вам необходимо:
#### 1. Создать бота в **BotFather** (бот для регистрации, настройки и управления другими telegram-ботами).
    1. Отправьте BotFather сообщение /newbot
    2. Введите название вашего бота
    3. Введите username бота (оно обязательно должно оканчиваться на слово “bot”). 
    4. После создания бота вы получите токен, который нужно будет в следующем шаге.
#### 2. В классе **ApiConstants** введите свой Coinmarketcap-API и токен вашего телеграмм-бота в поля соответствующих констант.
   ```c sharp
   public static class ApiConstants
   {
      public const string BOT_API = "Токен telegram-бота";
      public const string COIN_MARKET_CAP_API = "CMC API";
   }
   ```
#### 3. Сохраните и запустите приложение.
---
# Telegram bot for getting currency rates from **Coinmarketcap** (ENG)

In this project I have implemented a Telegram bot in C# language using **"Telegram.Bot "** library. This bot sends the current exchange rate of the selected cryptocurrency (BTC, ETH, BNB, DOGE) using Coinmarketcap API. The bot rounds the currency value to three decimal places.

## To run the bot by yourself you need:
#### 1. Create a bot in **BotFather** (a bot for registering, customizing and managing other telegram bots).
     1. Send BotFather the message /newbot
     2. Enter the name of your bot
     3. Enter the bot's username (it must end with the word "bot"). 
     4. After creating the bot you will receive a token, which you will need in the next step.
#### 2. In the **ApiConstants** class, enter your Coinmarketcap-API and your telegram bot token into the fields of the corresponding constants.
   ```c sharp
   public static class ApiConstants
   {
      public const string BOT_API = "Telegram bot token";
      public const string COIN_MARKET_CAP_API = "CMC API";
   }
   ```
#### 3. Save and run the application.
