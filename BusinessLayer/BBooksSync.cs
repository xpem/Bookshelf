using AcessLayer;
using AcessLayer.ABooks;
using ModelLayer;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
                        DateTime LastUptade = login.LastUpdate;

                        List<Books.Book> booksList = ABooksSqlite.GetBooksLocalByLastUpdate(login.Key, login.LastUpdate);

                        //atualiza banco fb
                        foreach (Books.Book book in booksList)
                        {
                            if (book.Key == null)
                            {
                                await ABooksFirebase.RegisterBook(book);
                            }
                            else
                            {
                                await ABooksFirebase.UpdateBook(book);
                            }
                        }

                        //atualiza banco sql
                        foreach (Books.Book book in await ABooksFirebase.GetBooksByLastUpdate(login.Key, login.LastUpdate))
                        {
                            ABooksSqlite.SyncUpdateBookLocal(book);

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
