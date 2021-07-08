using AcessLayer.Firebase;
using AcessLayer.SqLite;
using ModelLayer;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BBooksSync : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public static bool Sincronizando { get; set; }

        /// <summary>
        /// Carrega o banco de dados local do usuário
        /// </summary>
        /// <returns></returns>
        public async static void AtualizaBancoLocal()
        {
            try
            {
                BUser bUser = new BUser();               
                ABooksSqlite aBooksSqlite = new ABooksSqlite();
                ABooksFirebase aBooksFirebase = new ABooksFirebase();

                Users login = bUser.GetUserLocal();
                bool ProcessoContinuo = true;

                while (ProcessoContinuo)
                {
                    //usuario nao está logado
                    if (login == null)
                    {
                        break;
                    }

                    Sincronizando = true;
                    if (CrossConnectivity.Current.IsConnected)
                    {                      
                        DateTime LastUptade = login.LastUpdate;

                        List<Books.Book> booksList = await aBooksSqlite.GetBooksLocalByLastUpdate(login.Key, login.LastUpdate);

                        //atualiza banco fb
                        foreach (Books.Book book in booksList)
                        {

                            //caso o livro esteja com uma chave temporária local, cadastrá-lo no firebase
                            if (Guid.TryParse(book.Key, out _))
                            {
                                //seta a key como nula para cadastrar o livro no firebase
                                book.Key = null;
                              
                                await aBooksFirebase.AddBook(book);
                            }
                            else
                            {
                                await aBooksFirebase.UpdateBook(book);
                            }
                        }

                        //atualiza banco sql
                        foreach (Books.Book book in await aBooksFirebase.GetBooksByLastUpdate(login.Key, login.LastUpdate))
                        {
                            aBooksSqlite.SyncUpdateBook(book);

                            if (LastUptade < book.LastUpdate) LastUptade = book.LastUpdate;
                        }
                        bUser.UpdateUserLastUpdadeLocal(login.Key, LastUptade);

                    }

                    Sincronizando = false;
                    //de tres em tres minutos checa atualizações
                    await Task.Delay(180000);
                }
            }
            catch (Exception ex) { throw ex; }
        }


    }
}
