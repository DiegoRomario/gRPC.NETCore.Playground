using gRPC.NETCore.Host;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gRPC.NETCore.Client
{
    class Program
    {
        static async Task Main()
        {            
            Console.WriteLine("Which stock would you like to subscribe to get quotes?\n");
            string STOCK = Console.ReadLine();

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new StockListener.StockListenerClient(channel);
            try
            {
                await foreach (var quote in client.GetQuotesStream(new Stock { Ticker = STOCK }).ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"\nDate: {quote.Datetime:s}\nTicker: {quote.Ticker}");
                    if (quote.Percentagechange.Contains("-"))
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Quote: {quote.Quote}\nVariation Value: { quote.Variationvalue}\nPercentage Change: { quote.Percentagechange}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Prior Quote: { quote.Priorquote}\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{new String('-', 50)}");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading response: " + ex);
            }

        }
    }
}
