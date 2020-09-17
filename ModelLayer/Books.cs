using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer
{
  public class Books
    {

        public class Totals
        {
            public int Marked { get; set; }//
            public int IllRead { get; set; }//1
            public int Reading { get; set; }//2
            public int Read { get; set; }//3
            public int Interrupted { get; set; }//4
        }

        public class Book
        {
            public string Key { get; set; }
            public string UserKey { get; set; }
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string Authors { get; set; }
            public int Year { get; set; }
            public string Isbn { get; set; }
            public string GoogleId { get; set; }
            public int Pages { get; set; }
            public string Genre { get; set; }

            public BookSituation BooksSituations { get; set; }
        }

        public class BookSituation
        {
            public string Key { get; set; }
            public int Situation { get; set; }
            public int Rate { get; set; }
            public string Comment { get; set; }
        }

        /// <summary>
        /// item da UI da lista de livros
        /// </summary>
        public class ItemBookList
        {
            public string Key { get; set; }
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string AuthorsAndYear { get; set; }
            public string Pages { get; set; }
            public string Rate { get; set; }
        }
    }
}
