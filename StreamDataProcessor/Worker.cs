using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace StreamDataProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly StreamReader _streamReader;
        private readonly DbHelper _dbHelper;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(StreamReader streamReader, DbHelper dbHelper, IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger)
        {
            _streamReader = streamReader;
            _dbHelper = dbHelper;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

                    await Task.Run(() => processData(stoppingToken), stoppingToken);
                }

                _streamReader.Dispose();
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
        
        private void processData(CancellationToken stoppingToken)
        {
            var cr = _streamReader.Read(stoppingToken);

            var msg = cr?.Message?.Value;

            if (msg == null) return;
            
            _logger.LogInformation("message: {Msg}", msg);
            processMsg(msg);
        }
        
        private void processMsg(string msg)
        {
            var userTx = JsonConvert.DeserializeObject<UserTx>(msg);

            _dbHelper.RecordUserData(userTx);
        }
    }
}
