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
    /// Firebase Book Table access
    /// </summary>
    public class ABooksFirebase : IABooksFirebase
    {
        /// <summary>
        /// Verify register of book in database by name of book
        /// </summary>
        /// <returns>true if it exists</returns>
        public async Task<bool> VerifyBook(string bookName, string userKey)
        {
            if ((await AcessFirebase.firebase.Child("Books")
               .OnceAsync<Book>()).Where(a => a.Object.Title == bookName && a.Object.UserKey == userKey).Select(item =>
               new Book { Key = item.Key, }).ToList().Count > 0)
                return false;
            else return true;
        }

        /// <summary>
        /// Add a book
        /// </summary>
        /// <param name="book"></param>
        /// <returns>Key of book added</returns>
        public async Task<string> AddBook(Book book)
        {
            return (await AcessFirebase.firebase.Child("Books").PostAsync(book)).Key;
        }

        /// <summary>
        /// Modify a book
        /// </summary>
        /// <param name="book"></param>
        public async Task UpdateBook(Book book)
        {
            //recover the complete object of the book to be modified
            Book toUpdateBookStatus = (await AcessFirebase.firebase.Child("Books")
             .OnceAsync<Book>()).Where(a => a.Key == book.Key && a.Object.UserKey == book.UserKey).FirstOrDefault().Object;

            toUpdateBookStatus = book;

            await AcessFirebase.firebase.Child("Books").Child(book.Key).PutAsync(toUpdateBookStatus);
        }

        /// <summary>
        /// recover list of books by yours last update
        /// </summary>
        /// <param name="vUserKey"></param>
        /// <param name="vLastUpdate"></param>
        /// <returns></returns>
        public async Task<List<Book>> GetBooksByLastUpdate(string vUserKey, DateTime vLastUpdate)
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

        /// <summary>
        /// update a read situation of a book
        /// </summary>
        public async void UpdateBookSituation(string Key, string UserKey, Situation Situation, int Rate, string Comment, DateTime lastUpdate)
        {
            Book toUpdateBookStatus = (await AcessFirebase.firebase.Child("Books").OnceAsync<Book>()).FirstOrDefault(a => a.Key == Key && a.Object.UserKey == UserKey).Object;
            //
            toUpdateBookStatus.BooksSituations.Situation = Situation;
            toUpdateBookStatus.BooksSituations.Rate = Rate;
            toUpdateBookStatus.BooksSituations.Comment = Comment;
            toUpdateBookStatus.LastUpdate = lastUpdate;
            //
            await AcessFirebase.firebase.Child("Books").Child(Key).PutAsync(toUpdateBookStatus);
        }

        /// <summary>
        /// Inactivate the book, excluding a book from the listing
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public async Task InactivateBook(Book book)
        {
            var toUpdateBookStatus = (await AcessFirebase.firebase
             .Child("Books")
             .OnceAsync<Book>()).Where(a => a.Key == book.Key && a.Object.UserKey == book.UserKey).FirstOrDefault().Object;

            toUpdateBookStatus = book;

            await AcessFirebase.firebase.Child("Books").Child(book.Key).PutAsync(toUpdateBookStatus);
        }
    }
}
