/*
CameraViewModel 클래스:
웹캠 캡처, 이미지 처리, 객체 인식, 게임 제어 등의 로직을 담당하는 ViewModel이다.
MVVM 패턴의 ViewModel에 해당하는 부분이다.

ViewModel:
Model과 View 사이의 중개자 역할을 하는 ViewModel 클래스를 정의한다.
Model에서 데이터를 가져와 View에 표시하고, View에서 발생하는 이벤트를 처리하는 클래스를 정의한다. 
INotifyPropertyChanged 인터페이스를 구현하여 데이터 변경 알림을 지원한다.
*/

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using OpenCvSharpProjects.Models;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OpenCvSharpProjects.ViewModels
{
    public class CameraViewModel : ViewModelBase // CameraViewModel 클래스는 ViewModelBase 클래스를 상속받는다.
    {
        private VideoCapture capture; // VideoCapture 객체를 저장할 private 필드이다. 웹캠에서 영상을 가져오는 데 사용된다.
        private Mat frame; // Mat 객체를 저장할 private 필드이다. OpenCV에서 이미지를 표현하는 데 사용된다.
        private GameInfo gameInfo; // GameInfo 객체를 저장할 private 필드이다. 게임 정보를 저장하는 데 사용된다.


        public CameraViewModel() // CameraViewModel 클래스의 생성자이다.
        {
            capture = new VideoCapture(0); // VideoCapture 객체를 생성하고 기본 웹캠 장치(0)를 사용하도록 설정한다.
            frame = new Mat(); // Mat 객체를 생성한다.
            gameInfo = new GameInfo(); // GameInfo 객체를 생성한다.

            StartCaptureCommand = new RelayCommand(StartCapture); // RelayCommand는 ICommand 인터페이스를 구현한 클래스이다. StartCapture 메서드를 실행하는 Command를 생성한다.
            StopCaptureCommand = new RelayCommand(StopCapture); // RelayCommand는 ICommand 인터페이스를 구현한 클래스이다. StopCapture 메서드를 실행하는 Command를 생성한다.
        }


        // View에 바인딩할 속성
        public WriteableBitmap CameraImage // 카메라 이미지를 나타내는 WriteableBitmap 객체이다. WPF에서 이미지를 표시하기 위해 사용된다.
        {
            get { return frame.ToWriteableBitmap(); } // frame을 WriteableBitmap으로 변환하여 반환한다.
            set
            {
                frame = value.ToMat(); // WriteableBitmap을 Mat으로 변환하여 frame에 저장한다.
                OnPropertyChanged(); // CameraImage 속성이 변경되었음을 알린다. ViewModelBase 클래스에서 제공하는 메서드이다.
            }
        }

        public Rect GameWindowRect // 게임 화면 영역을 나타내는 Rect 객체이다.
        {
            get { return gameInfo.GameWindowRect; } // gameInfo에서 GameWindowRect 값을 가져와 반환한다.
            set
            {
                gameInfo.GameWindowRect = value; // gameInfo에 GameWindowRect 값을 설정한다.
                OnPropertyChanged(); // GameWindowRect 속성이 변경되었음을 알린다.
            }
        }

        // View에서 실행할 Command
        public ICommand StartCaptureCommand { get; private set; } // 웹캠 캡처를 시작하는 Command이다.
        public ICommand StopCaptureCommand { get; private set; } // 웹캠 캡처를 중지하는 Command이다.


        // 웹캠 캡처 시작
        private async void StartCapture() // 웹캠 캡처를 시작하는 메서드이다.
        {
            await Task.Run(() => // 백그라운드 스레드에서 실행
            {
                // 무거운 작업 (예: 초기화 작업)
            });

            // UI 스레드에서 실행해야 하는 작업 (예: 이벤트 등록)
            Dispatcher.Invoke(() =>
            {
                CompositionTarget.Rendering += UpdateFrame; 
                // CompositionTarget.Rendering 이벤트에 UpdateFrame 메서드를 등록한다.이벤트가 발생할 때마다 UpdateFrame 메서드가 호출된다.

            });
        }

        // 웹캠 캡처 중지
        private void StopCapture() // 웹캠 캡처를 중지하는 메서드이다.
        {
            CompositionTarget.Rendering -= UpdateFrame; // CompositionTarget.Rendering 이벤트에서 UpdateFrame 메서드를 제거한다.
        }

        // 프레임 업데이트
        private async void UpdateFrame(object? sender, EventArgs e) // 프레임 업데이트 메서드이다. CompositionTarget.Rendering 이벤트가 발생할 때마다 호출된다.
        {
            capture.Read(frame); // 웹캠에서 현재 프레임을 읽어와 frame에 저장한다.

            if (!frame.Empty()) // frame이 비어있지 않으면, 즉 프레임을 읽어오는 데 성공하면
            {
                await Task.Run(() => ProcessImage(frame.Clone())); // ProcessImage 메서드를 비동기적으로 실행한다. frame.Clone()을 사용하여 frame의 복사본을 전달한다.

                OnPropertyChanged("CameraImage"); // CameraImage 속성 변경을 알린다.
            }
        }

        // 이미지 처리
        private void ProcessImage(Mat image) // 이미지 처리 메서드이다.
        {
            // TODO: 게임 화면 영역 검출, 특정 객체 인식, 행동 결정 등의 로직 구현
            // 이 부분은 다음 단계에서 구현합니다.
        }
    }
}