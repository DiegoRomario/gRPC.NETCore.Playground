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
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var client = new StockListener.StockListenerClient(channel);

            try
            {
                await foreach (var quote in client.GetQuotesStream(new Stock { Ticker = "GOOG" }).ResponseStream.ReadAllAsync())
                {
                    var date = quote.Datetime;

                    Console.WriteLine($"{quote.Ticker} = {date:s}");
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
