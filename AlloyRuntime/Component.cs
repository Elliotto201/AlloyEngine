using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine
{
    public abstract class Component
    {
        protected virtual void OnStart() { }
        protected virtual void OnTick() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnDestroy() { }

        internal void CallStart()
        {
            OnStart();
        }

        internal void CallTick()
        {
            OnTick();
        }

        internal void CallUpdate(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        internal void CallDestroy()
        {
            OnDestroy();
        }
    }
}
