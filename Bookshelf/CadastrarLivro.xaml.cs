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

        //
        private bool IsUpdate = false;

        private readonly string BookKey;

        private string bTitle, bSubTitle, bVolume, bAuthors, bYear, bIsbn, bPages, bGenre, bComment, bSituation, bRate;

        public string BTitle { get => bTitle; set { bTitle = value; OnPropertyChanged(); } }

        public string BSubTitle { get => bSubTitle; set { bSubTitle = value; OnPropertyChanged(); } }

        public string BVolume { get => bVolume; set { bVolume = value; OnPropertyChanged(); } }

        public string BAuthors { get => bAuthors; set { bAuthors = value; OnPropertyChanged(); } }

        public string BYear { get => bYear; set { bYear = value; OnPropertyChanged(); } }

        public string BIsbn { get => bIsbn; set { bIsbn = value; OnPropertyChanged(); } }

        public string BPages { get => bPages; set { bPages = value; OnPropertyChanged(); } }

        public string BGenre { get => bGenre; set { bGenre = value; OnPropertyChanged(); } }

        public string BComment { get => bComment; set { bComment = value; OnPropertyChanged(); } }

        public string BSituation { get => bSituation; set { bSituation = value; OnPropertyChanged(); } }

        public string BRate { get => bRate; set { bRate = value; OnPropertyChanged(); } }

        #endregion

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
                BTitle = BSubTitle = BAuthors = BYear = BIsbn = BPages = BGenre = BVolume = "";
            }
            else
            {
                Task.Run(() => GetBook(BookKey));
            }
        }

        private void GetBook(string BookKey)
        {
            Books.Book book = BusinessLayer.BBooks.GetBook(BookKey);
            BTitle = book.Title;
            BSubTitle = book.SubTitle;
            BAuthors = book.Authors;
            BYear = book.Year.ToString();
            BIsbn = book.Isbn;
            BPages = book.Pages.ToString();
            BGenre = book.Genre;
            BVolume = book.Volume;

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
            IsUpdate = true;
        }

        private async void BtnCadastrar_Clicked(object sender, EventArgs e)
        {

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
                    Volume = EntVol.Text,
                };

                //cadastra o livro 
                string mensagem;

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
                        Situation = (Books.Situation)PkrSituation.SelectedIndex,
                        Rate = rate,
                        Comment = EdtComment.Text,
                    };
                    mensagem = "Livro e avaliação";
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

                if (!string.IsNullOrEmpty(BookKey))
                {
                    book.Key = BookKey;
                    await BusinessLayer.BBooks.UpdateBook(book);
                    mensagem += " Atualizados";
                }
                else
                {
                    await BusinessLayer.BBooks.RegisterBook(book);
                    mensagem += " Cadastrados";
                }

                bool resposta = await DisplayAlert("Aviso", mensagem, null, "Ok");

                if (!resposta)
                {
                   // Application.Current.MainPage = new MainPage();
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
                if (!IsUpdate)
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