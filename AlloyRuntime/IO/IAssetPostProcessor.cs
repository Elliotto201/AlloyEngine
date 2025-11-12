using AlloyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal interface IAssetPostProcessor<T> where T : EngineObject
    {
        public void Process(EngineObject obj);
    }
}
