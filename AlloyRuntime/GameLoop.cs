using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine
{
    internal static class GameLoop
    {
        internal static event Action<float> OnFrame;
        internal static event Action OnTick;
    }
}
