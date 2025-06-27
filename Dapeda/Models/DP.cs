using System;
using Dapper;// Dapper: Mikro ORM kütüphanesi. SQL işlemlerini sadeleştirir.
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;

namespace Dapeda.Models
{
    public class DP
    {
        public static string connectionString = "Server=localhost; Database=Market; User Id=SA; Password=reallyStrongPwd123; TrustServerCertificate=True";
        public static void ExecuteReturn(string procadi, DynamicParameters param = null) //EKLEME GÜNCELLEME SİLME // Metodu: Stored Procedure çağır, sonuç döndürme
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Execute(procadi, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public static IEnumerable<T> Listeleme<T>(string procadi, DynamicParameters param = null)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.Query<T>(procadi, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public static T ExecuteScalar<T>(string procadi, DynamicParameters param = null)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.ExecuteScalar<T>(procadi, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public static T GetSingle<T>(string procadi, DynamicParameters param = null)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.QueryFirstOrDefault<T>(procadi, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public static IEnumerable<TFirst> ListelemeMultiMapping<TFirst, TSecond>(
        string procadi,
        Func<TFirst, TSecond, TFirst> map,
        string splitOn = "Id",
        DynamicParameters param = null)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.Query<TFirst, TSecond, TFirst>(
                    procadi,
                    map,
                    param,
                    commandType: System.Data.CommandType.StoredProcedure,
                    splitOn: splitOn
                );
            }
        }
        public static T Get<T>(string procadi, DynamicParameters param = null)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.QueryFirstOrDefault<T>(procadi, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        public static int Execute(string procadi, DynamicParameters param = null)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.Execute(procadi, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        public static T QuerySingle<T>(string procName, DynamicParameters param)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                var command = new CommandDefinition(procName, param, commandType: CommandType.StoredProcedure);
                return db.QuerySingle<T>(command);
            }
        }
        
    }
}


