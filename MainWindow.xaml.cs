
using System.Windows;

using System.Windows.Threading;


namespace OpenCvSharpProjects
{ 
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int count = 0;

        public MainWindow()
        {
            InitializeComponent();

            // DispatcherTimer 객체 생성 및 설정
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // 1초 간격
            timer.Tick += Timer_Tick; // Tick 이벤트 핸들러 등록
            timer.Start(); // 타이머 시작
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // 텍스트 블록의 내용을 업데이트하는 UI 작업
            txtCounter.Text = $"현재 카운트: {count++}";
        }
    }
}