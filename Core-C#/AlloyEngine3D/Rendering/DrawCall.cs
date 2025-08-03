using AlloyEngine3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D_Internal
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DrawCall
    {
        public Matrix4x4 transform;
        public IntPtr meshPtr;
        public IntPtr materialPtr;
        public ShaderInfo shaderInfo;
    }
}
