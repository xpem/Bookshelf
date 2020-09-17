using ModelLayer;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #region Properties

        private string BookKey, bTitle, bSubTitle, bAuthors, bYear, bIsbn, bPages, bGenre, bGoogleId, bComment;

        public string BTitle
        {
            get => bTitle;
            set
            {
                bTitle = value; OnPropertyChanged();
            }
        }

        public string BSubTitle
        {
            get => bSubTitle;
            set
            {
                bSubTitle = value; OnPropertyChanged();
            }
        }

        public string BAuthors
        {
            get => bAuthors;
            set
            {
                bAuthors = value; OnPropertyChanged();
            }
        }

        public string BYear
        {
            get => bYear;
            set
            {
                bYear = value; OnPropertyChanged();
            }
        }

        public string BIsbn
        {
            get => bIsbn;
            set
            {
                bIsbn = value; OnPropertyChanged();
            }
        }

        public string BPages
        {
            get => bPages;
            set
            {
                bPages = value; OnPropertyChanged();
            }
        }

        public string BGenre
        {
            get => bGenre;
            set
            {
                bGenre = value; OnPropertyChanged();
            }
        }

        public string BGoogleId
        {
            get => bGoogleId;
            set
            {
                bGoogleId = value; OnPropertyChanged();
            }
        }

        public string BComment
        {
            get => bComment;
            set
            {
                bComment = value; OnPropertyChanged();
            }
        }


        #endregion

        private string bSituation, bRate;

        public string BSituation
        {
            get => bSituation;
            set
            {
                bSituation = value; OnPropertyChanged();
            }
        }

        public string BRate
        {
            get => bRate;
            set
            {
                bRate = value; OnPropertyChanged();
            }
        }

        public CadastrarLivro(string bookKey)
        {
            BRate = BSituation = "0";
            BookKey = bookKey;

            BindingContext = this;

            InitializeComponent();

            ObservableCollection<string> BBTStatus = new ObservableCollection<string> { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };

            PkrSituation.ItemsSource = BBTStatus;

            if (string.IsNullOrEmpty(BookKey))
            {
                PkrSituation.SelectedIndex = 0;

                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = false;
                BTitle = BSubTitle = BAuthors = BYear = BIsbn = BPages = BGenre = BGoogleId = "";
            }
            else
            {
                GetBook(BookKey);
            }
        }

        private async void GetBook(string BookKey)
        {
            Books.Book book = await BusinessLayer.BBooks.getBook(BookKey);
            BTitle = book.Title;
            BSubTitle = book.SubTitle;
            BAuthors = book.Authors;
            BYear = book.Year.ToString();
            BIsbn = book.Isbn;
            BPages = book.Pages.ToString();
            BGenre = book.Genre;
            BGoogleId = book.GoogleId;

            BComment = book.BooksSituations.Comment;

            if (book.BooksSituations.Situation > 0)
            {
                BSituation = book.BooksSituations.Situation.ToString();
                BRate = book.BooksSituations.Rate.ToString();
                BComment = book.BooksSituations.Comment;
            }
            else
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = BtnCadastrar.IsVisible = false;

                BSituation = "0";
                BRate = "";
                BComment = "";
            }

            if (PkrSituation.SelectedIndex == 3)
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = true;
            }
            else
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = false;
            }

            //StyleClass = "button_primary"
            IList<string> vs = new List<string>();
            vs.Add("button_secundary");
            BtnCadastrar.StyleClass = vs;
            BtnCadastrar.Text = "Alterar";
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
                    Title = EntTitle.Text,
                    SubTitle = EntSubTitle.Text,
                    Authors = EntAutor.Text,
                    Year = Convert.ToInt32(EntAno.Text),
                    Isbn = EntIsbn.Text,
                    Pages = Convert.ToInt32(EntPages.Text),
                    Genre = EntGenrer.Text,
                    GoogleId = EntGoogleId.Text,
                };
                //

                //cadastra o livro 
                string mensagem = "";

                //caso tenha avaliação
                if (PkrSituation.SelectedIndex > 0)
                {
                    int rate = 0;
                    if (PkrSituation.SelectedIndex == 3)
                    {
                        rate = Convert.ToInt32(SldrRate.Value);
                    }

                    book.BooksSituations = new Books.BookSituation
                    {
                        Situation = PkrSituation.SelectedIndex,
                        Rate = rate,
                        Comment = EdtComment.Text,
                    };
                    mensagem = "Livro e avaliacção";
                }
                else
                {
                    book.BooksSituations = new Books.BookSituation
                    {
                        Situation = 0,
                        Rate = 0,
                        Comment = "",
                    };

                    mensagem = "Livro";

                }

                if(!string.IsNullOrEmpty(BookKey))
                {
                    await BusinessLayer.BBooks.UpdateBook(book,BookKey);
                    mensagem += " Atualiados";
                }
                else
                {
                    await BusinessLayer.BBooks.RegisterBook(book);
                    mensagem += " Cadastrados";
                }

               

                bool resposta = await DisplayAlert("Aviso", mensagem, null, "Ok");

                if (!resposta)
                {
                    Application.Current.MainPage = new MainPage();
                    Application.Current.MainPage = new NavigationPage(new MainPage())
                    {
                        BarBackgroundColor = Color.FromHex("#301810"),
                        BarTextColor = Color.White
                    };
                }
            }
        }

        private void PkrSituation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PkrSituation.SelectedIndex == 3)
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = true;
            }
            else
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = false;
            }
        }

        private bool VerrifyFields()
        {
            bool VerFields = true;
            if (string.IsNullOrEmpty(EntTitle.Text))
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
            if (string.IsNullOrEmpty(EntPages.Text))
            {
                if (Int32.TryParse(EntPages.Text, out int pages))
                {
                    if (pages <= 0)
                    {
                        VerFields = false;
                    }
                }
                else
                {
                    VerFields = false;
                }

            }
            if (string.IsNullOrEmpty(EntGenrer.Text))
            {
                VerFields = false;
            }

            if (!VerFields)
            {
                DisplayAlert("Aviso", "Preencha os campos obrigatórios", null, "Ok");
            }
            else
            {
                VerFields = BusinessLayer.BBooks.VerifyRegisterBook(EntTitle.Text);
                if (!VerFields)
                {
                    DisplayAlert("Aviso", "Livro já cadastrados", null, "Ok");
                }
            }
            return VerFields;

        }


    }
}