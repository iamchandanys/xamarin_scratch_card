using Scratch_Card.Views;
using System;
using Xamarin.Forms;

namespace Scratch_Card
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var control = this;
            control.Opacity = 1;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var control = this;
            control.Opacity = 0.6;

            _ = Navigation.PushModalAsync(new ScratchView(), false);
        }
    }
}
