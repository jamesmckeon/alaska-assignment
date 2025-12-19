// displays message and prompts user to exit the program

// ReSharper disable All

using System.Net.Http.Json;
using System.Text.Json;
using ElevatorApi.Api.Models;

void Exit(string message)
{
    Console.WriteLine(message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return;
}

try
{
    #region Setup

    using var handler = new HttpClientHandler()
    {
        CheckCertificateRevocationList = true
    };

    using var client = new HttpClient(handler);
    client.BaseAddress = new Uri("http://localhost:8080");

    // executes a GET request and returns the response if successful
    var ping = async (string endpoint) =>
    {
        var response = await client.GetAsync(new Uri(endpoint, UriKind.Relative));

        if (!response.IsSuccessStatusCode)
        {
            Exit($"Error: {endpoint} endpoint returned {response.StatusCode}.");
        }

        return response;
    };

    // executes a request and returns the corresponding CarResponse DTO if successful
    var pingCar = async (string endpoint, HttpMethod method) =>
    {
        HttpResponseMessage response;

        if (method == HttpMethod.Get)
        {
            response = await client.GetAsync(new Uri(endpoint, UriKind.Relative));
        }
        else if (method == HttpMethod.Post)
        {
            response = await client.PostAsync(new Uri(endpoint, UriKind.Relative), null);
        }
        else
        {
            throw new NotSupportedException($"Method {method} not supported");
        }

        if (!response.IsSuccessStatusCode)
        {
            Exit($"Error: {endpoint} endpoint returned {response.StatusCode}.");
        }

        var car = await response.Content.ReadFromJsonAsync<CarResponse>();
        Console.WriteLine($"Response: {JsonSerializer.Serialize(car)}");
        return car;
    };

    var moveCar = async (CarResponse carToMove) =>
    {
        Console.WriteLine($"Moving car #{carToMove.Id} to next stop ...");

        var response = await client.PostAsync(new Uri($"/cars/{carToMove.Id}/move", UriKind.Relative), null);

        if (!response.IsSuccessStatusCode)
        {
            Exit($"Error: move endpoint returned {response.StatusCode}.");
        }

        var movedCar = await response.Content.ReadFromJsonAsync<CarResponse>();
        Console.WriteLine($"Response: {JsonSerializer.Serialize(movedCar)}");
        Console.WriteLine("");

        return movedCar;
    };

    #endregion

    Console.WriteLine("Checking health ...");
    await ping("/health");
    Console.WriteLine();

    Console.WriteLine("This simulates a building with two elevators, a basement floor, a lobby, " + 
                      "and 5 floors above it.  Both cars start at the lobby by default.");
    Console.WriteLine();
    
    Console.WriteLine("Car #1 initial state ...");
    await pingCar("/cars/1", HttpMethod.Get);
    
    Console.WriteLine("Car #2 initial state ...");
    await pingCar("/cars/2", HttpMethod.Get);
    Console.WriteLine();

    Console.WriteLine("Requesting car for floor #1 ...");
    var firstFloor = await pingCar("/cars/call/1", HttpMethod.Post);
    Console.WriteLine();

    await moveCar(firstFloor);
/*
    Console.WriteLine($"Adding stop to car #{lobby.Id} ...");
    await pingCar($"/cars/{lobby.Id}/stops/2", HttpMethod.Post);
    Console.WriteLine();

    Console.WriteLine($"Adding stop to car #{lobby.Id} ...");
    await pingCar($"/cars/{lobby.Id}/stops/5", HttpMethod.Post);
    Console.WriteLine();

    await moveCar(lobby);

    Console.WriteLine("Requesting car for floor #-2 ...");
    var basement = await pingCar("/cars/call/-2", HttpMethod.Post);
    Console.WriteLine();

    Console.WriteLine("Requesting car for floor #5 ...");
    var fifthFloor = await pingCar("/cars/call/5", HttpMethod.Post);
    Console.WriteLine();

    await moveCar(basement);
*/
    Console.WriteLine("All done.  Press any key to exit...");
    Console.ReadKey();
}
catch (HttpRequestException ex)
{
    Exit($"Request encountered an exception ('{ex.Message}'): verify that api is running.");
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031

{
    Exit($"Exception encountered: {ex.Message}");
}