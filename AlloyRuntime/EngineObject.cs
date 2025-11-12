using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine
{
    public abstract class EngineObject
    {

        internal void CreateObject()
        {

        }

        internal void DisposeObject()
        {

        }

        protected abstract void Create();
        protected abstract void Dispose();
    }
}
