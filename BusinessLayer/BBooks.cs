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

        readonly ABooksSqlite aBooksSqlite = new ABooksSqlite();
        readonly ABooksFirebase aBooksFirebase = new ABooksFirebase();
        readonly BUser bUser = new BUser();

        public BBooks()
        {
            Login = bUser.GetUserLocal();
        }

        public Totals GetBookshelfTotais()
        {
            Totals BTotais = new Totals();

            if (Login.Key != null)
            {
                List<BookSituation> lista = aBooksSqlite.GetBookshelfTotals(Login.Key);

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


        /// <summary>
        /// verifica se o livro existe no banco firebase
        /// </summary>
        /// <param name="BookName"></param>
        /// <returns></returns>
        public bool VerifyRegisterBook(string BookName)
        {
            bool ret = false;

            Task.Run(async () => ret = await (aBooksFirebase).VerifyBook(BookName, Login.Key)).Wait();
            return ret;
        }

        public async Task RegisterBook(Books.Book book)
        {

            book.UserKey = Login.Key;
            book.LastUpdate = DateTime.Now;

            if (CrossConnectivity.Current.IsConnected)
            {
                book.Key = await aBooksFirebase.AddBook(book);
            }
            else
            {
                //gera um id local temporário único
                book.Key = Guid.NewGuid().ToString();
            }

            aBooksSqlite.AddBook(book);


        }

        public Book GetBook(string bookKey) => (aBooksSqlite).GetBook(Login.Key, bookKey);

        public void UpdateBookSituation(string Key, Situation Situation, int Rate, string Comment)
        {
            DateTime lastUpdate = DateTime.Now;

            aBooksSqlite.UpdateBookSituation(Key, Login.Key, Situation, Rate, Comment, lastUpdate);

            if (CrossConnectivity.Current.IsConnected)
            {
                aBooksFirebase.UpdateBookSituation(Key, Login.Key, Situation, Rate, Comment, lastUpdate);
            }
        }

        public async Task UpdateBook(Book book)
        {
            book.UserKey = Login.Key;
            book.LastUpdate = DateTime.Now;

            aBooksSqlite.UpdateBook(book);
            aBooksSqlite.UpdateBookSituation(book.Key, book.UserKey, book.BooksSituations.Situation, Convert.ToInt32(book.BooksSituations.Rate), book.BooksSituations.Comment, book.LastUpdate);
            //
            if (CrossConnectivity.Current.IsConnected)
            {
                await aBooksFirebase.UpdateBook(book);
            }
        }

        public async void InactivateBook(string bookKey)
        {
            Book book = aBooksSqlite.GetBook(Login.Key, bookKey);

            book.UserKey = Login.Key;
            book.LastUpdate = DateTime.Now;
            book.Inativo = true;

            aBooksSqlite.InactivateBook(book);

            if (CrossConnectivity.Current.IsConnected)
            {
                await aBooksFirebase.InactivateBook(book);
            }

        }

        /// <summary>
        /// recupera os livros pela situação
        /// </summary>
        /// <param name="Situation"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<List<Book>> GetBookSituationByStatus(int Situation, int index, string textoBusca = null)
        {
            List<Book> lista = await aBooksSqlite.GetBookSituationByStatus(Situation, Login.Key, textoBusca);

            Total = lista.Count;

            return Total > (index + 10) ? lista.GetRange(index, 10) : lista.GetRange(index, Total - index);
        }
    }
}
