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

        public async static Task<List<Books.BookSituation>> GetBookshelfTotais(string KeyUsuario)
        {
            List<Books.BookSituation> lista = new List<Books.BookSituation>();

            //obtenho a lista geral(pois não consegui obter uma lista de counts independentes)
            return ((await AcessFirebase.firebase
              .Child("Books")
              .OnceAsync<Books.Book>())
              .Where(a => a.Object.UserKey == KeyUsuario).Select(obj => new Books.BookSituation { Situation = ((obj.Object.BooksSituations == null) ? 0 : obj.Object.BooksSituations.Situation) })).ToList();

        }

        /// <summary>
        /// Verify register of book in the database
        /// </summary>
        public async static Task<bool> VerRegBook(string bookName, string userKey)
        {
            if ((await AcessFirebase.firebase
               .Child("Books")
               .OnceAsync<Books.Book>()).Where(a => a.Object.Title == bookName && a.Object.UserKey == userKey).Select(item => new Books.Book
               {
                   Key = item.Key,
               }).ToList().Count > 0)
                return false;
            else return true;
        }

        public async static Task<string> RegisterBook(Books.Book book)
        {
            var resp = await AcessFirebase.firebase.Child("Books").PostAsync(book);
            return resp.Key;
        }

        public async static Task<Books.Book> GetBook(string userKey, string bookKey)
        {
            return (await AcessFirebase.firebase
              .Child("Books")
              .OnceAsync<Books.Book>()).Where(a => a.Key == bookKey && a.Object.UserKey == userKey)
              .Select(item => new Books.Book
              {
                  Key = item.Key,
                  Title = item.Object.Title,
                  Authors = item.Object.Authors,
                  Pages = item.Object.Pages,
                  Genre = item.Object.Genre,
                  Year = item.Object.Year,
                  SubTitle = item.Object.SubTitle,
                  GoogleId = item.Object.GoogleId,
                  BooksSituations = item.Object.BooksSituations,
                  Isbn = item.Object.Isbn,
              }).ToList().FirstOrDefault();
        }

        public async static Task<List<Books.Book>> GetBookSituations(string vUserKey)
        {
            return (await AcessFirebase.firebase
             .Child("Books")
             .OnceAsync<Books.Book>()).Where(a => a.Object.UserKey == vUserKey).Select(item => new Books.Book
             {
                 Key = item.Key,
                 Title = item.Object.Title,
                 Authors = item.Object.Authors,
                 Pages = item.Object.Pages,
                 Genre = item.Object.Genre,
                 Year = item.Object.Year,
                 SubTitle = item.Object.SubTitle,
                 BooksSituations = item.Object.BooksSituations,
             }).ToList();
        }

        public async static Task<List<Books.Book>> GetBookSituationByStatus(int vSituation, string vUserKey)
        {
            return (await AcessFirebase.firebase
              .Child("Books")
              .OnceAsync<Books.Book>()).Where(a => a.Object.UserKey == vUserKey && a.Object.BooksSituations.Situation == vSituation).Select(item => new Books.Book
              {
                  Key = item.Key,
                  Title = item.Object.Title,
                  Authors = item.Object.Authors,
                  Pages = item.Object.Pages,
                  Genre = item.Object.Genre,
                  Year = item.Object.Year,
                  SubTitle = item.Object.SubTitle,
                  BooksSituations = item.Object.BooksSituations,
              }).ToList();
        }

        public async static Task UpdateSituationBook(string vKey, string vUserKey, int vSituation, int vRate, string vComment)
        {
            var toUpdateBookStatus = (await AcessFirebase.firebase
              .Child("Books")
              .OnceAsync<Books.Book>()).Where(a => a.Key == vKey && a.Object.UserKey == vUserKey).FirstOrDefault().Object;

            toUpdateBookStatus.BooksSituations.Situation = vSituation;
            toUpdateBookStatus.BooksSituations.Rate = vRate;
            toUpdateBookStatus.BooksSituations.Comment = vComment;

            await AcessFirebase.firebase
              .Child("Books")
              .Child(vKey)
              .PutAsync(toUpdateBookStatus);
        }

        public async static Task UpdateBook(Books.Book book,string bookKey)
        {

            var toUpdateBookStatus = (await AcessFirebase.firebase
             .Child("Books")
             .OnceAsync<Books.Book>()).Where(a => a.Key == bookKey && a.Object.UserKey == book.UserKey).FirstOrDefault().Object;

            toUpdateBookStatus = book;


            await AcessFirebase.firebase
              .Child("Books")
              .Child(bookKey)
              .PutAsync(toUpdateBookStatus);
        }



    }
}
