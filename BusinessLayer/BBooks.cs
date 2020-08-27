using ModelLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
  public  class BBooks
    {

        public static int Total { get; set; }


        public async static Task<Books.Totals> GetBookshelfTotais()
        {
            Users login = SqLiteUser.RecAcesso();
            return await AcessLayer.ABooks.GetBookshelfTotais(login.Key);
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

            await AcessLayer.ABooks.RegisterBook(book);
        }

    }
}
