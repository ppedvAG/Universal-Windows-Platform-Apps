using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ContactsUWP.Common.Helper
{
    public static class FileHelper
    {
        public static async Task<BitmapImage> LoadImageFromBytesAsync(byte[] bytes)
        {
            BitmapImage image = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(bytes.AsBuffer());
                stream.Seek(0);
                await image.SetSourceAsync(stream);
            }

            return image;
        }

        public static async Task<byte[]> StorageFileAsByteArrayAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }

        public static async Task<BitmapImage> LoadImageFromFileAsync(StorageFile file)
        {
            BitmapImage image = new BitmapImage();
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                await image.SetSourceAsync(stream);
            }
            return image;
        }

        public static async void CompressImageFile(StorageFile file, uint compressionFactor = 2)
        {
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                using (var encoderStream = new InMemoryRandomAccessStream())
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(encoderStream, decoder);
                    uint newHeight = decoder.PixelHeight / compressionFactor;
                    uint newWidth = decoder.PixelWidth / compressionFactor;
                    encoder.BitmapTransform.ScaledHeight = newHeight;
                    encoder.BitmapTransform.ScaledWidth = newWidth;

                    await encoder.FlushAsync();

                    byte[] pixels = new byte[newWidth * newHeight * 4];

                    await encoderStream.ReadAsync(pixels.AsBuffer(), (uint)pixels.Length, InputStreamOptions.None);
                }
            }
        }
    }
}
