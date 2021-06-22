using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Connectivity;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BusinessLayer;

namespace Bookshelf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CadastrarUsuario : ContentPage
    {
        public CadastrarUsuario()
        {
            InitializeComponent();
        }

        private bool ValidaCadastro()
        {
            bool ValCampos = true;
            if (string.IsNullOrEmpty(EntNome.Text))
            {
                ValCampos = false;
            }
            if (string.IsNullOrEmpty(EntEmail.Text))
            {
                ValCampos = false;
            }
            else if (!BUser.Valida_email(EntEmail.Text))
            {
                DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(EntNomeAcesso.Text))
            {
                ValCampos = false;
            }
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
            if (EntConfSenha.Text.ToUpper() != EntSenha.Text.ToUpper())
            {
                ValCampos = false;
            }

            if (!ValCampos)
            {
                DisplayAlert("Aviso", "Preencha os campos e confirme a senha corretamente", null, "Ok");
            }
            else
            {
                ValCampos = BUser.VerificaCadUsuario(EntNome.Text, EntEmail.Text.ToUpper());
                if (!ValCampos)
                {
                    DisplayAlert("Aviso", "Nome de Acesso/Email já cadastrados", null, "Ok");
                }
            }
            return ValCampos;
        }

        private async void BtnCadastrar_Clicked(object sender, EventArgs e)
        {    
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            if (ValidaCadastro())
            {
                BtnCadastrar.IsEnabled = false;
                Users user = new Users
                {
                    UserName = EntNome.Text,
                    Nick = EntNomeAcesso.Text,
                    Email = EntEmail.Text.ToUpper(),
                    Passworld = BUser.CPEncrypt(EntSenha.Text, EntSenha.Text.Length)

                };
                //
                await BUser.CadastraUsuario(user);


                bool resposta = await DisplayAlert("Aviso", "Usuário cadastrado", null, "Ok");

                if (!resposta)
                {
                    Acessa pag = new Acessa();
                    Application.Current.MainPage = new Acessa();
                    await Navigation.PushModalAsync(pag);
                }
            }
        }
    }
}