using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRendering
{
    public static partial class WindowHandler
    {
        [LibraryImport("AlloyRendering.dll")]
        private static partial WindowHandle _OpenWindow();
        //The u1 means the bool is marshaled as a 1 byte bool. Just exists for marshalling purposes.
        [return: MarshalAs(UnmanagedType.U1)]
        [LibraryImport("AlloyRendering.dll")]
        private static partial bool _CloseWindow(WindowHandle handle);


        /// <summary>
        /// Creates a new window and returns the window handle you can use to use functions and manage the window
        /// </summary>
        /// <returns></returns>
        public static WindowHandle CreateWindow()
        {
            return _OpenWindow();
        }

        /// <summary>
        /// Tries to close a window with specified WindowHandle and returns based on success or fail
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool CloseWindow(WindowHandle handle)
        {
            return _CloseWindow(handle);
        }
    }
}
