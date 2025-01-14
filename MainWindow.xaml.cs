using System.Windows;
using System.Windows.Threading;

using OpenCvSharp;
// OpenCV4의 데이터 형식이나 함수 및 메서드를 사용하기 위해 네임스페이스에 using OpenCvSharp;을 추가합니다.
// Mat 클래스 또한 using OpenCvSharp;에 포함되어 있습니다.

namespace OpenCvSharpProjects
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        
        private VideoCapture video;
        private Mat frame;

        public MainWindow()
        {
            InitializeComponent();

            VideoCapture video = new VideoCapture(0);
            Mat frame = new Mat();

            Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //  프레임을 읽어오고 표시하는 작업을 반복
            while (Cv2.WaitKey(1) != 'q') // 1밀리초 동안 키 입력 대기
            {
                video.Read(frame);
                if (!frame.Empty())
                {
                    Cv2.ImShow("MainWindow", frame); // OpenCV 창에 이미지 표시
                }
            }


            frame.Dispose();
            video.Release();
            Cv2.DestroyAllWindows();
        }
    }
}