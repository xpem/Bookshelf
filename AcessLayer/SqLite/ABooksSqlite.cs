using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshelf.AcessLayer.SqLite;
//
using Firebase.Database.Query;
using Microsoft.Data.Sqlite;
using ModelLayer;
using static ModelLayer.Books;

namespace AcessLayer.SqLite
{
    public class ABooksSqlite
    {
        /// <summary>
        /// Atualiza o livro e sua situação do banco de dados local
        /// </summary>
        /// <param name="book"></param>
        public static void SyncUpdateBookLocal(Book book)
        {
            //verificar a data da ultima atualização

            DateTime? lastUpdate = GetLastUpdateBook(book.Key,book.Title);

            if (lastUpdate == null && !book.Inativo)
            {
                RegisterBookLocal(book);
            }
            else if (book.LastUpdate > lastUpdate)
            {

                UpdateBookLocal(book);

                SqLiteFunctions.OpenIfClosed();

                SqliteCommand UpdateCommand = new SqliteCommand
                {
                    Connection = ASqLite.db,
                    CommandText = "update BOOKSITUATIONS set Situation = @Situation,Rate = @Rate,Comment = @Comment where BookKey = @Key and UserKey = @UserKey"
                };
                UpdateCommand.Parameters.AddWithNullableValue("@Key", book.Key);
                UpdateCommand.Parameters.AddWithNullableValue("@UserKey", book.UserKey);
                UpdateCommand.Parameters.AddWithNullableValue("@Situation", book.BooksSituations.Situation);
                UpdateCommand.Parameters.AddWithNullableValue("@Rate", book.BooksSituations.Rate);
                UpdateCommand.Parameters.AddWithNullableValue("@Comment", book.BooksSituations.Comment);
                UpdateCommand.ExecuteReader();

                SqLiteFunctions.CloseIfOpen();
            }
        }

        public static void RegisterBookLocal(Book book)
        {
            SqLiteFunctions.OpenIfClosed();

            using (SqliteCommand insertCommand = new SqliteCommand
            {
                Connection = ASqLite.db,
                CommandText = "insert into BOOK(KEY, UserKey, Title, SubTitle, Authors, Year, Volume, Pages, Genre, LastUpdate, Isbn) values(@Key, @UserKey, @Title, @SubTitle, @Authors, @Year, @Volume, @Pages, @Genre, @LastUpdate,@Isbn)"
            })
            {
                insertCommand.Parameters.AddWithNullableValue("@Key", book.Key);
                insertCommand.Parameters.AddWithNullableValue("@UserKey", book.UserKey);
                insertCommand.Parameters.AddWithNullableValue("@Title", book.Title);
                insertCommand.Parameters.AddWithNullableValue("@SubTitle", book.SubTitle);
                insertCommand.Parameters.AddWithNullableValue("@Authors", book.Authors);
                insertCommand.Parameters.AddWithNullableValue("@Year", book.Year);
                insertCommand.Parameters.AddWithNullableValue("@Volume", book.Volume);
                insertCommand.Parameters.AddWithNullableValue("@Pages", book.Pages);
                insertCommand.Parameters.AddWithNullableValue("@Genre", book.Genre);
                insertCommand.Parameters.AddWithNullableValue("@LastUpdate", book.LastUpdate);
                insertCommand.Parameters.AddWithNullableValue("@Isbn", book.Isbn);
                insertCommand.ExecuteReader();
            }

            using (SqliteCommand insertCommand = new SqliteCommand
            {
                Connection = ASqLite.db,
                CommandText = "insert into BOOKSITUATIONS(BookKey,UserKey,Situation,Rate,Comment) values (@Key, @UserKey, @Situation, @Rate, @Comment)"
            })
            {
                insertCommand.Parameters.AddWithNullableValue("@Key", book.Key);
                insertCommand.Parameters.AddWithNullableValue("@UserKey", book.UserKey);
                insertCommand.Parameters.AddWithNullableValue("@Situation", book.BooksSituations.Situation);
                insertCommand.Parameters.AddWithNullableValue("@Rate", book.BooksSituations.Rate);
                insertCommand.Parameters.AddWithNullableValue("@Comment", book.BooksSituations.Comment);
                insertCommand.ExecuteReader();
            }

            SqLiteFunctions.CloseIfOpen();
        }

        public static void UpdateSituationBookLocal(string Key, string UserKey, int Situation, int Rate, string Comment, DateTime lastUpdate)
        {
            SqLiteFunctions.OpenIfClosed();

            SqliteCommand UpdateCommand = new SqliteCommand
            {
                Connection = ASqLite.db,
                CommandText = "update BOOKSITUATIONS set Situation = @Situation,Rate = @Rate,Comment = @Comment where BookKey = @Key and UserKey = @UserKey"
            };
            UpdateCommand.Parameters.AddWithNullableValue("@Key", Key);
            UpdateCommand.Parameters.AddWithNullableValue("@UserKey", UserKey);
            UpdateCommand.Parameters.AddWithNullableValue("@Situation", Situation);
            UpdateCommand.Parameters.AddWithNullableValue("@Rate", Rate);
            UpdateCommand.Parameters.AddWithNullableValue("@Comment", Comment);
            UpdateCommand.ExecuteReader();

            UpdateCommand = new SqliteCommand
            {
                Connection = ASqLite.db,
                CommandText = "update BOOK set LastUpdate = @LastUpdate where KEY = @Key and UserKey = @UserKey"
            };
            UpdateCommand.Parameters.AddWithNullableValue("@Key", Key);
            UpdateCommand.Parameters.AddWithNullableValue("@UserKey", UserKey);
            UpdateCommand.Parameters.AddWithNullableValue("@LastUpdate", lastUpdate);
            UpdateCommand.ExecuteReader();

            SqLiteFunctions.CloseIfOpen();
        }

        public static void UpdateBookLocal(Book book)
        {
            SqLiteFunctions.OpenIfClosed();

            SqliteCommand UpdateCommand = new SqliteCommand
            {
                Connection = ASqLite.db,
                CommandText = "update BOOK set Title = @Title, SubTitle = @SubTitle, Authors = @Authors, Year = @Year, Volume = @Volume, Pages = @Pages" +
                 ", Genre = @Genre, LastUpdate = @LastUpdate,Isbn = @Isbn,Inativo = @Inativo where KEY = @Key and UserKey = @UserKey"
            };
            UpdateCommand.Parameters.AddWithNullableValue("@Key", book.Key);
            UpdateCommand.Parameters.AddWithNullableValue("@UserKey", book.UserKey);
            UpdateCommand.Parameters.AddWithNullableValue("@Title", book.Title);
            UpdateCommand.Parameters.AddWithNullableValue("@SubTitle", book.SubTitle);
            UpdateCommand.Parameters.AddWithNullableValue("@Authors", book.Authors);
            UpdateCommand.Parameters.AddWithNullableValue("@Year", book.Year);
            UpdateCommand.Parameters.AddWithNullableValue("@Volume", book.Volume);
            UpdateCommand.Parameters.AddWithNullableValue("@Pages", book.Pages);
            UpdateCommand.Parameters.AddWithNullableValue("@Genre", book.Genre);
            UpdateCommand.Parameters.AddWithNullableValue("@LastUpdate", book.LastUpdate);
            UpdateCommand.Parameters.AddWithNullableValue("@Isbn", book.Isbn);

            //Livro desativado
            if(book.Inativo) UpdateCommand.Parameters.AddWithNullableValue("@Inativo", "1");

            UpdateCommand.ExecuteReader();

            SqLiteFunctions.CloseIfOpen();
        }

        /// <summary>
        /// Inativa o livro no banco local
        /// </summary>
        /// <param name="book"></param>
        public static void InactivateBookLocal(Book book)
        {
            SqLiteFunctions.OpenIfClosed();

            SqliteCommand UpdateCommand = new SqliteCommand
            {
                Connection = ASqLite.db,
                CommandText = "update BOOK set Inativo = '1',LastUpdate = @LastUpdate where KEY = @Key and UserKey = @UserKey"
            };
            UpdateCommand.Parameters.AddWithNullableValue("@Key", book.Key);
            UpdateCommand.Parameters.AddWithNullableValue("@UserKey", book.UserKey);
            UpdateCommand.Parameters.AddWithNullableValue("@LastUpdate", book.LastUpdate);
            UpdateCommand.ExecuteReader();

            SqLiteFunctions.CloseIfOpen();
        }

        /// <summary>
        /// busca a data de ultima atualzação do livro
        /// </summary>
        /// <param name="KEY">nulo caso o livro esteja no momento cadastrado apenas localmente</param>
        /// <param name="Title"></param>
        /// <returns></returns>
        public static DateTime? GetLastUpdateBook(string KEY,string Title)
        {
            SqLiteFunctions.OpenIfClosed();
            string query = "select LastUpdate from BOOK where";

            if (string.IsNullOrEmpty(KEY))
            {
                query += " KEY = @Key";
            }
            else
            {
                query += " title = @Title";
            }


            SqliteCommand selectCommand = new SqliteCommand(query, ASqLite.db);


            if (string.IsNullOrEmpty(KEY))
            {
                selectCommand.Parameters.AddWithValue("@Key", KEY);
            }
            else
            {
                selectCommand.Parameters.AddWithValue("@Title", Title);
            }

            var resposta = (string)selectCommand.ExecuteScalar();

            SqLiteFunctions.CloseIfOpen();

            if (string.IsNullOrEmpty(resposta)) return null;
            else return Convert.ToDateTime(resposta);
        }

        public static List<BookSituation> GetBookshelfTotais(string userKey)
        {
            SqLiteFunctions.OpenIfClosed();

            SqliteCommand selectCommand = new SqliteCommand("select bs.Situation from BOOKSITUATIONS bs inner join BOOK b on  bs.BookKey = b.Key where b.UserKey = @userKey and b.Inativo is null", ASqLite.db);
            selectCommand.Parameters.AddWithValue("@userKey", userKey);
            SqliteDataReader query = selectCommand.ExecuteReader();

            List<Books.BookSituation> lista = new List<BookSituation>();

            while (query.Read())
            {
                lista.Add(new Books.BookSituation() { Situation = query.GetInt32(0) });
            }

            SqLiteFunctions.CloseIfOpen();

            return lista;

        }

        public static List<Book> GetBookSituationByStatus(int Situation, string UserKey, string textoBusca)
        {
            try
            {
                SqLiteFunctions.OpenIfClosed();
                string query = "select b.key,b.title,b.Authors,b.Year,b.Volume,b.Pages,b.Genre,b.LastUpdate,b.SubTitle from BOOK b";

                if (Situation > 0)
                    query += " inner join BOOKSITUATIONS bs on bs.BookKey = b.key where b.UserKey = @userKey and bs.situation = @situation";
                else
                    query += " where b.UserKey = @userKey";

                if (!string.IsNullOrEmpty(textoBusca))
                    query += " and b.title like @textoBusca";

                query += " and b.Inativo is null";

                SqliteCommand selectCommand = new SqliteCommand(query, ASqLite.db);
                selectCommand.Parameters.AddWithValue("@userKey", UserKey);

                if (Situation > 0)
                    selectCommand.Parameters.AddWithValue("@situation", Situation);

                if (!string.IsNullOrEmpty(textoBusca))
                    selectCommand.Parameters.AddWithValue("@textoBusca", "%"+textoBusca+ "%");

                SqliteDataReader Retorno = selectCommand.ExecuteReader();

                List<Book> lista = new List<Book>();

                while (Retorno.Read())
                {
                    lista.Add(new Books.Book()
                    {
                        Key = Retorno.GetWithNullableString(0),
                        Title = Retorno.GetWithNullableString(1),
                        Authors = Retorno.GetWithNullableString(2),
                        Year = Retorno.GetInt32(3),
                        Volume = Retorno.GetWithNullableString(4),
                        Pages = Retorno.GetInt32(5),
                        Genre = Retorno.GetWithNullableString(6),
                        LastUpdate = Convert.ToDateTime(Retorno.GetWithNullableString(7)),
                        SubTitle = Retorno.GetWithNullableString(8)
                    });
                }

                SqLiteFunctions.CloseIfOpen();

                return lista;
            }
            catch (Exception ex) { throw ex; }
        }

        public static List<Book> GetBooksLocalByLastUpdate(string userKey, DateTime lastUpdate)
        {
            try
            {
                SqLiteFunctions.OpenIfClosed();
                string query = "select b.title,b.Authors,b.Year,b.Volume,b.Pages,b.Genre,b.LastUpdate,b.SubTitle,b.Isbn,bs.Rate,bs.situation,bs.comment,b.userKey,b.Key,b.inativo from BOOK b inner join BOOKSITUATIONS bs on bs.BookKey = b.key where b.UserKey = @userKey and b.lastUpdate >= @lastUpdate";
                SqliteCommand selectCommand = new SqliteCommand(query, ASqLite.db);
                selectCommand.Parameters.AddWithValue("@userKey", userKey);
                selectCommand.Parameters.AddWithValue("@lastUpdate", lastUpdate);

                SqliteDataReader Retorno = selectCommand.ExecuteReader();

                List<Book> lista = new List<Book>();

                while (Retorno.Read())
                {
                    lista.Add(new Books.Book()
                    {
                        Title = Retorno.GetWithNullableString(0),
                        Authors = Retorno.GetWithNullableString(1),
                        Year = Retorno.GetInt32(2),
                        Volume = Retorno.GetWithNullableString(3),
                        Pages = Retorno.GetInt32(4),
                        Genre = Retorno.GetWithNullableString(5),
                        LastUpdate = Convert.ToDateTime(Retorno.GetWithNullableString(6)),
                        SubTitle = Retorno.GetWithNullableString(7),
                        Isbn = Retorno.GetWithNullableString(8),
                        BooksSituations = new BookSituation() { Rate = Retorno.GetWithNullableInt(9), Situation = Retorno.GetInt32(10), Comment = Retorno.GetWithNullableString(11) },
                        UserKey = Retorno.GetWithNullableString(12),
                        Key = Retorno.GetWithNullableString(13),
                        Inativo = Retorno.GetWithNullableBool(14)
                    });
                }

                SqLiteFunctions.CloseIfOpen();

                return lista;
            }
            catch (Exception ex) { throw ex; }
        }



        public static Book GetBook(string userKey, string bookKey)
        {
            try
            {
                SqLiteFunctions.OpenIfClosed();
                string query = "select b.key,b.title,b.Authors,b.Year,b.Volume,b.Pages,b.Genre,b.LastUpdate,b.SubTitle,b.Isbn,bs.Rate,bs.situation,bs.comment from BOOK b inner join BOOKSITUATIONS bs on bs.BookKey = b.key where b.UserKey = @userKey and b.Key = @key";
                SqliteCommand selectCommand = new SqliteCommand(query, ASqLite.db);
                selectCommand.Parameters.AddWithValue("@userKey", userKey);
                selectCommand.Parameters.AddWithValue("@key", bookKey);
                SqliteDataReader Retorno = selectCommand.ExecuteReader();
                Retorno.Read();

                var book = new Books.Book()
                {
                    Key = Retorno.GetWithNullableString(0),
                    Title = Retorno.GetWithNullableString(1),
                    Authors = Retorno.GetWithNullableString(2),
                    Year = Retorno.GetInt32(3),
                    Volume = Retorno.GetWithNullableString(4),
                    Pages = Retorno.GetInt32(5),
                    Genre = Retorno.GetWithNullableString(6),
                    LastUpdate = Convert.ToDateTime(Retorno.GetWithNullableString(7)),
                    SubTitle = Retorno.GetWithNullableString(8),
                    Isbn = Retorno.GetWithNullableString(9),
                    BooksSituations = new BookSituation() { Rate = Retorno.GetWithNullableInt(10), Situation = Retorno.GetInt32(11), Comment = Retorno.GetWithNullableString(12) }
                };

                SqLiteFunctions.CloseIfOpen();

                return book;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
