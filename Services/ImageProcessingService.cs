using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OpenCvSharp;

namespace OpenCvSharpProjects.Services
{
    internal class ImageProcessingService
    {
        private Mat _sourceImage;
        private Mat _miniMap;
        private Rect _playerHpRoi;
        private Rect _miniMapRoi;

        public Mat MiniMapImage { get; private set; }

        // ... 기타 필요한 변수들

        public void ProcessImage(Mat sourceImage)
        {
            _sourceImage = sourceImage;

            // 미니맵 영역 자르기
            OpenCvSharp.Rect miniMapRect = new Rect(_miniMapRoi.X, _miniMapRoi.Y, _miniMapRoi.Width, _miniMapRoi.Height);
            _miniMap = new Mat(_sourceImage, miniMapRect);
            


            // 플레이어 체력 게이지 영역 자르기
            OpenCvSharp.Rect hpRect = new Rect(_playerHpRoi.X, _playerHpRoi.Y, _playerHpRoi.Width, _playerHpRoi.Height);
            Mat hpImage = new Mat(_sourceImage, hpRect);



            // 플레이어 위치 찾기 (템플릿 매칭 등 사용)
            // ...

            // 플레이어 체력 계산 (색상 분석 등 사용)
            // ...

            // GameInfo 객체 업데이트
            // ...
        }

        // ... 기타 이미지 처리 함수들
    }

}

