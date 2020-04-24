using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using gRPC.NETCore.Host.Entensions;
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

        public override async Task GetQuotesStream(Stock request, IServerStreamWriter<QuotesData> responseStream, ServerCallContext context)
        {
            Random random = new Random();
            double previousValue = 0, variationValue = 0, percentageChange = 0, currentValue = 0;
            string bearOrBull = string.Empty;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                currentValue = random.NextDouble(1276.00, 1285.50);
                bearOrBull = (previousValue > currentValue ? "🐻" : "🐂");
                variationValue = currentValue - (previousValue == 0 ? currentValue : previousValue);
                percentageChange = (previousValue > 0 ? ((currentValue / previousValue) * 100) - 100 : 0);
                var quotes = new QuotesData
                {
                    Datetime = DateTime.UtcNow.ToString(),
                    Ticker = request.Ticker,                    
                    Quote = currentValue.ToString("f2"),
                    Priorquote = previousValue.ToString("f2"),
                    Variationvalue = variationValue.ToString("f2"),
                    Percentagechange = $"{percentageChange.ToString("f2")}%",
                    Details = $"{bearOrBull}"
                };

                previousValue = currentValue;
                _logger.LogInformation($"Sending {request} quotes");
                await responseStream.WriteAsync(quotes);

                await Task.Delay(1200); 
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("The client cancelled their request");
            }
        }

    }
}
