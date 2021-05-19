using Firebase.Database.Query;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelLayer.Books;

namespace AcessLayer.Firebase
{
    /// <summary>
    /// acesso às tabelas de books no firebase
    /// </summary>
    public class ABooksFirebase
    {
        /// <summary>
        /// Verify register of book in the database
        /// </summary>
        public async static Task<bool> VerRegBook(string bookName, string userKey)
        {
            if ((await AcessFirebase.firebase
               .Child("Books")
               .OnceAsync<Book>()).Where(a => a.Object.Title == bookName && a.Object.UserKey == userKey).Select(item => new Books.Book
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

        public async static Task<List<Book>> GetBooksByLastUpdate(string vUserKey, DateTime vLastUpdate)
        {
            return (await AcessFirebase.firebase
             .Child("Books")
             .OnceAsync<Book>()).Where(a => a.Object.UserKey == vUserKey && a.Object.LastUpdate >= vLastUpdate).Select(item => new Books.Book
             {
                 Key = item.Key,
                 UserKey = item.Object.UserKey,
                 Title = item.Object.Title,
                 Authors = item.Object.Authors,
                 Pages = item.Object.Pages,
                 Genre = item.Object.Genre,
                 Year = item.Object.Year,
                 SubTitle = item.Object.SubTitle,
                 BooksSituations = item.Object.BooksSituations,
                 Volume = item.Object.Volume,
                 LastUpdate = item.Object.LastUpdate,
                 Isbn = item.Object.Isbn,
                 Inativo = item.Object.Inativo                 
             }).ToList();
        }

        public async static void UpdateSituationBook(string Key, string UserKey, int Situation, int Rate, string Comment, DateTime lastUpdate)
        {
            Book toUpdateBookStatus = (await AcessFirebase.firebase.Child("Books").OnceAsync<Books.Book>()).Where(a => a.Key == Key && a.Object.UserKey == UserKey).FirstOrDefault().Object;
            //
            toUpdateBookStatus.BooksSituations.Situation = Situation;
            toUpdateBookStatus.BooksSituations.Rate = Rate;
            toUpdateBookStatus.BooksSituations.Comment = Comment;
            toUpdateBookStatus.LastUpdate = lastUpdate;
            //
            await AcessFirebase.firebase.Child("Books").Child(Key).PutAsync(toUpdateBookStatus);
        }

        public async static Task UpdateBook(Book book)
        {
            var toUpdateBookStatus = (await AcessFirebase.firebase
             .Child("Books")
             .OnceAsync<Book>()).Where(a => a.Key == book.Key && a.Object.UserKey == book.UserKey).FirstOrDefault().Object;

            toUpdateBookStatus = book;

            await AcessFirebase.firebase.Child("Books").Child(book.Key).PutAsync(toUpdateBookStatus);
        }

        /// <summary>
        /// Inativa o livro, excluindo ele das listajens.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public async static Task InactivateBook(Book book)
        {
            var toUpdateBookStatus = (await AcessFirebase.firebase
             .Child("Books")
             .OnceAsync<Book>()).Where(a => a.Key == book.Key && a.Object.UserKey == book.UserKey).FirstOrDefault().Object;

            toUpdateBookStatus = book;

            await AcessFirebase.firebase.Child("Books").Child(book.Key).PutAsync(toUpdateBookStatus);
        }
    }
}
