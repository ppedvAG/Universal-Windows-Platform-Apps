using ContactsUWP.Common.Helper;
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ContactsUWP.Common.Controls
{
    public sealed partial class ImageCaptureControl : UserControl
    {
        private CameraCaptureUI captureUI;
        public StorageFile ImageFile { get; set; }

        public event Action<ImageCaptureControl> ImageSelected;

        public ImageCaptureControl()
        {
            InitializeComponent();
            captureUI = new CameraCaptureUI();
        }

        private void rootGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        public async Task LoadImageFromFileAsync(StorageFile file)
        {
            var ImageFile = file;
            imgControl.Source = await FileHelper.LoadImageFromFileAsync(ImageFile);
            SymbolCamera.Visibility = Visibility.Collapsed;
        }

        public async Task LoadImageFromBytesAsync(byte[] bytes)
        {
            BitmapImage image = await FileHelper.LoadImageFromBytesAsync(bytes);
            imgControl.Source = image;
            SymbolCamera.Visibility = Visibility.Collapsed;
        }

        private async void BtnCamera_Click(object sender, RoutedEventArgs e)
        {
            captureUI.PhotoSettings.AllowCropping = true;
            captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.
            HighestAvailable;
            var capturedImage = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (capturedImage != null)
            {
                ImageFile = capturedImage;
                imgControl.Source = await FileHelper.LoadImageFromFileAsync(ImageFile);
                SymbolCamera.Visibility = Visibility.Collapsed;
                ImageSelected?.Invoke(this);
            }
        }

        private async void BtnPicker_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png" }
            };
            ImageFile = await picker.PickSingleFileAsync();
            imgControl.Source = await FileHelper.LoadImageFromFileAsync(ImageFile);
            SymbolCamera.Visibility = Visibility.Collapsed;
            ImageSelected?.Invoke(this);
        }
    }
}
