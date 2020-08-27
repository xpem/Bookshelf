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
    public partial class CadastrarLivro : ContentPage
    {
        public CadastrarLivro()
        {
            InitializeComponent();
        }

        private async void BtnCadastrar_Clicked(object sender, EventArgs e)
        {
         

            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            if (VerrifyFields())
            {
                BtnCadastrar.IsEnabled = false;
                Books.Book book = new Books.Book
                {
                    BookName = EntNome.Text,
                    Author = EntAutor.Text,
                    Year = EntAno.Text,
                    Isbn = EntIsbn.Text,
                };
                //
                await BusinessLayer.BBooks.RegisterBook(book);


                bool resposta = await DisplayAlert("Aviso", "Livro cadastrado", null, "Ok");

                if (!resposta)
                {
                    MainPage pag = new MainPage();
                    Application.Current.MainPage = new MainPage();
                    await Navigation.PushModalAsync(pag);
                }
            }
        }


        private bool VerrifyFields()
        {
            bool VerFields = true;
            if (string.IsNullOrEmpty(EntNome.Text))
            {
                VerFields = false;
            }
            if (string.IsNullOrEmpty(EntAutor.Text))
            {
                VerFields = false;
            }
            if (string.IsNullOrEmpty(EntAno.Text))
            {
                VerFields = false;
            }

            if (!VerFields)
            {
                DisplayAlert("Aviso", "Preencha os campos Nome do livro, Autor  e Ano", null, "Ok");
            }
            else
            {
                VerFields = BusinessLayer.BBooks.VerifyRegisterBook(EntNome.Text);
                if (!VerFields)
                {
                    DisplayAlert("Aviso", "Livro já cadastrados", null, "Ok");
                }
            }
            return VerFields;

        }


    }
}