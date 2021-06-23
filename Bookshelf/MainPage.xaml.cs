using Bookshelf.Resources;
using BusinessLayer;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
            {
                VerificaSync();
            }
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
                {
                    IsConnected = "#FF0000";
                }
                else
                {
                    IsConnected = "#fff";
                    if (BBooksSync.Sincronizando)
                    {
                        IsSync = "#008000";
                    }
                    else
                    {
                        CarregaBookshelfTotais();
                        IsSync = "#fff";
                    }
                }
                await Task.Delay(10000);
            }
        }

        public async void CarregaBookshelfTotais()
        {
            //
            ModelLayer.Books.Totals totais = new ModelLayer.Books.Totals();
            _ = await Task.Run(() => totais = BBooks.GetBookshelfTotais());

            if (totais.IllRead.ToString() != VVouLer) { VVouLer = totais.IllRead.ToString(); }
            if (totais.Reading.ToString() != VLendo) { VLendo = totais.Reading.ToString(); }
            if (totais.Read.ToString() != VLido) { VLido = totais.Read.ToString(); }
            if (totais.Interrupted.ToString() != VInterrompido) { VInterrompido = totais.Interrupted.ToString(); }
            //

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

        private void btnSync_Clicked(object sender, EventArgs e)
        {
            Thread thread = new Thread(BusinessLayer.BBooksSync.AtualizaBancoLocal) { IsBackground = true };
            thread.Start();
            //inicia processo de sincronização
           // Task.Run(async () => await BusinessLayer.BBooksSync.AtualizaBancoLocal());
        }

        private void TpGstRg_Read_Tapped(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(3));
        }

        private void TpGstRg_Interrupted_Tapped(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(4));
        }

        private void TpGstRg_Reading_Tapped(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(2));
        }

        private void TpGstRg_IllRead_Tapped(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(1));
        }

        private void BtnArchive_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new BooksList(0));
        }

        #endregion
    }
}
