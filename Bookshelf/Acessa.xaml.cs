using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bookshelf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Acessa : ContentPage
    {
        public Acessa()
        {
            InitializeComponent();
        }

        private void BtnCadAcesso_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new CadastrarUsuario());
        }

        private void BtnAcessa_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(EntNomeAcesso.Text) && !string.IsNullOrEmpty(EntSenha.Text))
            {
                if (EntSenha.Text.Length > 3)
                {
                    BtnCadAcesso.IsEnabled = false;
                    bool resp = false;

                    //To upper para garantir que tudo esteja maiúsculo independente do dispositivo
                    Task.Run(async () => resp = await BusinessLayer.BUser.RecoverUser(EntNomeAcesso.Text.ToUpper(), EntSenha.Text)).Wait();

                    if (resp)
                    {
                        Thread thread = new Thread(BusinessLayer.BBooksSync.AtualizaBancoLocal) { IsBackground = true };
                        thread.Start();
                        //inicia processo de sincronização
                      //  Task.Run(async () => await BusinessLayer.BBooksSync.AtualizaBancoLocal());

                        Application.Current.MainPage = new MainPage();
                        Application.Current.MainPage = new NavigationPage(new MainPage())
                        {
                            BarBackgroundColor = Color.FromHex("#301810"),
                            BarTextColor = Color.White
                        };
                    }
                    else
                    {
                        DisplayAlert("Aviso", "Usuário/senha incorretos", null, "Ok");
                    }
                    BtnCadAcesso.IsEnabled = true;
                }
                else
                {
                    DisplayAlert("Aviso", "Digite sua senha", null, "Ok");
                }
            }
        }

        private void BtnRecSenha_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new ConfirmaEmail());
        }
    }
}