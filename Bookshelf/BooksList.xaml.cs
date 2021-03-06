﻿using ModelLayer;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bookshelf
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BooksList : ContentPage
    {

        public ObservableCollection<ModelLayer.Books.ItemBookList> ObBooksList;
        public BusinessLayer.BBooks bBooks;

        private bool isLoading;
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(); } }
        private int SituationIndex { get; set; }

        /// <summary>
        /// true se o processo de busca estiver em funcionamento
        /// </summary>
        public bool Efetuandobusca { get; set; }
        public BooksList(int situation)
        {
            bBooks = new BusinessLayer.BBooks();
            BindingContext = this;
            SituationIndex = situation;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ObBooksList = new ObservableCollection<Books.ItemBookList>();
            LoadBooks(0);
            Task.Run(() => PartialLoadBooks());
        }

        //Recupera os livros por status
        private async void LoadBooks(int Index)
        {
            Title = "Carregando lista...";
            IsLoading = true;

           
            string SubtitleAndVol;

            string textoBusca = "";
            if (!string.IsNullOrEmpty(EntSearchTitle.Text))
            {
                textoBusca = EntSearchTitle.Text.ToUpper();
            }

            foreach (Books.Book book in await bBooks.GetBookSituationByStatus(SituationIndex, Index, textoBusca))
            {
                SubtitleAndVol = "";
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

                Books.ItemBookList itemBookList = new Books.ItemBookList
                {
                    Key = book.Key,
                    Title = book.Title,
                    AuthorsAndYear = book.Authors + "; Ano: " + book.Year,
                    Pages = book.Pages.ToString(),
                    SubtitleAndVol = SubtitleAndVol,
                    Rate = book.BooksSituations.Rate > 0 ? string.Format("Avaliação pessoal: {0} de 5", book.BooksSituations.Rate.ToString()) : "",
                };

                ObBooksList.Add(itemBookList);
            }

            LstBooks.ItemsSource = ObBooksList;

            //Definição do título da interface
            Title = "Lista de Livros";
            switch (SituationIndex)
            {
                case 0: this.Title += " (Arquivo)"; break;
                case 1: this.Title += " (Vou Ler)"; break;
                case 2: this.Title += " (Lendo)"; break;
                case 3: this.Title += " (Lido)"; break;
                case 4: this.Title += " (Interrompido)"; break;
            }

            IsLoading = false;
        }

        //função que carrega parcialmente a lista de livros por status
        private void PartialLoadBooks()
        {
            int Index = 0;
            LstBooks.ItemAppearing += (sender, e) =>
                {
                    if (BusinessLayer.BBooks.Total > 10)
                    {
                        if (IsLoading || ObBooksList.Count == 0)
                        {
                            return;
                        }

                        //hit bottom!
                        if (e.Item == ObBooksList[ObBooksList.Count - 1])
                        {
                            Index += 10;
                            if (Index < BusinessLayer.BBooks.Total)
                                LoadBooks(Index);
                        }
                    }
                };

        }

        private void LstBooks_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Books.ItemBookList itemBook = (Books.ItemBookList)e.SelectedItem;

                if (SituationIndex == -1)
                {
                    CadastrarLivro page = new CadastrarLivro(itemBook.Key);
                    Navigation.PushAsync(page);
                    LstBooks.SelectedItem = null;
                }
                else
                {
                    DetailBook detalhaLivros = new DetailBook(itemBook.Key);
                    Navigation.PushAsync(detalhaLivros);
                    LstBooks.SelectedItem = null;
                }
            }
        }

        private void EntSearchTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            EfetuaBusca();
        }

        /// <summary>
        /// Filtra a lista pela busca por texto
        /// </summary>
        private async void EfetuaBusca()
        {
            //rodar apenas se nao estiver no processo de busca
            if (!Efetuandobusca)
            {
                Efetuandobusca = true;

                while (Efetuandobusca)
                {
                    try
                    {
                        //espera dois segundos para efetuar a busca
                        await Task.Delay(2000);

                        ObBooksList = new ObservableCollection<Books.ItemBookList>();
                        LoadBooks(0);

                        Efetuandobusca = false;
                    }
                    catch (Exception ex)
                    { throw ex; }
                }
            }
        }

    }
}