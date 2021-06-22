using AcessLayer.Firebase;
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
                Users login = SqLiteUser.RecAcesso();

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
                        IABooksFirebase _myService = new ABooksFirebase();
                        DateTime LastUptade = login.LastUpdate;

                        List<Books.Book> booksList = AcessLayer.SqLite.ABooksSqlite.GetBooksLocalByLastUpdate(login.Key, login.LastUpdate);

                        //atualiza banco fb
                        foreach (Books.Book book in booksList)
                        {

                            //caso o livro esteja com uma chave temporária local, cadastrá-lo no firebase
                            if (Guid.TryParse(book.Key, out _))
                            {
                                //seta a key como nula para cadastrar o livro no firebase
                                book.Key = null;
                              
                                await _myService.AddBook(book);
                            }
                            else
                            {
                                await _myService.UpdateBook(book);
                            }
                        }

                        //atualiza banco sql
                        foreach (Books.Book book in await _myService.GetBooksByLastUpdate(login.Key, login.LastUpdate))
                        {
                            AcessLayer.SqLite.ABooksSqlite.SyncUpdateBookLocal(book);

                            if (LastUptade < book.LastUpdate) LastUptade = book.LastUpdate;
                        }
                        SqLiteUser.AtualizaAcessoLastUpdade(login.Key, LastUptade);

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
