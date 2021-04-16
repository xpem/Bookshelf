using Bookshelf.Resources;
using BusinessLayer;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bookshelf
{
    public partial class MainPage : ContentPage
    {
        #region variaveis

        private bool isLoading;
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(); } }

        private string vVouLer, vLendo, vLido, vInterrompido, isSync, isConnected;

        public string VVouLer { get => vVouLer; set { vVouLer = value; OnPropertyChanged(); } }

        public string VLendo { get => vLendo; set { vLendo = value; OnPropertyChanged(); } }

        public string VLido { get => vLido; set { vLido = value; OnPropertyChanged(); } }

        public string VInterrompido { get => vInterrompido; set { vInterrompido = value; OnPropertyChanged(); } }

        public string IsSync { get => isSync; set { isSync = value; OnPropertyChanged(); } }

        public string IsConnected { get => isConnected; set { isConnected = value; OnPropertyChanged(); } }
        /// <summary>
        /// variavel que define se a função que verifica a sincronização já está rodando ou não.
        /// </summary>
        private static bool VerificandoSync { get; set; }

        #endregion

        public MainPage()
        {
            BindingContext = this;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            VVouLer = VLendo = VLido = VInterrompido = "...";

            CarregaBookshelfTotais();

            if (!VerificandoSync)
                VerificaSync();
        }

        /// <summary>
        /// Operação paralela que verifica se o sistema está sincronizando ou nao.
        /// Irei verificar a utilidade disso quando estiver refazendo a interface
        /// </summary>
        private async void VerificaSync()
        {
            VerificandoSync = true;

            while (VerificandoSync)
            {
                if (!CrossConnectivity.Current.IsConnected)
                    IsConnected = "#FF0000";
                else
                {
                    IsConnected = "#fff";
                    if (BBooksSync.Sincronizando) IsSync = "#008000";
                    else IsSync = "#fff";
                }
                await Task.Delay(5000);
            }
        }

        public async void CarregaBookshelfTotais()
        {
            BtnIllRead.IsEnabled = BtnReading.IsEnabled = BtnRead.IsEnabled = BtnInterrupted.IsEnabled = false;
            //
            ModelLayer.Books.Totals totais = new ModelLayer.Books.Totals();
            await Task.Run(() => totais = BusinessLayer.BBooks.GetBookshelfTotais());
            VVouLer = totais.IllRead.ToString();
            VLendo = totais.Reading.ToString();
            VLido = totais.Read.ToString();
            VInterrompido = totais.Interrupted.ToString();
            //
            BtnIllRead.IsEnabled = BtnReading.IsEnabled = BtnRead.IsEnabled = BtnInterrupted.IsEnabled = true;
        }

        #region navegação

        private async void TbiSair_Clicked(object sender, EventArgs e)
        {
            var resp = await DisplayAlert("Confirmação", "Deseja sair e retornar a tela inicial?", "Sim", "Cancelar");

            if (resp)
            {
                SqLiteUser.DelAcesso();
                Application.Current.MainPage = new Acessa();
                Application.Current.MainPage = new NavigationPage(new Acessa())
                {
                    BarBackgroundColor = Color.FromHex("#301810"),
                    BarTextColor = Color.White
                };
            }
        }

        private void BtnRegisterBook_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new CadastrarLivro(""));
        }

        private void BtnVouLer_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(1));
        }

        private void BtnReading_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(2));
        }

        private void BtnRead_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(3));
        }

        private void BtnInterrupted_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(4));
        }

        private void BtnArchive_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(-1));
        }

        #endregion
    }
}
