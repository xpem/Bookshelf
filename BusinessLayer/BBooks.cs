using ModelLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AcessLayer;

namespace BusinessLayer
{
    public class BBooks
    {

        public static int Total { get; set; }
        public Users login { get; set; }

        public AcessLayer.ASqLite aSqLite { get; set; }




        public async static Task<Books.Totals> GetBookshelfTotais()
        {
            Users login = SqLiteUser.RecAcesso();

            List<Books.BookSituation> lista = ASqLite.GetBookshelfTotais(login.Key);

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

        //public async static Task<Books.Totals> GetBookshelfTotais()
        //{
        //    Users login = SqLiteUser.RecAcesso();

        //    List<Books.BookSituation> lista = await AcessLayer.ABooks.GetBookshelfTotais(login.Key);

        //    Books.Totals BTotais = new Books.Totals();

        //    if (lista.Count > 0)
        //    {
        //        //aqui filtro na lista counts por status
        //        BTotais.IllRead = lista.Where(a => a.Situation == 1).Count();
        //        BTotais.Reading = lista.Where(a => a.Situation == 2).Count();
        //        BTotais.Read = lista.Where(a => a.Situation == 3).Count();
        //        BTotais.Interrupted = lista.Where(a => a.Situation == 4).Count();
        //    }
        //    else
        //    {
        //        BTotais.IllRead = BTotais.Reading = BTotais.Read = BTotais.Interrupted = 0;
        //    }
        //    return BTotais;
        //}

        public async Task<List<Books.Book>> GetBookSituationByStatus(int Situation, int index)
        {

            login = SqLiteUser.RecAcesso();

            List<Books.Book> lista = new List<Books.Book>();

            if (Situation == -1)
            { lista = await AcessLayer.ABooks.GetBookSituations(login.Key); }
            else
                lista = await AcessLayer.ABooks.GetBookSituationByStatus(Situation, login.Key);

            Total = lista.Count;

            if (Total > (index + 10))
                return lista.GetRange(index, 10);
            else
                return lista.GetRange(index, (Total - index));
        }


     


        public static bool VerifyRegisterBook(string BookName)
        {
            Users login = SqLiteUser.RecAcesso();
            bool ret = false;
            Task.Run(async () => ret = await AcessLayer.ABooks.VerRegBook(BookName, login.Key)).Wait();
            return ret;
        }


        public static async Task<string> RegisterBook(Books.Book book)
        {
            Users login = SqLiteUser.RecAcesso();

            book.UserKey = login.Key;
            book.LastUpdate = DateTime.Now;

            return await AcessLayer.ABooks.RegisterBook(book);
        }

        public static async Task<Books.Book> getBook(string bookKey)
        {
            Users login = SqLiteUser.RecAcesso();

            return await AcessLayer.ABooks.GetBook(login.Key, bookKey);
        }

        public static async Task UpdateSituationBook(string vKey, int vSituation, int vRate, string vComment)
        {
            Users login = SqLiteUser.RecAcesso();
            await AcessLayer.ABooks.UpdateSituationBook(vKey, login.Key, vSituation, vRate, vComment, DateTime.Now);
        }

        public static async Task UpdateBook(Books.Book book, string userKey)
        {
            Users login = SqLiteUser.RecAcesso();
            book.UserKey = login.Key;
            book.LastUpdate = DateTime.Now;
            await AcessLayer.ABooks.UpdateBook(book, userKey);
        }
    }
}
