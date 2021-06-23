using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bookshelf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailBook : ContentPage
    {

        #region variaveis do bind.

        public string bTitle, bAuthors, bPages, bGenre, bSituation, bLblSituationtext, bRate, BookKey, bComment, bSubtitleAndVol, bVolume;

        public string BTitle { get => bTitle; set { bTitle = value; OnPropertyChanged(); } }
        public string BAuthors { get => bAuthors; set { bAuthors = value; OnPropertyChanged(); } }
        public string BPages { get => bPages; set { bPages = value; OnPropertyChanged(); } }

        public string BGenre
        {
            get => bGenre;
            set
            {
                bGenre = value; OnPropertyChanged();
            }
        }

        #endregion

        public readonly ObservableCollection<string> BBTSituation = new ObservableCollection<string> { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };

        private string BSituationOri { get; set; }

        private string BRateOri { get; set; }

        private string BCommentOri { get; set; }

        public string BSituation { get => bSituation; set { bSituation = value; OnPropertyChanged(); } }

        public string BSubtitleAndVol { get => bSubtitleAndVol; set { bSubtitleAndVol = value; OnPropertyChanged(); } }

        public string BLblSituationtext { get => bLblSituationtext; set { bLblSituationtext = value; OnPropertyChanged(); } }

        public string BRate { get => bRate; set { bRate = value; OnPropertyChanged(); } }

        public string BComment { get => bComment; set { bComment = value; OnPropertyChanged(); } }

        private bool DisableUpdates { get; set; }


        public DetailBook(string bookKey)
        {
            BRate = BSituation = "0";
            DisableUpdates = false;
            BindingContext = this;
            BookKey = bookKey;
            InitializeComponent();

            PkrSituation.ItemsSource = BBTSituation;

            Task.Run(() => { CarregaBook(bookKey); return Task.CompletedTask; }).Wait();
        }

        private async void CarregaBook(string bookKey)
        {
            ModelLayer.Books.Book book = new ModelLayer.Books.Book();

            Task.Run(() => book = BusinessLayer.BBooks.GetBook(bookKey)).Wait();

            string SubtitleAndVol = "";

            if (!string.IsNullOrEmpty(book.SubTitle))
            {
                SubtitleAndVol = book.SubTitle;
            }
            if (!string.IsNullOrEmpty(book.SubTitle) && !string.IsNullOrEmpty(book.Volume))
            {
                SubtitleAndVol += "; ";
            }
            if (!string.IsNullOrEmpty(book.Volume))
            {
                SubtitleAndVol += "Vol.: " + book.Volume;
            }

            BTitle = book.Title;
            BAuthors = book.Authors;
            BGenre = book.Genre;
            BPages = book.Pages.ToString();
            BComment = book.BooksSituations.Comment;
            BSubtitleAndVol = SubtitleAndVol;
            BLblSituationtext = BBTSituation[(int)book.BooksSituations.Situation];

            if (book.BooksSituations.Situation != ModelLayer.Books.Situation.None)
            {
                BSituation = BSituationOri = book.BooksSituations.Situation.ToString();
                BRate = BRateOri = book.BooksSituations.Rate.ToString();
                BComment = BCommentOri = book.BooksSituations.Comment;
                PkrSituation.SelectedIndex = (int)book.BooksSituations.Situation;
                //PkrSituation.IsVisible = false;

                lblHComment.IsVisible = lblComment.IsVisible = !string.IsNullOrEmpty(BComment);

                DisableUpdates = true;
                lblSituation.IsVisible = lblHSituation.IsVisible = PkrSituation.IsEnabled = EdtComment.IsEnabled =
                    EdtComment.IsVisible = SldrRate.IsVisible = LblSdlrRate.IsVisible = false;

                if (book.BooksSituations.Situation == ModelLayer.Books.Situation.Read)
                {

                    if (!string.IsNullOrEmpty(EdtComment.Text))
                    {
                        EdtComment.IsVisible = true;
                    }
                    SldrRate.IsVisible = false;
                    LblSdlrRate.IsVisible = true;
                }

                BtnConf.StyleClass = new List<string> { "button_secundary" };
                BtnConf.Text = "Alterar";
            }
            else
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = BtnConf.IsVisible = false;
                lblSituation.IsVisible = lblHSituation.IsVisible = true;
                BSituation = "0";
                BRate = BComment = "";
            }
        }

        private async void BtnConf_Clicked(object sender, EventArgs e)
        {
            if (DisableUpdates)
            {
                PkrSituation.IsVisible = PkrSituation.IsEnabled = EdtComment.IsEnabled = true;
                lblHComment.IsVisible = lblHSituation.IsVisible = lblComment.IsVisible = lblSituation.IsVisible = false;

                DisableUpdates = false;
                BtnConf.StyleClass = new List<string> { "button_primary" };
                BtnConf.Text = "Confirmar";
                return;
            }

            BtnConf.IsEnabled = false;
            bool alterou = false;
            string commment = "";
            int rate = Convert.ToInt32(SldrRate.Value);

            if (BRateOri != rate.ToString())
            {
                alterou = true;
            }
            else if (BSituationOri != (PkrSituation.SelectedIndex).ToString())
            {
                alterou = true;

                if (PkrSituation.SelectedIndex == 3)
                {
                    commment = EdtComment.Text;
                    if (BComment != BCommentOri)
                    {
                        alterou = true;
                    }
                }
            }
            if (alterou)
            {
                _ = Task.Run(() => { BusinessLayer.BBooks.UpdateSituationBook(BookKey, (ModelLayer.Books.Situation)PkrSituation.SelectedIndex, rate, commment); return Task.CompletedTask; });

                if (!await DisplayAlert("Aviso", "Situação alterada", null, "Ok"))
                {
                    _ = await Navigation.PopAsync();
                }
            }
            else
            {
                _ = DisplayAlert("Aviso", "Sem alterações", null, "Ok");
                BtnConf.IsEnabled = true;
            }

        }

        private void PkrSituation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PkrSituation.SelectedIndex == 0)
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = false;
                BtnConf.IsVisible = false;
            }
            else
            {
                BtnConf.IsVisible = true;

                if (PkrSituation.SelectedIndex == 3)
                {
                    SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = lblHComment.IsVisible = true;
                }
                else
                {
                    SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = lblHComment.IsVisible = false;
                }
            }
        }

        private void TbiEditarLivro_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CadastrarLivro(BookKey));
        }

        private async void TbiExcLivro_Clicked(object sender, EventArgs e)
        {
            if ((await DisplayAlert("Confirmação", "Deseja excluir esse livro?", "Sim", "Cancelar")))
            {
                _ = Task.Run(() => { BusinessLayer.BBooks.InactivateBook(BookKey); });

                if (!(await DisplayAlert("Aviso", "Livro excluído!", null, "Ok")))
                {
                    await Navigation.PopAsync();
                }
            }
        }

    }
}