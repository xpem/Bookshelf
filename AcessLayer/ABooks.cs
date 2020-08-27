using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using Firebase.Database.Query;
using ModelLayer;

namespace AcessLayer
{
   public class ABooks
    {

        public async static Task<Books.Totals> GetBookshelfTotais(string KeyUsuario)
        {
            Books.Totals BTotais = new Books.Totals();

            //obtenho a lista geral(pois não consegui obter uma lista de counts independentes)
            List<Books.BookSiituation> lista = ((await AcessFirebase.firebase
              .Child("BookStatus")
              .OnceAsync<Books.BookSiituation>()).Where(a => a.Object.UserKey == KeyUsuario).Select(obj => new Books.BookSiituation { Status = obj.Object.Status })).ToList();

            if (lista != null)
            {
                //aqui filtro na lista counts por status
                BTotais.IllRead = lista.Where(a => a.Status == 1).Count();
                BTotais.Reading = lista.Where(a => a.Status == 2).Count();
                BTotais.Read = lista.Where(a => a.Status == 3).Count();
                BTotais.Interrupted = lista.Where(a => a.Status == 4).Count();
            }
            return BTotais;
        }

        /// <summary>
        /// Verify register of book in the database
        /// </summary>
        public async static Task<bool> VerRegBook(string bookName,string userKey)
        {
            if ((await AcessFirebase.firebase
               .Child("Books")
               .OnceAsync<Books.Book>()).Where(a => a.Object.BookName == bookName && a.Object.UserKey == userKey).Select(item => new Books.Book
               {
                   Key = item.Key,
               }).ToList().Count > 0)
                return false;
            else return true;
        }

        public async static Task RegisterBook(Books.Book book)
        {
            await AcessFirebase.firebase.Child("Books").PostAsync(book);
        }
    }
}
