var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var exchangeRates = new Dictionary<string, decimal>
{
    ["USD-EUR"] = 0.85m,
    ["EUR-USD"] = 1.18m,
    ["USD-GBP"] = 0.75m,
    ["GBP-USD"] = 1.33m,
    ["EUR-GBP"] = 0.88m,
    ["GBP-EUR"] = 1.14m,
    ["USD-UAH"] = 41.50m,
    ["UAH-USD"] = 0.0241m,
    ["EUR-UAH"] = 48.20m,
    ["UAH-EUR"] = 0.0207m,
    ["GBP-UAH"] = 55.20m,
    ["UAH-GBP"] = 0.0181m
};

string[] currencies = { "USD", "EUR", "UAH", "GBP" };

app.MapGet("/", () => "Currency Converter API");

app.MapGet("/currencies", () =>
{
    return Results.Ok(currencies);
});

app.MapGet("/exchangeRates", () =>
{
    return Results.Ok(exchangeRates);
});

app.MapGet("/exchangeRate/{from}/{to}", (string from, string to) =>
{
    from = from.ToUpper();
    to = to.ToUpper();

    if (!currencies.Contains(from) || !currencies.Contains(to))
    {
        return Results.BadRequest(new
        {
            error = "Валюта не підтримується"
        });
    }

    if (from == to)
    {
        return Results.Ok(1m);
    }

    string key = $"{from}-{to}";

    if (exchangeRates.TryGetValue(key, out decimal rate))
    {
        return Results.Ok(rate);
    }

    return Results.NotFound(new
    {
        error = "Курс не знайдено"
    });
});

app.MapGet("/convertCurrency/{from}/{to}/{amount}", (string from, string to, decimal amount) =>
    {
        from = from.ToUpper();
        to = to.ToUpper();

        if (!currencies.Contains(from) || !currencies.Contains(to))
        {
            return Results.BadRequest(new
            {
                error = "Валюта не підтримується"
            });
        }

        if (amount <= 0)
        {
            return Results.BadRequest(new
            {
                error = "Сума має бути більшою за нуль"
            });
        }

        decimal rate;

        if (from == to)
        {
            rate = 1m;
        }
        else
        {
            string key = $"{from}-{to}";

            if (!exchangeRates.TryGetValue(key, out rate))
            {
                return Results.NotFound(new
                {
                    error = "Курс не знайдено"
                });
            }
        }

        decimal result = amount * rate;

        return Results.Ok(new
        {
            from,
            to,
            amount,
            rate,
            result
        });
    });

app.Run();
