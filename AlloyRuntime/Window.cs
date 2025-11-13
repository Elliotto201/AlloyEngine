using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime
{
    internal sealed class Window : GameWindow
    {
        private readonly double TickRate;
        private double lastTickTime;

        public Window(string windowTitle, int width, int height, GameConfig config, WindowIcon icon = null) : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            Title = windowTitle,
            ClientSize = new OpenTK.Mathematics.Vector2i(width, height),
        })
        {
            if(icon != null)
            {
                Icon = new OpenTK.Windowing.Common.Input.WindowIcon(new OpenTK.Windowing.Common.Input.Image(icon.Width, icon.Height, icon.Data));
            }

            TickRate = 1 / config.TickRate;
            lastTickTime = 0;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            lastTickTime += args.Time;

            while (lastTickTime > TickRate)
            {
                lastTickTime -= TickRate;
                GameLoop.CallOnTick();
            }
            GameLoop.CallOnFrame((float)args.Time);
        }

        public sealed class GameConfig
        {
            public int TickRate;
        }

        public sealed class WindowIcon
        {
            public int Width;
            public int Height;
            public byte[] Data;

            public WindowIcon(int width, int height, byte[] data)
            {
                Width = width; 
                Height = height; 
                Data = data;
            }
        }
    }
}
