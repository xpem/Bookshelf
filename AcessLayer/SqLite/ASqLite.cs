using Bookshelf.AcessLayer.SqLite;
using Microsoft.Data.Sqlite;
using ModelLayer;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ModelLayer.Books;

namespace AcessLayer.SqLite
{
    public static class ASqLite
    {

        public readonly static SqliteConnection db = new SqliteConnection($"Filename={System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf.db3")}");

        /// <summary>
        /// aumento da versao de alguma tabela força a atualização da mesma
        /// </summary>
        public static DbVersions ActualDbVersions = new DbVersions() { ACESSADD = 1, BOOK = 15 };

        public static void CriaDb()
        {

            SqLiteFunctions.OpenIfClosed();

            VerDbVersions();

            string command = "create table if not exists ACESSADD (ID integer primary key autoincrement, KEY text, LOGINNOME text, LastUpdate DATETIME);";
            SqliteCommand createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            command = "create table if not exists BOOK (KEY text, UserKey text, Title text, SubTitle text, Authors text, Year integer, Volume text, Pages integer, Isbn text, Genre text, LastUpdate DATETIME,Inativo integer);";
            createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            command = "create table if not exists BOOKSITUATIONS (BookKey text, UserKey text, Situation integer, Rate integer, Comment text);";
            createTable = new SqliteCommand(command, db);
            createTable.ExecuteReader();
            createTable = new SqliteCommand("create table if not exists Versiondb (ACESSADD integer,BOOK integer);", db);
            createTable.ExecuteReader();

            SqLiteFunctions.CloseIfOpen();
        }

        /// <summary>
        /// deleta as tabelas de versão anterior para serem recriadas.
        /// </summary>
        public static void VerDbVersions()
        {
            //SqliteCommand sqliteCommand = new SqliteCommand("drop table Versiondb", db);
            //sqliteCommand.ExecuteReader();
            //
            SqliteCommand sqliteCommand = new SqliteCommand("create table if not exists Versiondb (Key integer, ACESSADD integer,BOOK integer);", db);
            sqliteCommand.ExecuteReader();
            //

            sqliteCommand = new SqliteCommand("select ACESSADD,BOOK from Versiondb", db);
            DbVersions dbVersions;

            using (SqliteDataReader Retorno = sqliteCommand.ExecuteReader())
            {
                Retorno.Read();

                if (Retorno.HasRows)
                {
                    dbVersions = new DbVersions()
                    {
                        ACESSADD = Retorno.GetWithNullableInt(0),
                        BOOK = Retorno.GetWithNullableInt(1)
                    };

                }
                else
                {
                    dbVersions = new DbVersions()
                    {
                        ACESSADD = 0,
                        BOOK = 0
                    };
                    InsereAtualizaVersiondb(false, dbVersions);
                }
            }

            SqliteCommand command;
            bool atualizarVersaoDb = false;

            if ((dbVersions.BOOK < ActualDbVersions.BOOK) || (dbVersions.ACESSADD < ActualDbVersions.ACESSADD))
            {
                command = new SqliteCommand("drop table if exists ACESSADD", db);
                command.ExecuteReader();
                atualizarVersaoDb = true;
            }
            if (dbVersions.BOOK < ActualDbVersions.BOOK)
            {
                command = new SqliteCommand("drop table if exists BOOK", db);
                command.ExecuteReader();

                command = new SqliteCommand("drop table if exists BOOKSITUATIONS", db);
                command.ExecuteReader();

                atualizarVersaoDb = true;
            }

            if (atualizarVersaoDb)
                InsereAtualizaVersiondb(true, ActualDbVersions);
        }


        public static void CadastraAcesso(string id, string login)
        {
            SqLiteFunctions.OpenIfClosed();
            SqliteCommand insertCommand = new SqliteCommand
            {
                Connection = db,
                CommandText = "insert into ACESSADD (KEY,LOGINNOME,LASTUPDATE) values (@KEY,@LOGINNOME,@LASTUPDATE)"
            };
            insertCommand.Parameters.AddWithNullableValue("@KEY", id);
            insertCommand.Parameters.AddWithNullableValue("@LOGINNOME", login);
            insertCommand.Parameters.AddWithNullableValue("@LASTUPDATE", DateTime.MinValue);
            insertCommand.ExecuteReader();
            SqLiteFunctions.CloseIfOpen();
        }

        public static void DelAcesso()
        {
            SqLiteFunctions.OpenIfClosed();
            SqliteCommand insertCommand = new SqliteCommand { Connection = db, CommandText = "delete from ACESSADD" };
            insertCommand.ExecuteReader();
            SqLiteFunctions.CloseIfOpen();
        }

        public static Users RecAcesso()
        {
            SqLiteFunctions.OpenIfClosed();

            SqliteCommand selectCommand = new SqliteCommand("select KEY,LOGINNOME,LASTUPDATE from ACESSADD", db);
            SqliteDataReader Retorno = selectCommand.ExecuteReader();
            Retorno.Read();

            if (Retorno.HasRows)
            {
                var user = new Users()
                {
                    Key = Retorno.GetWithNullableString(0),
                    Nick = Retorno.GetWithNullableString(1),
                    LastUpdate = Retorno.GetDateTime(2)
                };
                SqLiteFunctions.CloseIfOpen();

                return user;
            }
            else
            {
                SqLiteFunctions.CloseIfOpen();
                return null;
            }
        }

        public static void AtualizaAcessoLastUpdade(string Key, DateTime LastUpdate)
        {
            SqLiteFunctions.OpenIfClosed();

            SqliteCommand insertCommand = new SqliteCommand
            {
                Connection = db,
                CommandText = "update ACESSADD set LastUpdate = @LastUpdate where key = @Key"
            };
            insertCommand.Parameters.AddWithNullableValue("@Key", Key);
            insertCommand.Parameters.AddWithNullableValue("@LastUpdate", LastUpdate);
            insertCommand.ExecuteReader();

            SqLiteFunctions.CloseIfOpen();
        }

        public static void InsereAtualizaVersiondb(bool isUpdate, DbVersions dbVersions)
        {
            SqliteCommand Command;

            if (!isUpdate)
            {
                Command = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "insert into Versiondb(key,ACESSADD,BOOK) values ('1',@ACESSADD,@BOOK)"
                };
            }
            else
            {
                Command = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "update Versiondb set ACESSADD = @ACESSADD,BOOK = @BOOK where key = '1'"
                };
            }
            Command.Parameters.AddWithNullableValue("@ACESSADD", dbVersions.ACESSADD);
            Command.Parameters.AddWithNullableValue("@BOOK", dbVersions.BOOK);
            Command.ExecuteReader();
        }

    }
}
