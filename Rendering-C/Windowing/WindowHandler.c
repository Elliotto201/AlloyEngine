#include "WindowHandle.h"
#include <stdbool.h>

__declspec(dllexport) WindowHandle OpenWindow() {
    WindowHandle handle;
    handle.handle = 0;

    return handle;
}

__declspec(dllexport) bool CloseWindow(WindowHandle handle) {
    return true;
}
