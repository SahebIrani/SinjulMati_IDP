
using System.Text.Json;

using IdentityModel.Client;

{
    // discover endpoints from metadata
    var client = new HttpClient();
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
    if (disco.IsError)
    {
        Console.WriteLine(disco.Error);
        return;
    }

    // request token
    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,

        ClientId = "client",
        ClientSecret = "secret",
        Scope = "api1"
    });

    if (tokenResponse.IsError)
    {
        Console.WriteLine(tokenResponse.Error);
        return;
    }

    Console.WriteLine(tokenResponse.AccessToken);

    // call api
    var apiClient = new HttpClient();
    apiClient.SetBearerToken(tokenResponse!.AccessToken!);

    var response = await apiClient.GetAsync("https://localhost:6001/getIdentity");
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(response.StatusCode);
    }
    else
    {
        var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
    }

    Console.ReadLine();
}

{
    var identityClient = new HttpClient();

    var discovery = await identityClient.GetDiscoveryDocumentAsync("https://localhost:7003");

    if (discovery.IsError)
    {
        System.Console.WriteLine(discovery.Error);
        return;
    }

    var tokenResponse = await identityClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = discovery.TokenEndpoint,
        ClientId = "client",
        ClientSecret = "secret",
        Scope = "api1"
    });

    if (tokenResponse.IsError)
    {
        System.Console.WriteLine(tokenResponse.Error);
        return;
    }

    var client = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7001")
    };

    client.SetBearerToken(tokenResponse!.AccessToken!);

    var stringResult = await client.GetStringAsync("/WeatherForecast");

    var result = JsonSerializer.Deserialize<List<WeatherForecast>>(stringResult);

    foreach (var item in result!)
    {
        Console.WriteLine($"{item.Date}\t {item.Summary} \t\t {item.TemperatureC}\t{item.TemperatureF}");
        Console.WriteLine("".PadLeft(200, '-'));
    }
}

Console.ReadLine();


internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
