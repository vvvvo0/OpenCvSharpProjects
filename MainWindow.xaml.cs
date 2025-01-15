using System.Windows.Threading;

using OpenCvSharp;
// OpenCV4의 데이터 형식이나 함수 및 메서드를 사용하기 위해 네임스페이스에 using OpenCvSharp;을 추가합니다.
// Mat 클래스 또한 using OpenCvSharp;에 포함되어 있습니다.

namespace OpenCvSharpProjects
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window // MainWindow 클래스는 Window 클래스를 상속받는다
    {
        private VideoCapture capture; // VideoCapture 객체를 저장할 필드, 웹캠에서 영상을 가져오는 데 사용된다
        private Mat frame; // Mat 객체를 저장할 필드, OpenCV에서 이미지를 표현하는 데 사용된다
        private DispatcherTimer timer; // DispatcherTimer 객체를 저장할 필드, UI 스레드에서 작업을 예약하는 데 사용된다

        public MainWindow() // 생성자
        {
            InitializeComponent(); // 윈도우를 초기화한다. XAML에서 정의된 컨트롤들을 초기화하고 이벤트 핸들러를 연결하는 등의 작업을 수행한다

            capture = new VideoCapture(0); // VideoCapture 객체를 생성하고 기본 웹캠 장치(0)를 사용하도록 설정한다
            frame = new Mat(); // Mat 객체를 생성한다
            timer = new DispatcherTimer(); // DispatcherTimer 객체를 생성한다
            timer.Interval = TimeSpan.FromMilliseconds(33); // 타이머 간격을 33밀리초로 설정한다. 약 30fps(초당 프레임 수)로 프레임을 업데이트한다
            timer.Tick += UpdateFrame; // timer의 Tick 이벤트에 UpdateFrame 메서드를 연결한다. 즉, 타이머가 발생할 때마다 UpdateFrame 메서드가 호출된다
            timer.Start(); // 타이머를 시작한다
        }

        private void UpdateFrame(object sender, EventArgs e) // 타이머 이벤트가 발생할 때마다 호출되는 메서드이다
        {
            capture.Read(frame); // 웹캠에서 현재 프레임을 읽어와 frame 객체에 저장한다
            if (!frame.Empty()) // frame 객체가 비어 있지 않으면, 즉 프레임을 읽어오는 데 성공하면 다음 코드를 실행한다
            {
                Cv2.ImShow("Camera Output", frame); // Cv2.ImShow() 메서드를 사용하여 frame 객체(이미지)를 "Camera Output" 창(OpenCV 창)에 표시한다
            }
        }


        // 윈도우가 닫힐 때 리소스를 해제하는 방법
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) // 윈도우가 닫힐 때 발생하는 이벤트를 처리하는 메서드이다
        {
            // 윈도우가 닫힐 때 리소스를 해제합니다.
            capture.Release(); // VideoCapture 객체를 해제하여 웹캠 장치를 닫는다
            frame.Dispose(); // Mat 객체를 해제하여 이미지 데이터가 차지하는 메모리를 해제한다
            Cv2.DestroyAllWindows(); // 모든 OpenCV 창을 닫는다
            timer.Stop(); // 타이머를 중지한다
        }
        
    }
}