using ModelLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ModelLayer.Books;

namespace AcessLayer.Firebase
{
    public interface IABooksFirebase
    {
        Task<bool> VerifyBook(string bookName, string userKey);

        Task<string> AddBook(Book book);

        Task UpdateBook(Book book);

        Task<List<Book>> GetBooksByLastUpdate(string vUserKey, DateTime vLastUpdate);

        Task InactivateBook(Book book);

        void UpdateBookSituation(string Key, string UserKey, Situation Situation, int Rate, string Comment, DateTime lastUpdate);
    }
}
