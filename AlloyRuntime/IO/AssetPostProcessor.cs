using AlloyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal abstract class AssetPostProcessor
    {
        public abstract void Process(EngineObject obj);
    }
}
