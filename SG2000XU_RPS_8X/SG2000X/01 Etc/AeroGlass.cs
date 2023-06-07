using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

namespace AeroGlassWindow
{
    // Aero Glass가 적용될 영역의 여백
    public struct AeroMargin
    {
        public int Left, Right, Top, Bottom;
        public AeroMargin(int Left, int Right, int Top, int Bottom)
        {
            this.Left = Left;
            this.Right = Right;
            this.Top = Top;
            this.Bottom = Bottom;
        }
    }

    public static class AeroGlassHelper
    {
        /////////////////////////////////////////////////////////////////////////////////////
        // Desktop Windows Manger API
        /////////////////////////////////////////////////////////////////////////////////////
        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref AeroMargin margins);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();
        /////////////////////////////////////////////////////////////////////////////////////

        public static bool SetAreoGlass(this Window Window)
        {
            // 여백의 영역을 모두 -1로 설정하여 전체화면으로 지정.
            return SetAreoGlass(Window, new AeroMargin(-1, -1, -1, -1));
        }
        public static bool SetAreoGlass(this Window Window, AeroMargin Margin)
        {
            // 현재 시스템에서 DWM을 사용할 수 있는지를 판단
            if (!DwmIsCompositionEnabled()) return false;

            // 해당 Window에서 Win32 Handle을 얻는다.
            // 주의 : Window가 Load되기 전에는 Handle을 얻을 수 없으므로
            //        Loaded이벤트가 발생한 이후 이 함수를 호출해야한다.
            IntPtr hwnd = new WindowInteropHelper(Window).Handle;

            // Aero Glass를 적용하기위해 배경색을 투명으로 설정한다. 
            // (다른색으로 할경우 적용안됨)
            Window.Background = Brushes.Transparent;
            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

            // Aero Glass를 적용한다.
            DwmExtendFrameIntoClientArea(hwnd, ref Margin);
            return true;
        }
    }
}
