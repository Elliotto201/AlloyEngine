using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D_Internal
{
    public static partial class DrawCallScheduler
    {
        [LibraryImport("AlloyRendering.dll")]
        public static partial void RenderFrame([In] DrawCall[] drawCalls, int count);
    }
}
