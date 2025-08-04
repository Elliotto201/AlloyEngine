using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRendering
{
    public static partial class DrawCallScheduler
    {
        [LibraryImport("AlloyRendering.dll")]
        private static partial void RenderFrame(WindowHandle handle, [In] DrawCall[] drawCalls, int count);
    }
}
