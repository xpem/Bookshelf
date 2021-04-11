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
    public partial class DetailBook : ContentPage
    {

        #region variaveis do bind.

        public string bTitle, bAuthors, bPages,
            bGenre;

        public string BTitle
        {
            get => bTitle;
            set
            {
                bTitle = value; OnPropertyChanged();
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

        #endregion

        private string bSituation, bLblSituationtext, bRate, BookKey, bComment, bSubtitleAndVol, bVolume;

        public ObservableCollection<string> BBTSituation;
        private bool bVisibleTexts;
        private string BSituationOri { get; set; }
        private string BRateOri { get; set; }

        private string BCommentOri { get; set; }
        public string BSituation
        {
            get => bSituation;
            set
            {
                bSituation = value; OnPropertyChanged();
            }
        }
        public string BSubtitleAndVol
        {
            get => bSubtitleAndVol;
            set
            {
                bSubtitleAndVol = value; OnPropertyChanged();
            }
        }
        public string BVolume
        {
            get => bVolume;
            set
            {
                bVolume = value; OnPropertyChanged();
            }
        }

        public string BLblSituationtext
        {
            get => bLblSituationtext;
            set
            {
                bLblSituationtext = value; OnPropertyChanged();
            }
        }


        public bool BVisibleTexts
        {
            get => bVisibleTexts;
            set
            {
                bVisibleTexts = value; OnPropertyChanged();
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

        public string BComment
        {
            get => bComment;
            set
            {
                bComment = value; OnPropertyChanged();
            }
        }

        private bool DisableUpdates { get; set; }


        public DetailBook(string bookKey)
        {
            BRate = BSituation = "0";
            DisableUpdates = false;
            BSituation = BRate = "0";
            BindingContext = this;
            BookKey = bookKey;
            InitializeComponent();

            BBTSituation = new ObservableCollection<string> { "Nenhuma", "Vou ler", "Lendo", "Lido", "Interrompido" };

            PkrSituation.ItemsSource = BBTSituation;

            this.Title = "Loading";
            CarregaBook(bookKey);


            this.Title = "Minha Estante";
        }
        private async void CarregaBook(string bookKey)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            ModelLayer.Books.Book book = await BusinessLayer.BBooks.getBook(bookKey);

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
            BLblSituationtext = BBTSituation[book.BooksSituations.Situation];
            lblSituation.IsVisible = lblHSituation.IsVisible = true;

            if (book.BooksSituations.Situation > 0)
            {
                BSituation = BSituationOri = book.BooksSituations.Situation.ToString();
                BRate = BRateOri = book.BooksSituations.Rate.ToString();
                BComment = BCommentOri = book.BooksSituations.Comment;

                PkrSituation.IsVisible = false;

                if (string.IsNullOrEmpty(BComment))
                    lblHComment.IsVisible = lblComment.IsVisible = false;
                else
                    lblHComment.IsVisible = lblComment.IsVisible = true;
            }
            else
            {
                SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = BtnConf.IsVisible = false;

                BSituation = "0";
                BRate = "";
                BComment = "";
            }

            if (PkrSituation.SelectedIndex != 0)
            {
                DisableUpdates = true;
                PkrSituation.IsEnabled = EdtComment.IsEnabled = SldrRate.IsVisible = false;

                if (PkrSituation.SelectedIndex == 3)
                {
                    if (string.IsNullOrEmpty(EdtComment.Text))
                    {
                        EdtComment.IsVisible = false;
                    }
                    SldrRate.IsVisible = false;
                }

                //StyleClass = "button_primary"
                IList<string> vs = new List<string>();
                vs.Add("button_secundary");
                BtnConf.StyleClass = vs;
                BtnConf.Text = "Alterar";
            }
        }

        private async void BtnConf_Clicked(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }
            if (DisableUpdates)
            {
                PkrSituation.IsVisible = PkrSituation.IsEnabled = EdtComment.IsEnabled = true;
                //EdtComment.IsVisible = PkrSituation.IsEnabled = EdtComment.IsEnabled = SldrRate.IsVisible = true;
                lblHComment.IsVisible = lblHSituation.IsVisible = lblComment.IsVisible = lblSituation.IsVisible = false;

                DisableUpdates = false;
                IList<string> vs = new List<string>();
                vs.Add("button_primary");
                BtnConf.StyleClass = vs;
                BtnConf.Text = "Confirmar";
                return;
            }

            this.Title = "Carregando...";
            BtnConf.IsEnabled = false;
            bool alterou = false;
            string commment = "";
            int rate = 0;
            rate = Convert.ToInt32(SldrRate.Value);

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
                await BusinessLayer.BBooks.UpdateSituationBook(BookKey, PkrSituation.SelectedIndex, rate, commment);
                DisplayAlert("Aviso", "Situação alterada", null, "Ok");
            }
            else
            {
                DisplayAlert("Aviso", "Sem alterações", null, "Ok");
                BtnConf.IsEnabled = true;
            }

            this.Title = "Minha Estante";
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
                    SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = true;
                }
                else
                {
                    SldrRate.IsVisible = LblSdlrRate.IsVisible = EdtComment.IsVisible = false;
                }
            }
        }

        private void TbiEditarLivro_Clicked(object sender, EventArgs e)
        {
            CadastrarLivro page = new CadastrarLivro(BookKey);
            Navigation.PushAsync(page);
        }

    }
}