using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharpProjects.Services;
using System.Windows.Media.Imaging;
using System.Windows.Media;


namespace OpenCvSharpProjects.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly ImageProcessingService _imageProcessingService;

        [ObservableProperty]
        private BitmapSource _gameScreenImage;

        [ObservableProperty]
        private BitmapSource _miniMapImage;

        public MainWindowViewModel()
        {
            _imageProcessingService = new ImageProcessingService();

            // 이미지 파일 로드 (예시)
            Mat sourceImage = Cv2.ImRead("your_image_path.jpg");

            // 이미지 처리
            _imageProcessingService.ProcessImage(sourceImage);

            // 이미지 업데이트
            GameScreenImage = sourceImage.ToBitmapSource();
            MiniMapImage = _imageProcessingService.MiniMapImage.ToBitmapSource();
        }
    }
}