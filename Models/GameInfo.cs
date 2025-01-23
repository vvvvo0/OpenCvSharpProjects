﻿
/*
GameInfo 클래스: 
게임 관련 정보를 저장하는 Model입니다. 
게임 화면 영역과 유저 위치 정보를 저장하도록 구현되어 있습니다. 
필요에 따라 다른 게임 정보 (예: 객체 정보, 맵 정보)를 추가할 수 있습니다.
 */

using OpenCvSharp;
// `Rect`와 `Point` 형식은 `OpenCvSharp` 네임스페이스에 정의되어 있습니다.
// 따라서 `using OpenCvSharp;` 지시문을 추가해야 합니다.

namespace OpenCvSharpProjects.Models
{
    internal class GameInfo
    {
        public double PlayerHp { get; set; } // 플레이어의 현재 체력
        public Point PlayerPosition { get; set; } // 플레이어의 위치 (미니맵 상 좌표)
        public bool IsHpLow { get; set; } // 체력이 낮은 상태인지 여부


        // public Rect GameWindowRect { get; set; } // 게임 화면 영역
        
        // 필요에 따라 게임 정보 추가


        
    }
}