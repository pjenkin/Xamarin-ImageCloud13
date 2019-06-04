using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageCloud13
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void SelectImageButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            // Cross-platform initialise to use native technology (Initialize is async)

            // Is device able to take photos?
            if (!CrossMedia.Current.IsPickPhotoSupported)                           // options also pick|take photo|video
            {
                await DisplayAlert("Error","This is not supported on your device","Ok");
                return;         // give up here
            }

            var mediaOptions = new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium       // medium is 50%, small is 25%; width/height, full & custom available
            };      // initialize variable property(ies)

            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);      // options also pick|take photo|video
            // get the file (in this case, pick image)

            if (selectedImageFile == null)          // standard check if something is really existent (maybe some glitch or deletion)
            {
                await DisplayAlert("Error", "There was an error when trying to get the image", "Ok");
                return;                             // and give up
            }

            selectedImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
            // TODO: why lambda syntax here? - perhaps as could be more complex than just the 1 statement here (in curly braces)
        }
    }
}
