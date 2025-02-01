// Services/ImageProcessingService.cs
using OpenCvSharp;
using OpenCvSharpProjects.Models;
using System;
using System.Collections.Generic;
using NLog;

namespace OpenCvSharpProjects.Services
{
    public class ImageProcessingService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger(); // Logger 객체 생성
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

                // 테스트 이미지 로드
                Mat testImage = Cv2.ImRead("Resources/minimap_template.png", ImreadModes.Color); // 리소스 폴더의 미니맵 템플릿 이미지 사용


                // 웹캠 이미지 크기 조정 (예: 절반으로 줄이기)
                Cv2.Resize(testImage, testImage, new OpenCvSharp.Size(testImage.Width / 2, testImage.Height / 2));

                // 밝기/대비 조절 (예: 밝기 1.2배, 대비 1.5배)
                Cv2.AddWeighted(testImage, 1.2, new Mat(testImage.Size(), testImage.Type()), 0, 1.5, testImage); // 빈 이미지의 크기와 타입을 testImage와 동일하게 설정


                // 1. 특징점 매칭을 이용한 게임 화면 영역 검출
                gameInfo.GameWindowRect = DetectGameWindow(testImage); // 테스트 이미지 사용(testImage 전달)

                // 2. 객체 인식 (플레이어 및 몬스터)
                // TODO: 객체 인식 모델을 사용하여 플레이어와 몬스터를 인식하고 위치 정보를 gameInfo에 저장합니다.
            }


            catch (OpenCvSharp.OpenCVException ex)
            {
                Console.WriteLine($"OpenCV 예외 발생: {ex.Message}");
                logger.Error(ex, "OpenCV 예외 발생"); // 예외 정보와 함께 로그 출력
            }


            catch (Exception ex)
            {
                Console.WriteLine($"예외 발생: {ex.Message}");
                logger.Error(ex, "예외 발생"); // 예외 정보와 함께 로그 출력
            }

            return gameInfo; // gameInfo 객체 반환
        }



        private Rect DetectGameWindow(Mat image)
        {
            // 템플릿 이미지 파일 경로
            string templatePath = "Resources/minimap_template.png"; // 템플릿 이미지 파일 경로

            // 각 템플릿 이미지에 대한 매칭 결과를 저장할 리스트
            var matchPoints = new List<Point2f>(); // foreach 루프 밖에서 리스트 생성

            // 템플릿 이미지 로드
            var template = Cv2.ImRead(templatePath, ImreadModes.Grayscale);


            // 미니맵 템플릿 매칭
            Mat result = new Mat();

            // image를 그레이스케일로 변환합니다.
            Mat grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY); // grayImage를 사용합니다.



            Cv2.MatchTemplate(grayImage, template, result, TemplateMatchModes.CCoeffNormed);
            double minVal, maxVal;
            Point minLoc, maxLoc;
            Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);

            if (maxVal > 0.8) // 템플릿 매칭 성공
            {
                // 미니맵 영역 추출
                OpenCvSharp.Rect minimapRect = new OpenCvSharp.Rect(maxLoc.X, maxLoc.Y, template.Width, template.Height);
                Mat minimapImage = new Mat(image, minimapRect);

                // 특징점 검출 및 기술 (minimapImage 사용)
                KeyPoint[] keypoints1, keypoints2;
                Mat descriptors1 = new Mat(), descriptors2 = new Mat();
                orb.DetectAndCompute(minimapImage, null, out keypoints1, descriptors1);
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

                // 꼭짓점 좌표를 이용하여 게임 화면 영역 계산
                if (matchPoints.Count == 4)
                {
                    var topLeft = new OpenCvSharp.Point((int)matchPoints[0].X, (int)matchPoints[0].Y);
                    var bottomRight = new OpenCvSharp.Point((int)matchPoints[2].X, (int)matchPoints[2].Y);

                    // Rect 객체 생성
                    int width = bottomRight.X - topLeft.X; // 너비 계산
                    int height = bottomRight.Y - topLeft.Y; // 높이 계산

                    logger.Debug("미니맵 영역 검출 성공"); // 로그 출력

                    return new OpenCvSharp.Rect(topLeft.X, topLeft.Y, width, height); // 왼쪽 상단 좌표, 너비, 높이를 사용하여 Rect 객체 생성
                }
                else
                {
                    logger.Debug("미니맵 영역 검출 실패"); // 로그 출력

                    return new OpenCvSharp.Rect(); // 빈 Rect 객체 반환
                }
            }


            else
            {
                logger.Debug("미니맵 템플릿 매칭 실패"); // 로그 출력
                return new OpenCvSharp.Rect(); // 빈 Rect 객체 반환
            }


        }
    }
}




/*
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

                matchPoints.AddRange(imageCorners); // imageCorners 배열의 모든 꼭짓점 좌표를 추가
            }




            // 꼭짓점 좌표를 이용하여 게임 화면 영역 계산
            if (matchPoints.Count == 4) // matchPoints 리스트의 크기가 4인지 확인
            {
                var topLeft = new OpenCvSharp.Point((int)matchPoints[0].X, (int)matchPoints[0].Y);
                var bottomRight = new OpenCvSharp.Point((int)matchPoints[2].X, (int)matchPoints[2].Y);

                // Rect 객체 생성
                int width = bottomRight.X - topLeft.X; // 너비 계산
                int height = bottomRight.Y - topLeft.Y; // 높이 계산

                logger.Debug("미니맵 영역 검출 성공"); // 로그 출력

                return new OpenCvSharp.Rect(topLeft.X, topLeft.Y, width, height); // 왼쪽 상단 좌표, 너비, 높이를 사용하여 Rect 객체 생성
            }
            else
            {
                logger.Debug("미니맵 영역 검출 실패"); // 로그 출력

                // matchPoints 리스트에 4개의 꼭짓점 좌표가 없는 경우 처리
                logger.Warn("미니맵 영역 검출 실패 - 꼭짓점 좌표 부족"); // 로그 출력

                return new OpenCvSharp.Rect(); // 빈 Rect 객체 반환
            }





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

        matchPoints.AddRange(imageCorners); // matchPoints 리스트에 특징점 추가
    }
}


// 꼭짓점 좌표를 이용하여 게임 화면 영역 계산
if (matchPoints.Count == 4) // matchPoints 리스트의 크기가 4인지 확인
{
    var topLeft = new OpenCvSharp.Point((int)matchPoints[0].X, (int)matchPoints[0].Y);
    var bottomRight = new OpenCvSharp.Point((int)matchPoints[2].X, (int)matchPoints[2].Y);

    // Rect 객체 생성
    int width = bottomRight.X - topLeft.X; // 너비 계산
    int height = bottomRight.Y - topLeft.Y; // 높이 계산

    logger.Debug("미니맵 영역 검출 성공"); // 로그 출력

    return new OpenCvSharp.Rect(topLeft.X, topLeft.Y, width, height); // 왼쪽 상단 좌표, 너비, 높이를 사용하여 Rect 객체 생성
}
else
{
    logger.Debug("미니맵 영역 검출 실패"); // 로그 출력

    return new OpenCvSharp.Rect(); // 빈 Rect 객체 반환
}

*/