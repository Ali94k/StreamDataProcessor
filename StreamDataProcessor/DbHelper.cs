using System.Collections.Generic;
using System.Linq;

using DataModels;

using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

using Microsoft.Extensions.Logging;

namespace StreamDataProcessor
{
    public class DbHelper
    {
        private readonly ILogger<DbHelper> _logger;

        public DbHelper(ILogger<DbHelper> logger)
        {
            _logger = logger;
        }
        
        public void RecordUserData(UserTx tx)
        {
            DataConnection.DefaultSettings = new MySettings();

            using var db = new UserDataDB();

            var updatedRecords = db.DailyTxes
                .Where(x => x.UserId == tx.UserId && x.TxDate == tx.TxDate.Date)
                .Set(x => x.TotalAmount, x => x.TotalAmount + tx.TotalAmount)
                .Set(x => x.TxCount, x => x.TxCount + 1)
                .Update();

            if (updatedRecords == 0)
            {
                db.Insert(new DailyTx
                {
                    UserId = tx.UserId,
                    TxDate = tx.TxDate.Date,
                    TotalAmount = tx.TotalAmount,
                    TxCount = 1
                });
            }

            _logger.LogInformation("Recorded user data {tx.UserId}", tx.UserId);
        }

        private class ConnectionStringSettings : IConnectionStringSettings
        {
            public string ConnectionString { get; set; }
            public string Name { get; set; }
            public string ProviderName { get; set; }
            public bool IsGlobal => false;
        }

        private class MySettings : ILinqToDBSettings
        {
            public IEnumerable<IDataProviderSettings> DataProviders
                => Enumerable.Empty<IDataProviderSettings>();

            public string DefaultConfiguration => "SqlServer";
            public string DefaultDataProvider => "SqlServer";

            public IEnumerable<IConnectionStringSettings> ConnectionStrings
            {
                get
                {
                    yield return
                        new ConnectionStringSettings
                        {
                            Name = "UserData",
                            ProviderName = ProviderName.SqlServer,
                            ConnectionString =
                                @"Server=localhost;Database=UserData;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
                        };
                }
            }
        }
    }
}
