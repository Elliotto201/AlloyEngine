using AlloyShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D
{
    public abstract class EngineObject
    {
        public string Name { get; private set; }
        public AssetId AssetId { get; private set; }

        public abstract bool IsVisibleAsset { get; }
    }
}
