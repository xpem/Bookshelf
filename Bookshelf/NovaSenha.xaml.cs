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
    public partial class NovaSenha : ContentPage
    {

        private string vNick, vUserKey;

        public string VNick
        {
            get => vNick;
            set
            {
                vNick = value; OnPropertyChanged();
            }
        }

        public NovaSenha(string Nick, string UserKey)
        {

            VNick = Nick;
            vUserKey = UserKey;
            BindingContext = this;

            InitializeComponent();
        }
    }
}