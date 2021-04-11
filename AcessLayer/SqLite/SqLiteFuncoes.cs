using Microsoft.Data.Sqlite;
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
    }
}
