// Services/ImageProcessingService.cs
using OpenCvSharp;
using OpenCvSharpProjects.Models;
using System;
using System.Collections.Generic;

namespace OpenCvSharpProjects.Services
{
    public class ImageProcessingService
    {
        private Mat descriptors1; // 클래스 멤버 변수로 선언
        private Mat descriptors2; // 클래스 멤버 변수로 선언

        public ImageProcessingService()
        {
            descriptors1 = new Mat(); // 생성자에서 초기화
            descriptors2 = new Mat(); // 생성자에서 초기화
        }

        public GameInfo ProcessImage(Mat image)
        {
            var gameInfo = new GameInfo();
            try
            {
                // 템플릿 이미지 파일 경로
                string[] templatePaths = { "Resources/top_left.png", "Resources/bottom_right.png" };

                // 각 템플릿 이미지에 대한 매칭 결과를 저장할 리스트
                var matchPoints = new List<Point>();

                // ORB 특징점 검출기 생성
                var orb = ORB.Create();

                // 각 템플릿 이미지에 대해 특징점 매칭 수행
                foreach (var templatePath in templatePaths)
                {
                    var template = Cv2.ImRead(templatePath, ImreadModes.Grayscale);

                    // 템플릿 이미지 로드 확인
                    if (template.Empty())
                    {
                        Console.WriteLine($"템플릿 이미지 로드 실패: {templatePath}");
                        continue;
                    }

                    // 템플릿 이미지 크기 확인
                    if (template.Width == 0 || template.Height == 0)
                    {
                        Console.WriteLine($"템플릿 이미지 크기 오류: {templatePath}");
                        continue;
                    }

                    // 특징점 검출 및 기술
                    KeyPoint[] keypoints1, keypoints2;
                    descriptors1 = new Mat(); // 새로운 Mat 객체로 초기화
                    descriptors2 = new Mat(); // 새로운 Mat 객체로 초기화
                    orb.DetectAndCompute(image, null, out keypoints1, descriptors1);
                    orb.DetectAndCompute(template, null, out keypoints2, descriptors2);


                    // BFMatcher 생성
                    var matcher = new BFMatcher(NormTypes.Hamming, true);

                    // 특징점 매칭
                    var matches = matcher.Match(descriptors1, descriptors2);

                    // 좋은 매칭 결과만 선택
                    var goodMatches = new List<DMatch>();
                    double minDist = double.MaxValue;
                    double maxDist = double.MinValue;
                    for (int i = 0; i < descriptors1.Rows; i++)
                    {
                        double dist = matches[i].Distance;
                        if (dist < minDist) minDist = dist;
                        if (dist > maxDist) maxDist = dist;
                    }
                    double goodMatchDist = 2 * minDist;
                    if (goodMatchDist > maxDist)
                    {
                        goodMatchDist = 0.7 * maxDist;
                    }
                    for (int i = 0; i < descriptors1.Rows; i++)
                    {
                        if (matches[i].Distance < goodMatchDist)
                        {
                            goodMatches.Add(matches[i]);
                        }
                    }

                    // 매칭 결과를 이용하여 템플릿 위치 계산
                    if (goodMatches.Count >= 4) // 최소 4개의 매칭점 필요
                    {
                        var pts1 = goodMatches.Select(m => keypoints1[m.QueryIdx].Pt).ToArray();
                        var pts2 = goodMatches.Select(m => keypoints2[m.TrainIdx].Pt).ToArray();

                        // 호모그래피 행렬 계산
                        // InputArray.Create() 사용
                        var homography = Cv2.FindHomography(InputArray.Create(pts2), InputArray.Create(pts1), HomographyMethods.Ransac);

                        // 템플릿 이미지의 네 꼭짓점 좌표
                        var templateCorners = new Point2f[]
                        {
                            new Point2f(0, 0),
                            new Point2f(template.Width, 0),
                            new Point2f(template.Width, template.Height),
                            new Point2f(0, template.Height)
                        };

                        // 호모그래피 행렬을 이용하여 웹캠 이미지에서 템플릿 이미지의 꼭짓점 좌표 계산
                        var imageCorners = Cv2.PerspectiveTransform(templateCorners, homography);

                        // 꼭짓점 좌표를 이용하여 게임 화면 영역 계산
                        var topLeft = new Point((int)imageCorners[0].X, (int)imageCorners[0].Y);
                        var bottomRight = new Point((int)imageCorners[2].X, (int)imageCorners[2].Y);
                        int x = (int)Math.Max(0, topLeft.X);
                        int y = (int)Math.Max(0, topLeft.Y);
                        int width = (int)Math.Min(image.Width - x, bottomRight.X - topLeft.X);
                        int height = (int)Math.Min(image.Height - y, bottomRight.Y - topLeft.Y);
                        var gameWindowRect = new Rect(x, y, width, height);

                        gameInfo.GameWindowRect = gameWindowRect;
                    }
                }
            }
            catch (OpenCvSharp.OpenCVException ex)
            {
                Console.WriteLine($"OpenCV 예외 발생: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"예외 발생: {ex.Message}");
            }

            return gameInfo;
        }
    }
}