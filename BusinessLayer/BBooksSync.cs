using AcessLayer;
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
        public async static Task AtualizaBancoLocal()
        {
            try
            {
                Users login = SqLiteUser.RecAcessoLastUpdate();

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

                        //atualiza banco fb
                        foreach (Books.Book book in AcessLayer.ABooks.GetBooksLocalByLastUpdate(login.Key, login.LastUpdate))
                        {
                            if (book.Key == null)
                            {
                                await ABooks.RegisterBook(book);
                            }
                            else
                            {
                                await ABooks.UpdateBook(book);
                            }
                        }

                        //atualiza banco sql
                        foreach (Books.Book book in await AcessLayer.ABooks.GetBooksByLastUpdate(login.Key, login.LastUpdate))
                        {
                            ABooks.UpdateBookLocal(book);
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
