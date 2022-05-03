using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace FaceDetectHF.Services
{
    class ImageProcessService
    {
        /// <summary>
        /// A kiválasztott kép Byte tömbbé való alakíttását végző függvény
        /// </summary>
        /// <param name="file">A kiválasztott képfile</param>
        /// <returns>Az átalakított bytetömb</returns>
        private async Task<byte[]> StorageFileToByteArray(StorageFile file)
        {
            byte[] byteArray = null;

            if (file != null)
            {
                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                var reader = new DataReader(fileStream.GetInputStreamAt(0));
                await reader.LoadAsync((uint)fileStream.Size);
                byteArray = new byte[fileStream.Size];
                reader.ReadBytes(byteArray);
            }

            return byteArray;
        }

        /// <summary>
        /// Fénykép kiválasztó felület megjelenítésért és a kiválasztásért felelős függvény
        /// </summary>
        /// <returns>A kiválasztott kép byte tömbje</returns>
        public async Task<byte[]> PicturePicker()
        {
            FileOpenPicker photoPicker = new FileOpenPicker();
            photoPicker.ViewMode = PickerViewMode.Thumbnail;
            photoPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            photoPicker.FileTypeFilter.Add(".jpg");
            photoPicker.FileTypeFilter.Add(".jpeg");
            photoPicker.FileTypeFilter.Add(".png");
            photoPicker.FileTypeFilter.Add(".bmp");

            StorageFile photoFile = await photoPicker.PickSingleFileAsync();
            if (photoFile == null)
            {
                return null;
            }
            return await StorageFileToByteArray(photoFile);
        }

        /// <summary>
        /// A kamera felület megjelenítésért felelős függvény
        /// </summary>
        /// <returns>A kiválasztott kép byte tömbje</returns>
        public async Task<byte[]> PictureTaker()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.AllowCropping = false;
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                return null;
            }
            return await StorageFileToByteArray(photo);
        }
    }
}
