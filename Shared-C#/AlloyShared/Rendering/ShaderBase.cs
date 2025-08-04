using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D.Rendering
{
    public abstract class ShaderBase
    {
        public abstract string FragmentPath { get; }
        public abstract string VertexPath { get; }
    }
}
