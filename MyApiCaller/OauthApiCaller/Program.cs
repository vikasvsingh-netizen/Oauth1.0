using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OauthApiCaller;
using System.Diagnostics;
using System.Net.Http;

namespace Oauth_1a_Demo_Caller
{
    public class Program
    {
        public async Task Main(string[] args)
        {

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("Config.json", optional: false, reloadOnChange: true);
                    config.AddUserSecrets<Program>();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<ApiCaller>();
                })
                .Build();

            // Resolve ApiCaller from DI
            var apiCaller = host.Services.GetRequiredService<ApiCaller>();

            // Prepare request options
            string consumerKey = "consumerKey1";
            string accessToken = "accessToken1";


            bool callAgain = true;

            while (callAgain)
            {
                var options = new RequestOptions
                {
                    Url = "https://localhost:7028/api/test/get",
                    Method = HttpMethod.Get,
                    QueryParams = new Dictionary<string, string>
        {
            { "name", "Vikas" },
            { "age", "21" }
        }
                };

                var stopwatch = Stopwatch.StartNew();
                try
                {
                    string response = await apiCaller.SendAsync(options, consumerKey, accessToken);

                    stopwatch.Stop();
                    Console.WriteLine("Response:");
                    Console.WriteLine(response);
                    Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Time taken before error: {stopwatch.ElapsedMilliseconds} ms");
                }

                Console.WriteLine("Do you want to call the API again? (y/n): ");
                var input = Console.ReadLine()?.Trim().ToLower();
                callAgain = input == "y" || input == "yes";
            }

            Console.WriteLine("Program ended.");
        }
    }
}