using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AcessLayer;
using Plugin.Connectivity;
using ModelLayer;

namespace BusinessLayer
{
    public class BBooks
    {

        public static int Total { get; set; }
        public ModelLayer.Users login { get; set; }

        public AcessLayer.ASqLite aSqLite { get; set; }

        public static Books.Totals GetBookshelfTotais()
        {
            Users login = SqLiteUser.RecAcesso();

            List<Books.BookSituation> lista = ABooks.GetBookshelfTotais(login.Key);

            Books.Totals BTotais = new Books.Totals();

            if (lista.Count > 0)
            {
                //aqui filtro na lista counts por status
                BTotais.IllRead = lista.Where(a => a.Situation == 1).Count();
                BTotais.Reading = lista.Where(a => a.Situation == 2).Count();
                BTotais.Read = lista.Where(a => a.Situation == 3).Count();
                BTotais.Interrupted = lista.Where(a => a.Situation == 4).Count();
            }
            else
            {
                BTotais.IllRead = BTotais.Reading = BTotais.Read = BTotais.Interrupted = 0;
            }
            return BTotais;
        }

        public static bool VerifyRegisterBook(string BookName)
        {
            Users login = SqLiteUser.RecAcesso();
            bool ret = false;
            Task.Run(async () => ret = await AcessLayer.ABooks.VerRegBook(BookName, login.Key)).Wait();
            return ret;
        }


        public static async Task RegisterBook(Books.Book book)
        {
            Users login = SqLiteUser.RecAcesso();

            book.UserKey = login.Key;
            book.LastUpdate = DateTime.Now;


            AcessLayer.ABooks.RegisterBookLocal(book);

            if (CrossConnectivity.Current.IsConnected)
                await AcessLayer.ABooks.RegisterBook(book);
        }

        public static Books.Book GetBook(string bookKey)
        {
            Users login = SqLiteUser.RecAcesso();
            return AcessLayer.ABooks.GetBook(login.Key, bookKey);
        }

        public static void UpdateSituationBook(string Key, int Situation, int Rate, string Comment)
        {
            Users login = SqLiteUser.RecAcesso();
            DateTime lastUpdate = DateTime.Now;
            ABooks.UpdateSituationBookLocal(Key, login.Key, Situation, Rate, Comment, lastUpdate);

            if (CrossConnectivity.Current.IsConnected)
            {
                ABooks.UpdateSituationBook(Key, login.Key, Situation, Rate, Comment, lastUpdate);
            }
        }

        public static async Task UpdateBook(Books.Book book)
        {
            Users login = SqLiteUser.RecAcesso();
            book.UserKey = login.Key;
            book.LastUpdate = DateTime.Now;

            AcessLayer.ABooks.UpdateBookLocal(book);
            AcessLayer.ABooks.UpdateSituationBookLocal(book.Key, book.UserKey, book.BooksSituations.Situation, Convert.ToInt32(book.BooksSituations.Rate), book.BooksSituations.Comment, book.LastUpdate);
            //
            if (CrossConnectivity.Current.IsConnected)
                await ABooks.UpdateBook(book);
        }

        /// <summary>
        /// recupera os livros pela situação
        /// </summary>
        /// <param name="Situation"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Books.Book> GetBookSituationByStatus(int Situation, int index, string textoBusca = null)
        {
            Users login = SqLiteUser.RecAcesso();

            List<Books.Book> lista = ABooks.GetBookSituationByStatus(Situation, login.Key, textoBusca);

            Total = lista.Count;

            if (Total > (index + 10))
                return lista.GetRange(index, 10);
            else
                return lista.GetRange(index, (Total - index));
        }
    }
}
