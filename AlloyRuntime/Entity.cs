using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine
{
    public sealed class Entity : EngineObject
    {
        private List<Component> components = new();

        private void GameLoop_OnFrame(float deltaTime)
        {
            foreach(var component in components)
            {
                component.CallUpdate(deltaTime);
            }
        }

        private void GameLoop_OnTick()
        {
            foreach (var component in components)
            {
                component.CallTick();
            }
        }

        protected override void Create()
        {
            GameLoop.OnFrame += GameLoop_OnFrame;
            GameLoop.OnTick += GameLoop_OnTick;
            foreach(var component in components)
            {
                component.CallStart();
            }
        }

        protected override void Dispose()
        {
            foreach(var component in components)
            {
                component.CallDestroy();
            }
        }
    }
}
