using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AcessLayer.SqLite;
using Plugin.Connectivity;
using ModelLayer;
using AcessLayer.Firebase;
using static ModelLayer.Books;

namespace BusinessLayer
{
    public class BBooks
    {

        public static int Total { get; set; }

        public Users Login { get; set; }

        public static Books.Totals GetBookshelfTotais()
        {
            Users login = SqLiteUser.RecAcesso();
            Books.Totals BTotais = new Books.Totals();
            if (login.Key != null)
            {
                List<BookSituation> lista = ABooksSqlite.GetBookshelfTotais(login.Key);

                if (lista.Count > 0)
                {
                    //aqui filtro na lista counts por status
                    BTotais.IllRead = lista.Where(a => a.Situation == Situation.IllRead).Count();
                    BTotais.Reading = lista.Where(a => a.Situation == Situation.Reading).Count();
                    BTotais.Read = lista.Where(a => a.Situation == Situation.Read).Count();
                    BTotais.Interrupted = lista.Where(a => a.Situation == Situation.Interrupted).Count();
                }
                else
                {
                    BTotais.IllRead = BTotais.Reading = BTotais.Read = BTotais.Interrupted = 0;
                }
            }
            return BTotais;
        }

        public static bool VerifyRegisterBook(string BookName)
        {
            Users login = SqLiteUser.RecAcesso();
            bool ret = false;
            IABooksFirebase _myService = new ABooksFirebase();
            Task.Run(async () => ret = await _myService.VerifyBook(BookName, login.Key)).Wait();
            return ret;
        }

        public static async Task RegisterBook(Books.Book book)
        {
            Users login = SqLiteUser.RecAcesso();

            book.UserKey = login.Key;
            book.LastUpdate = DateTime.Now;

            if (CrossConnectivity.Current.IsConnected)
            {
                IABooksFirebase _myService = new ABooksFirebase();
                book.Key = await _myService.AddBook(book);
            }
            else
            {
                //gera um id local temporário único
                book.Key = Guid.NewGuid().ToString();
            }
            ABooksSqlite.RegisterBookLocal(book);


        }

        public static Books.Book GetBook(string bookKey)
        {
            Users login = SqLiteUser.RecAcesso();
            return ABooksSqlite.GetBook(login.Key, bookKey);
        }

        public static void UpdateSituationBook(string Key, Situation Situation, int Rate, string Comment)
        {
            Users login = SqLiteUser.RecAcesso();
            DateTime lastUpdate = DateTime.Now;
            ABooksSqlite.UpdateSituationBookLocal(Key, login.Key, Situation, Rate, Comment, lastUpdate);

            if (CrossConnectivity.Current.IsConnected)
            {
                IABooksFirebase _myService = new ABooksFirebase();
                _myService.UpdateBookSituation(Key, login.Key, Situation, Rate, Comment, lastUpdate);
            }
        }

        public static async Task UpdateBook(Books.Book book)
        {
            Users login = SqLiteUser.RecAcesso();
            book.UserKey = login.Key;
            book.LastUpdate = DateTime.Now;

            ABooksSqlite.UpdateBookLocal(book);
            ABooksSqlite.UpdateSituationBookLocal(book.Key, book.UserKey, book.BooksSituations.Situation, Convert.ToInt32(book.BooksSituations.Rate), book.BooksSituations.Comment, book.LastUpdate);
            //
            if (CrossConnectivity.Current.IsConnected)
            {
                IABooksFirebase _myService = new ABooksFirebase();
                await _myService.UpdateBook(book);
            }
        }

        public static async void InactivateBook(string bookKey)
        {
            Users login = SqLiteUser.RecAcesso();
            Books.Book book = ABooksSqlite.GetBook(login.Key, bookKey);
            book.UserKey = login.Key;
            book.LastUpdate = DateTime.Now;
            book.Inativo = true;
            ABooksSqlite.InactivateBookLocal(book);
            if (CrossConnectivity.Current.IsConnected)
            {
                IABooksFirebase _myService = new ABooksFirebase();
                await _myService.InactivateBook(book);
            }

        }

        /// <summary>
        /// recupera os livros pela situação
        /// </summary>
        /// <param name="Situation"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<List<Books.Book>> GetBookSituationByStatus(int Situation, int index, string textoBusca = null)
        {
            Users login = SqLiteUser.RecAcesso();

            List<Books.Book> lista = await ABooksSqlite.GetBookSituationByStatus(Situation, login.Key, textoBusca);

            Total = lista.Count;

            if (Total > (index + 10))
                return lista.GetRange(index, 10);
            else
                return lista.GetRange(index, (Total - index));
        }
    }
}
