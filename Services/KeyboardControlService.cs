// Services/KeyboardControlService.cs
using System.Threading.Tasks;
using WindowsInput;


namespace OpenCvSharpProjects.Services
{
    public class KeyboardControlService
    {
        private readonly InputSimulator simulator = new InputSimulator(); // InputSimulator 객체 생성

        public async Task PressKeyAsync(WindowsInput.Native.VirtualKeyCode key) // VirtualKeyCode 사용
        {
            // 키보드 입력을 시뮬레이션합니다.
            await Task.Run(() => simulator.Keyboard.KeyPress(key)); // simulator.Keyboard.KeyPress() 사용
        }
    }
}