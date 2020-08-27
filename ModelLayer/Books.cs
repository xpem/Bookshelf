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

        public class BookSiituation
        {
            public string Key { get; set; }
            public string BookKey { get; set; }
            public string UserKey { get; set; }
            public int Status { get; set; }
            public int Rate { get; set; }
        }

        public class Book
        {
            public string Key { get; set; }
            public string UserKey { get; set; }
            public string BookName { get; set; }
            public string Author { get; set; }
            public string Year { get; set; }
            public string Isbn { get; set; }
            public string Pages { get; set; }
            public string Genre { get; set; }
        }
    }
}
