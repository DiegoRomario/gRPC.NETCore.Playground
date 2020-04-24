using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace gRPC.NETCore.Host
{
    public class StockListenerService : StockListener.StockListenerBase
    {
        private readonly ILogger<StockListenerService> _logger;
        public StockListenerService(ILogger<StockListenerService> logger)
        {
            _logger = logger;
        }


        public override async Task GetQuotesStream(Empty request, IServerStreamWriter<QuotesData> responseStream, ServerCallContext context)
        {
            Random random = new Random();
            while (!context.CancellationToken.IsCancellationRequested)
            {
                double value = random.NextDouble(5.00, 5.50);

                var quotes = new QuotesData
                { 
                    Datetime = DateTime.UtcNow.ToString(),
                    Quote = $"GOOG Stock: $ {value.ToString("f2")}"
                };

                _logger.LogInformation("Sending GOOG quotes");

                await responseStream.WriteAsync(quotes);

                await Task.Delay(500); 
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("The client cancelled their request");
            }
        }

        //public override Task SayHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        //{
        //    Random random = new Random();
        //    double value = random.NextDouble(5.00, 5.50);

        //    return Task.FromResult(new HelloReply
        //    {
        //        Message = $"Stock: {request.Name.ToUpper()}:\nPrice: $ {value.ToString("f2")}"
        //    });
        //}

    }
    public static class RandomExtensions
    {
        public static double NextDouble(
            this Random random,
            double minValue,
            double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
