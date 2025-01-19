/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/

using OpenCvSharp;
// `Rect`와 `Point` 형식은 `OpenCvSharp` 네임스페이스에 정의되어 있습니다.
// 따라서 `using OpenCvSharp;` 지시문을 추가해야 합니다.

namespace OpenCvSharpProjects.Models
{
    internal class GameInfo
    {
        
            public Rect GameWindowRect { get; set; } // 게임 화면 영역
            public Point UserLocation { get; set; } // 유저 위치
        
    }
}
