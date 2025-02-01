using OpenCvSharp;
using System.Threading.Tasks;
using System;


// 웹캠에서 이미지 프레임을 가져오는 역할
namespace OpenCvSharpProjects.Services
{
    class WebcamService
    {
        private VideoCapture capture;
        private Mat frame;

        public WebcamService()
        {
            // 생성자에서는 카메라 인덱스를 확인하고 설정합니다.
            int cameraIndex = FindCameraIndex();
            capture = new VideoCapture(cameraIndex, VideoCaptureAPIs.ANY);

            capture.Set(VideoCaptureProperties.FrameWidth, 640); // 너비 설정
            capture.Set(VideoCaptureProperties.FrameHeight, 480); // 높이 설정
            capture.Set(VideoCaptureProperties.Fps, 30); // 프레임 속도 설정


            // capture = new VideoCapture(0); // 기본 웹캠 장치를 엽니다.
            frame = new Mat();
        }

        private int FindCameraIndex()
        {
            // 연결된 카메라 장치 목록을 가져옵니다.
            for (int i = 0; i < 10; i++) // 최대 10개의 카메라 장치 확인
            {
                using (var capture = new VideoCapture(i))
                {
                    if (capture.IsOpened())
                    {
                        double frameHeight = capture.Get(VideoCaptureProperties.FrameHeight);
                        Console.WriteLine($"카메라 {i} - FrameHeight: {frameHeight}");
                    }
                }
            }

            // 갤럭시 S23 카메라 선택 (카메라 인덱스는 환경에 따라 다를 수 있습니다.)
            int cameraIndex = 1; // 갤럭시 S23 카메라가 1번 인덱스에 해당한다고 가정합니다. 
            return cameraIndex;
        }


        public async Task StartCaptureAsync()
        {
            // 웹캠 캡처를 시작합니다.
            await Task.Run(() =>
            {
                if (!capture.IsOpened())
                {
                    capture.Open(0);
                }
            });
        }

        public void StopCapture()
        {
            // 웹캠 캡처를 중지합니다.
            capture.Release(); 
        }

        public async Task<Mat> GetFrameAsync()
        {
            // 웹캠에서 프레임을 가져옵니다.
            await Task.Run(() => capture.Read(frame)); // 웹캠에서 프레임을 읽어옵니다.
            return frame; // 프레임을 반환합니다.
        }
    }
}
