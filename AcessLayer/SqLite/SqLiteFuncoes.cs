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

        public static string GetWithNullableString(this SqliteDataReader sqliteDataReader, int ordinal)
        {
            return !sqliteDataReader.IsDBNull(ordinal) ? sqliteDataReader.GetString(ordinal) : null;
        }

        public static bool GetWithNullableBool(this SqliteDataReader sqliteDataReader, int ordinal)
        {
            return !sqliteDataReader.IsDBNull(ordinal) ? sqliteDataReader.GetBoolean(ordinal) : false;
        }

        public static int? GetWithNullableInt(this SqliteDataReader sqliteDataReader, int ordinal)
        {
            return !sqliteDataReader.IsDBNull(ordinal) ? sqliteDataReader.GetInt32(ordinal) : (int?)null;
        }
    }
}
