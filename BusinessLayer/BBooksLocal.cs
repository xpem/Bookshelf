using ModelLayer;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BBooksLocal
    {
        /// <summary>
        /// define se está em processo de sincronização
        /// </summary>
        public static bool Sincronizando { get; set; }

        /// <summary>
        /// Carrega o banco de dados local do usuário
        /// </summary>
        /// <returns></returns>
        public async Task AtualizaBancoLocal()
        {
            try
            {
                Users login = SqLiteUser.RecAcessoLastUpdate(); ;

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

                        foreach (Books.Book book in await AcessLayer.ABooks.GetBooksByLastUpdate(login.Key, login.LastUpdate))
                        {
                            try
                            {
                              AcessLayer.ASqLite.CadastraLivroLocal(book);

                            }
                            catch (Exception ex) { throw ex; }
                            if (LastUptade < book.LastUpdate) LastUptade = book.LastUpdate;
                        }
                        SqLiteUser.AtualizaAcessoLastUpdade(login.Key, LastUptade);
                    }
                    Sincronizando = false;
                    //de tres em tres minutos checa atualizações
                    await Task.Delay(18000);
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
