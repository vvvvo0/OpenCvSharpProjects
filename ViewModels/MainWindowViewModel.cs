using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using OpenCvSharpProjects.Models;
using OpenCvSharpProjects.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OpenCvSharpProjects.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly WebcamService webcamService; // 웹캠 서비스를 사용하기 위한 변수이다.
        private readonly ImageProcessingService imageProcessingService; // 이미지 처리 서비스를 사용하기 위한 변수이다.
        private readonly KeyboardControlService keyboardControlService; // 키보드 제어 서비스를 사용하기 위한 변수이다.
        private GameInfo gameInfo; // 게임 정보를 저장하는 변수이다.

        

        public MainWindowViewModel()
        {
            // webcamService = new WebcamService(); // 웹캠 서비스 객체를 생성한다.

            imageProcessingService = new ImageProcessingService(); // 이미지 처리 서비스 객체를 생성한다.
            keyboardControlService = new KeyboardControlService(); // 키보드 제어 서비스 객체를 생성한다.
            gameInfo = new GameInfo(); // 게임 정보 객체를 생성한다.
            cameraImage = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Bgr32, null); // cameraImage를 초기화한다.


            // RelayCommand를 사용하여 Command를 초기화한다.
            StartCaptureCommand = new RelayCommand(StartCapture);
            StopCaptureCommand = new RelayCommand(StopCapture);
            StartGameCommand = new RelayCommand(StartGame);
            StopGameCommand = new RelayCommand(StopGame);
        }

        // View에 바인딩할 속성들이다.
        [ObservableProperty]
        private WriteableBitmap cameraImage; // 웹캠에서 캡처한 이미지를 표시하기 위한 속성이다.

        [ObservableProperty]
        private OpenCvSharp.Rect gameWindowRect; // 게임 화면 영역을 표시하기 위한 속성이다.
                                                 // // OpenCvSharp.Rect 명시

        [ObservableProperty]
        private bool isMinimapDetected; // 미니맵이 검출되었는지 여부를 표시하기 위한 속성이다.
                                        


        // View에서 실행할 Command들이다.
        public ICommand StartCaptureCommand { get; private set; } // 웹캠 캡처를 시작하는 Command이다.
        public ICommand StopCaptureCommand { get; private set; } // 웹캠 캡처를 중지하는 Command이다.
        public ICommand StartGameCommand { get; private set; } // 게임을 시작하는 Command이다.
        public ICommand StopGameCommand { get; private set; } // 게임을 중지하는 Command이다.



        // 웹캠 캡처 시작 메서드이다.
        private async void StartCapture()
        {
           

            // await webcamService.StartCaptureAsync(); // 웹캠 캡처를 시작한다.
            // CompositionTarget.Rendering += UpdateFrame; // CompositionTarget.Rendering 이벤트에 UpdateFrame 메서드를 등록하여 프레임 업데이트를 시작한다.


        }



        // 웹캠 캡처 중지 메서드이다.
        private void StopCapture()
        {
            // webcamService.StopCapture(); // 웹캠 캡처를 중지한다.
            // CompositionTarget.Rendering -= UpdateFrame; // CompositionTarget.Rendering 이벤트에서 UpdateFrame 메서드를 제거하여 프레임 업데이트를 중지한다.
        }



        // 게임 시작 메서드이다.
        private void StartGame()
        {
            // TODO: 게임 시작 로직 구현
        }



        // 게임 중지 메서드이다.
        private void StopGame()
        {
            // TODO: 게임 중지 로직 구현
        }



        // 프레임 업데이트 메서드이다.
        private async void UpdateFrame(object? sender, EventArgs e)
        {
            // Mat frame = await webcamService.GetFrameAsync(); // 웹캠에서 프레임을 가져온다.

            // if (!frame.Empty()) // 프레임이 비어있지 않으면
            // {
                // CameraImage = frame.ToWriteableBitmap(); // 프레임을 WriteableBitmap으로 변환하여 CameraImage 속성에 설정한다.

                // await Task.Run(() =>
                // {
                    // gameInfo = imageProcessingService.ProcessImage(frame.Clone()); // 이미지 처리 서비스를 사용하여 이미지를 처리하고 게임 정보를 업데이트한다.
                    // GameWindowRect = gameInfo.GameWindowRect; // 게임 화면 영역을 업데이트한다.
            
                    
                    // IsMinimapDetected = gameInfo.IsMinimapDetected;// GameInfo 객체의 IsMinimapDetected 값을 바인딩한다.
                   

                
                // });
            // }
        }
    }
}