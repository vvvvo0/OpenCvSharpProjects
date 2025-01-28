// Services/ImageProcessingService.cs
using OpenCvSharp;
using OpenCvSharpProjects.Models;
using System;
using System.Collections.Generic;


namespace OpenCvSharpProjects.Services
{
    public class ImageProcessingService
    {
        private readonly ORB orb;

        public ImageProcessingService()
        {
            orb = ORB.Create();
        }

        public GameInfo ProcessImage(Mat image)
        {
            var gameInfo = new GameInfo();

            try
            {
                // 웹캠 이미지 크기 조정
                Cv2.Resize(image, image, new OpenCvSharp.Size(image.Width / 2, image.Height / 2)); // 이미지 크기를 절반으로 줄입니다.
                                                                                                   // ProcessImage() 메서드 안에서 웹캠 이미지 크기를 조정하는 코드를 추가했다.

                // 1. 특징점 매칭을 이용한 게임 화면 영역 검출
                gameInfo.GameWindowRect = DetectGameWindow(image);

                // 2. 객체 인식 (플레이어 및 몬스터)
                // TODO: 객체 인식 모델을 사용하여 플레이어와 몬스터를 인식하고 위치 정보를 gameInfo에 저장합니다.

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

        private Rect DetectGameWindow(Mat image)
        {
            // 템플릿 이미지 파일 경로
            string[] templatePaths = { "Resources/top_left.png", "Resources/bottom_right.png" };

            // 각 템플릿 이미지에 대한 매칭 결과를 저장할 리스트
            var matchPoints = new List<Point2f>();

            // 각 템플릿 이미지에 대해 특징점 매칭 수행
            foreach (var templatePath in templatePaths)
            {
                var template = Cv2.ImRead(templatePath, ImreadModes.Grayscale);

                // 특징점 검출 및 기술
                KeyPoint[] keypoints1, keypoints2;
                Mat descriptors1 = new Mat(), descriptors2 = new Mat();
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

                    matchPoints.AddRange(imageCorners);
                }
            }

            // 꼭짓점 좌표를 이용하여 게임 화면 영역 계산
            if (matchPoints.Count == 4)
            {
                var topLeft = new OpenCvSharp.Point((int)matchPoints[0].X, (int)matchPoints[0].Y);
                var bottomRight = new OpenCvSharp.Point((int)matchPoints[2].X, (int)matchPoints[2].Y);

                // Rect 객체 생성
                int width = bottomRight.X - topLeft.X; // 너비 계산
                int height = bottomRight.Y - topLeft.Y; // 높이 계산

                return new OpenCvSharp.Rect(topLeft.X, topLeft.Y, width, height); // 왼쪽 상단 좌표, 너비, 높이를 사용하여 Rect 객체 생성
            }
            else
            {
                return new OpenCvSharp.Rect(); // 빈 Rect 객체 반환
            }
        }
    }
}