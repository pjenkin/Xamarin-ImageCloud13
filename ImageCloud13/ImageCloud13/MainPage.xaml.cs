using Microsoft.WindowsAzure.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
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

            try
            {
                var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);      // options also pick|take photo|video

                if (selectedImageFile == null)          // standard check if something is really existent (maybe some glitch or deletion)
                {
                    await DisplayAlert("Error", "There was an error when trying to get the image", "Ok");
                    return;                             // and give up
                }

                selectedImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
                // TODO: why lambda syntax here? - perhaps as could be more complex than just the 1 statement here (in curly braces)
                // get the file (in this case, pick image)

                UploadImage(selectedImageFile.GetStream());
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", exc.Message, "OK");
            }

        }

        // bespoke method (by Ctrl+. from code)
        private async void UploadImage(Stream stream)
        {
            // ought to be a Task, returning something?

            // throw new NotImplementedException();
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=pnj13imagestorage;AccountKey=ba8GZs7NHqLK1FEFFTbG+GOLnURNpXQfKv2LxrV9QpFcXkmJtDWVfXlFHa65sExaGDOe7+9b0tVY5y9+m6Ke2w==;EndpointSuffix=core.windows.net");
            // Connection String from Access Keys for Storage Service blade in Azure Portal
            try
            {
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("imagecontainer");     // refer to name of container
                await container.CreateIfNotExistsAsync();

                var someUniqueName = Guid.NewGuid().ToString();
                var blockBlob = container.GetBlockBlobReference($"{someUniqueName}.jpg");     // would normally pass in filename or ID of record - hard-coded for jpg at mo

                // Initialise possibly-newly-made container's blob by uploading a string, file, &c - in this case, the actual file blob to upload
                await blockBlob.UploadFromStreamAsync(stream);

                string url = blockBlob.Uri.OriginalString;
                await DisplayAlert("Image Upload URL", "Image Upload URL is " + url,"Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error uploading", ex.Message, "OK");
            }
        }
    }
}
