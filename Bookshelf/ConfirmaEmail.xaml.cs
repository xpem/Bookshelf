using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bookshelf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmaEmail : ContentPage
    {
        public ConfirmaEmail()
        {
            InitializeComponent();
        }

        private void BtnConfirmar_Clicked(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }


            if (string.IsNullOrEmpty(EntEmail.Text))
            {
                DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return;
            }
            else if (!BusinessLayer.BUser.Valida_email(EntEmail.Text))
            {
                DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return;
            }
            else
            {
                string UserNick, UserKey;

                bool resposta = BusinessLayer.BUser.RecoverUserEmail(EntEmail.Text, out UserNick, out UserKey);

                if (!resposta)
                {
                    DisplayAlert("Aviso", "Email não encontrado", null, "Ok");
                    return;
                }
                else
                {
                 this.Navigation.PushAsync(new NovaSenha(UserNick, UserKey));
                }

            }


        }
    }
}