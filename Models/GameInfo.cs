
/*
GameInfo 클래스: 
게임 관련 정보를 저장하는 Model입니다. 
게임 화면 영역과 유저 위치 정보를 저장하도록 구현되어 있습니다. 
필요에 따라 다른 게임 정보 (예: 객체 정보, 맵 정보)를 추가할 수 있습니다.
 */



using System.Collections.Generic;
// List<T> 클래스를 사용하기 위해 필요한 네임스페이스이다.
// 특징점 정보를 저장하는 List<KeyPoint> 타입의 Keypoints 속성을 위해 포함했다.

using OpenCvSharp;
// `Rect`와 `Point` 형식은 `OpenCvSharp` 네임스페이스에 정의되어 있습니다.
// 따라서 `using OpenCvSharp;` 지시문을 추가해야 합니다.

namespace OpenCvSharpProjects.Models
{
    public class GameInfo
    {
        public Rect GameWindowRect { get; set; } // 게임 화면 영역을 저장하는 속성이다.
        public Point UserLocation { get; set; } // 유저의 위치를 저장하는 속성이다.

        public List<KeyPoint> Keypoints { get; set; } = new List<KeyPoint>(); // 특징점 정보

        public Mat? Descriptors { get; set; } // 특징점 기술자


        public bool IsMinimapDetected { get; set; } // 미니맵 인식 여부

        // TODO: 필요에 따라 몬스터, NPC, 아이템 등의 정보를 추가할 수 있다.
    }
}



// public double PlayerHp { get; set; } // 플레이어의 현재 체력
// public Point PlayerPosition { get; set; } // 플레이어의 위치 (미니맵 상 좌표)
// public bool IsHpLow { get; set; } // 체력이 낮은 상태인지 여부
