using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Random random = new Random();
            double value = random.NextDouble(5.00, 5.50);
            return Task.FromResult(new HelloReply
            {
                Message = $"Stock: {request.Name.ToUpper()}:\nPrice: $ {value.ToString("f2")}"
            });
        }
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
