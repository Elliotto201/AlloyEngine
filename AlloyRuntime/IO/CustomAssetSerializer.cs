using AlloyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal abstract class CustomAssetSerializer
    {
        public abstract byte[] Serialize(EngineObject asset);
        public abstract object Deserialize(byte[] asset);
    }
}
