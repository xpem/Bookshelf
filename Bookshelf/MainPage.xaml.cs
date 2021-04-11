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

        private bool isLoading;
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(); } }

        private string vVouLer, vLendo, vLido, vInterrompido;

        public string VVouLer { get => vVouLer; set { vVouLer = value; OnPropertyChanged(); } }

        public string VLendo { get => vLendo; set { vLendo = value; OnPropertyChanged(); } }

        public string VLido { get => vLido; set { vLido = value; OnPropertyChanged(); } }

        public string VInterrompido { get => vInterrompido; set { vInterrompido = value; OnPropertyChanged(); } }

        private void TbiSair_Clicked(object sender, EventArgs e)
        {

            SqLiteUser.DelAcesso();
            Application.Current.MainPage = new Acessa();
            Application.Current.MainPage = new NavigationPage(new Acessa())
            {
                BarBackgroundColor = Color.FromHex("#301810"),
                BarTextColor = Color.White
            };
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


        }


        public async void CarregaBookshelfTotais()
        {
            while (BBooksLocal.Sincronizando)
            {
                this.Title = "Sincronizando...";
                await Task.Delay(3000);
            }

            BtnIllRead.IsEnabled = BtnReading.IsEnabled = BtnRead.IsEnabled = BtnInterrupted.IsEnabled = false;
            IsLoading = true;
            //
            ModelLayer.Books.Totals totais = await BusinessLayer.BBooks.GetBookshelfTotais();
            VVouLer = totais.IllRead.ToString();
            VLendo = totais.Reading.ToString();
            VLido = totais.Read.ToString();
            VInterrompido = totais.Interrupted.ToString();
            //
            BtnIllRead.IsEnabled = BtnReading.IsEnabled = BtnRead.IsEnabled = BtnInterrupted.IsEnabled = true;
            IsLoading = false;
        }
    }
}
