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

        private int vVouLer, vLendo, vLido, vInterrompido;

        public int VVouLer { get => vVouLer; set { vVouLer = value; OnPropertyChanged(); } }

        public int VLendo { get => vLendo; set { vLendo = value; OnPropertyChanged(); } }

        public int VLido { get => vLido; set { vLido = value; OnPropertyChanged(); } }

        public int VInterrompido { get => vInterrompido; set { vInterrompido = value; OnPropertyChanged(); } }

        private void TbiSair_Clicked(object sender, EventArgs e)
        {
            BusinessLayer.SqLiteUser.DelAcesso();
            Acessa pag = new Acessa();
            Application.Current.MainPage = new Acessa();
            Navigation.PushModalAsync(pag);
        }

        private void BtnRegisterBook_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new CadastrarLivro());
        }

        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregaBookshelfTotais();
        }


        public async void CarregaBookshelfTotais()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }
            IsLoading = true;

            ModelLayer.Books.Totals totais = await BusinessLayer.BBooks.GetBookshelfTotais();
            VVouLer = totais.IllRead;
            VLendo = totais.Read;
            VLido = totais.Reading;
            VInterrompido = totais.Interrupted;
            IsLoading = false;

        }
    }
}
