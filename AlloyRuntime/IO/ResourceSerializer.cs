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

        protected T CreateResource<T>(params object[] constructorArgs) where T : EngineResource
        {
            var t = Activator.CreateInstance(typeof(T), constructorArgs);

            return (T)t;
        }
    }
}
