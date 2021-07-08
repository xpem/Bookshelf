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
    public partial class NovaSenha : ContentPage
    {

        private string vNick, vUserKey;

        public string VNick
        {
            get => vNick;
            set
            {
                vNick = value; OnPropertyChanged();
            }
        }

        private async void BtnCadastrar_Clicked(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            bool ValCampos = true;

            if (string.IsNullOrEmpty(EntSenha.Text))
            {
                ValCampos = false;
            }
            else if (EntSenha.Text.Length < 4)
            {
                ValCampos = false;
            }
            if (string.IsNullOrEmpty(EntConfSenha.Text))
            {
                ValCampos = false;
            }
            if (EntConfSenha.Text == EntSenha.Text)
            {
                await DisplayAlert("Aviso", "Confirme a nova senha corretamente.", null, "Ok");
                return;
            }

            if (!ValCampos)
            {
                await DisplayAlert("Aviso", "Preencha os campos de senha corretament.", null, "Ok");
                return;
            }
            else
            {
                await new BusinessLayer.BUser().UpdateUserPassworld(vUserKey, EntConfSenha.Text);

                await DisplayAlert("Aviso", "Senha Alterada!", null, "Ok");

                Acessa pag = new Acessa();
                Application.Current.MainPage = new Acessa();
                await Navigation.PushModalAsync(pag);
            }


        }

        public NovaSenha(string Nick, string UserKey)
        {

            VNick = Nick;
            vUserKey = UserKey;
            BindingContext = this;

            InitializeComponent();
        }
    }
}