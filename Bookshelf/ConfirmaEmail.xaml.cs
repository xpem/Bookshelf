using ModelLayer;
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
            else if (!BusinessLayer.BUser.Valida_email(EntEmail.Text.ToUpper()))
            {
                DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return;
            }
            else
            {
                Users user = BusinessLayer.BUser.RecoverUserEmail(EntEmail.Text.ToUpper());

                if (user == null)
                {
                    DisplayAlert("Aviso", "Email não encontrado", null, "Ok");
                    return;
                }
                else
                {
                  this.Navigation.PushAsync(new NovaSenha(user.Nick, user.Key));
                }

            }


        }
    }
}