using OpenCvSharp;
using System.Threading.Tasks;

namespace OpenCvSharpProjects.Services
{
    class WebcamService
    {
        private VideoCapture capture;
        private Mat frame;

        public WebcamService()
        {
            capture = new VideoCapture(0); // 기본 웹캠 장치를 엽니다.
            frame = new Mat();
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
            await Task.Run(() => capture.Read(frame));
            return frame;
        }
    }
}
