using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime
{
    internal static class GameLoop
    {
        internal static event Action<float> OnFrame;
        internal static event Action OnTick;

        internal static void CallOnFrame(float time)
        {
            OnFrame?.Invoke(time);
        }

        internal static void CallOnTick()
        {
            OnTick?.Invoke();
        }
    }
}
