
using System.Windows.Threading;

using OpenCvSharp;
// OpenCV4의 데이터 형식이나 함수 및 메서드를 사용하기 위해 네임스페이스에 using OpenCvSharp;을 추가합니다.
// Mat 클래스 또한 using OpenCvSharp;에 포함되어 있습니다.

using OpenCvSharp.WpfExtensions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;


namespace OpenCvSharpProjects
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window // MainWindow 클래스는 Window 클래스를 상속받는다
    {
        private VideoCapture capture; // VideoCapture 객체를 저장할 필드, 웹캠에서 영상을 가져오는 데 사용된다
        private Mat frame; // Mat 객체를 저장할 필드, OpenCV에서 이미지를 표현하는 데 사용된다
        
        
        public MainWindow() // 생성자
        {
            InitializeComponent(); // 윈도우를 초기화한다. XAML에서 정의된 컨트롤들을 초기화하고 이벤트 핸들러를 연결하는 등의 작업을 수행한다

            capture = new VideoCapture(0); // VideoCapture 객체를 생성하고 기본 웹캠 장치(0)를 사용하도록 설정한다
            frame = new Mat(); // Mat 객체를 생성한다



            // CompositionTarget.Rendering 이벤트를 사용하여 프레임 업데이트
            CompositionTarget.Rendering += UpdateFrame; 
        }

        private async void UpdateFrame(object? sender, EventArgs e) // 타이머 이벤트가 발생할 때마다 호출되는 메서드이다
        {
            capture.Read(frame); // 웹캠에서 현재 프레임을 읽어와, frame 객체에 저장한다


            if (!frame.Empty()) // frame 객체가 비어 있지 않으면(프레임을 읽어오는 데 성공하면), 다음 코드를 실행한다
            {

                // 이미지 처리 및 객체 인식을 비동기적으로 수행
                await Task.Run(() => ProcessImage(frame.Clone()));


                // 이미지 처리 및 객체 인식 수행
                ProcessImage(frame); 


                // 화면에 이미지 표시
                Dispatcher.Invoke(() =>
                {
                    // WriteableBitmap을 사용하여 이미지 표시
                    CameraImage.Source = WriteableBitmapConverter.ToWriteableBitmap(frame);
                });

            }
        }


        private void ProcessImage(Mat image)
        {
            // TODO: 게임 화면 영역 검출, 특정 객체 인식, 행동 결정 등의 로직 구현


           
                // 템플릿 이미지 파일 경로
                // string[] templatePaths = { "top_left.png", "top_right.png", "bottom_left.png", "bottom_right.png" };
                string[] templatePaths = { "top_left.png", "bottom_right.png" };

                // 각 템플릿 이미지에 대한 매칭 결과를 저장할 리스트
                List<Point> matchPoints = new List<Point>();


            try
            {
                // 각 템플릿 이미지에 대해 템플릿 매칭 수행
                foreach (string templatePath in templatePaths)
                {
                    // 템플릿 이미지 로드
                    Mat template = Cv2.ImRead(templatePath, ImreadModes.Grayscale);



                    // 템플릿 이미지 로드 확인
                    if (template.Empty())
                    {
                        Console.WriteLine($"템플릿 이미지 로드 실패: {templatePath}");
                        continue; // 다음 템플릿 이미지로 넘어갑니다.
                    }


                    // 템플릿 이미지 크기 확인
                    if (template.Width == 0 || template.Height == 0)
                    {
                        Console.WriteLine($"템플릿 이미지 크기 오류: {templatePath}");
                        continue; // 다음 템플릿 이미지로 넘어갑니다.
                    }

                    // 이미지 타입 확인
                    if (image.Type() != template.Type())
                    {
                        Console.WriteLine($"이미지 타입 불일치: 입력 이미지 - {image.Type()}, 템플릿 이미지 - {template.Type()}");
                        Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY); // 입력 이미지를 그레이스케일로 변환
                    }



                    // 템플릿 이미지가 입력 이미지보다 큰 경우 크기 조정
                    if (template.Width > image.Width || template.Height > image.Height)
                    {
                        Cv2.Resize(template, template, new OpenCvSharp.Size(image.Width / 2, image.Height / 2));
                    }


                  

                    // 템플릿 매칭 결과를 저장할 Mat 객체 생성
                    Mat result = new Mat();


                    // MatchTemplate () 함수를 사용하여 템플릿 매칭 수행
                    Cv2.MatchTemplate(image, template, result, TemplateMatchModes.CCoeffNormed);

                    // 매칭 결과에서 최댓값과 그 위치를 찾음
                    double minVal, maxVal;
                    Point minLoc, maxLoc;
                    Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);

                    Console.WriteLine($"템플릿 매칭 결과 - 최대값: {maxVal}, 위치: ({maxLoc.X}, {maxLoc.Y})"); // 템플릿 매칭 결과 출력

                    // 최댓값이 임계값보다 크면 객체를 찾은 것으로 판단
                    if (maxVal > 0.8) // 임계값은 적절히 조정합니다.
                    {
                        // 템플릿의 크기를 고려하여 객체의 중심 좌표 계산
                        Point center = new Point(maxLoc.X + template.Width / 2, maxLoc.Y + template.Height / 2);
                        matchPoints.Add(center);
                    }
                }

           
                // 매칭 결과를 이용하여 게임 화면 영역 검출
                if (matchPoints.Count == 2) // 두 개의 꼭짓점을 모두 찾은 경우
                {

                    // 꼭짓점 좌표를 찾습니다.
                    Point topLeft = matchPoints[0]; // 왼쪽 상단
                    Point bottomRight = matchPoints[1]; // 오른쪽 하단

                    // 게임 화면 영역 계산
                    int x = (int)topLeft.X;
                    int y = (int)topLeft.Y;
                    int width = (int)(bottomRight.X - topLeft.X);
                    int height = (int)(bottomRight.Y - topLeft.Y);
                    Rect gameWindowRect = new Rect(x, y, width, height);

                    // 게임 화면 영역에 사각형 그리기
                    Cv2.Rectangle(image, gameWindowRect, Scalar.Red, 2);
                }

            }



            catch (OpenCvSharp.OpenCVException ex)
            {
                Console.WriteLine($"OpenCV 예외 발생: {ex.Message}");
                // ... (OpenCV 관련 예외 처리)
            }

            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"파일을 찾을 수 없습니다: {ex.FileName}");
                // ... (파일 관련 예외 처리)
            }

            catch (Exception ex)
            {
                Console.WriteLine($"예외 발생: {ex.Message}");
                // 예외 처리: 로그 기록, 오류 메시지 표시, 프로그램 종료 등
            }
        }



        // 윈도우가 닫힐 때 리소스를 해제하는 방법
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) // 윈도우가 닫힐 때 발생하는 이벤트를 처리하는 메서드이다
        {
            // 윈도우가 닫힐 때 리소스를 해제한다
            capture.Release(); // VideoCapture 객체를 해제하여 웹캠 장치를 닫는다
            frame.Dispose(); // Mat 객체를 해제하여 이미지 데이터가 차지하는 메모리를 해제한다
            Cv2.DestroyAllWindows(); // 모든 OpenCV 창을 닫는다
        }
        
    }
}