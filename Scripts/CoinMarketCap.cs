using System.Text.Json.Nodes;

namespace telegram_bot_currency_rate.Scripts;

public static class CoinMarketCap
{
    private static readonly string API_KEY = ApiConstants.COIN_MARKET_CAP_API;

    public static async Task<decimal> GetPriceAsync(string currencyCode)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", API_KEY);
        var response = await httpClient.GetAsync(
            $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={currencyCode}&convert=USD");
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonNode.Parse(responseString);
        var price = (decimal)jsonResponse["data"][currencyCode]["quote"]["USD"]["price"];

        return price;
    }
}