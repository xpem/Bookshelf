using AcessLayer.SqLite;
using Microsoft.Data.Sqlite;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookshelf.AcessLayer.SqLite
{
    /// <summary>
    /// funções auxiliares para as funções relacionadas ao SqLite
    /// </summary>
    public static class SqLiteFunctions
    {
        /// <summary>
        /// verificação de nulo
        /// </summary>
        public static SqliteParameter AddWithNullableValue(this SqliteParameterCollection collection, string parameterName, object value)
        {
            if (value == null)
                return collection.AddWithValue(parameterName, DBNull.Value);
            else
                return collection.AddWithValue(parameterName, value);
        }

        public static void OpenIfClosed() { if (ASqLite.db.State == System.Data.ConnectionState.Closed) { ASqLite.db.Open(); } }

        public static void CloseIfOpen() { if (ASqLite.db.State == System.Data.ConnectionState.Open) { ASqLite.db.Close(); } }

        public static string GetWithNullableString(this SqliteDataReader sqliteDataReader, int ordinal)
        {
            if (!sqliteDataReader.IsDBNull(ordinal))
                return sqliteDataReader.GetString(ordinal);
            else
                return null;
        }

        public static bool GetWithNullableBool(this SqliteDataReader sqliteDataReader, int ordinal)
        {
            if (!sqliteDataReader.IsDBNull(ordinal))
                return sqliteDataReader.GetBoolean(ordinal);
            else
                return false;
        }

        public static int? GetWithNullableInt(this SqliteDataReader sqliteDataReader, int ordinal)
        {
            if (!sqliteDataReader.IsDBNull(ordinal))
                return sqliteDataReader.GetInt32(ordinal);
            else
                return null;
        }
    }
}
