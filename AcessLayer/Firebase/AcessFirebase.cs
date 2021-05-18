using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;

using System;

namespace AcessLayer
{
    public class AcessFirebase
    {
        public static FirebaseClient firebase = new FirebaseClient("https://bookshelf-11701.firebaseio.com/");
    }
}
