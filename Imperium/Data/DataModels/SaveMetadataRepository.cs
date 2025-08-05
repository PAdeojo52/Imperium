using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Imperium.Data.DataInterfaces;

namespace Imperium.Data.Models
{
    public class SaveMetadataRepository : ISaveMetadataRepository
    {
        private readonly IDbConnection _connection;

        public SaveMetadataRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Insert(SaveMetadata metadata)
        {
            const string sql = @"  
                   INSERT INTO SaveMetadata (SaveName, SaveTime)  
                   VALUES (@SaveName, @SaveTime);";

            _connection.Execute(sql, metadata); // Dapper's Execute method  
        }
    }
}
