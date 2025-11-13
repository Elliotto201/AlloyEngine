using AlloyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal abstract class ResourceSerializer
    {
        public abstract byte[] Serialize(byte[] rawAsset);
        public abstract object Deserialize(byte[] asset);
    }
}
