﻿using AcessLayer.Firebase;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Connectivity;
using Splat;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bookshelf
{
    public partial class App : Application
    {

        /// <summary>
        /// https://material.io/resources/color/#!/?view.left=0&view.right=0&primary.color=5b3f36&secondary.color=00838F
        /// </summary>
        public App()
        {
            BusinessLayer.BUser bUser = new BusinessLayer.BUser();
            bUser.CreateDbLocal();

            InitializeComponent();


            if (bUser.GetUserLocal() != null)
            {
                Thread thread = new Thread(BusinessLayer.BBooksSync.AtualizaBancoLocal) { IsBackground = true };
                thread.Start();

                Application.Current.MainPage = new MainPage();
                Application.Current.MainPage = new NavigationPage(new MainPage())
                {
                    BarBackgroundColor = Color.FromHex("#301810"),
                    BarTextColor = Color.White
                };
            }
            else
            {
                MainPage = new Acessa();
                MainPage = new NavigationPage(new Acessa())
                {
                    BarBackgroundColor = Color.FromHex("#301810"),
                    BarTextColor = Color.White
                };
            }
        }


        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
