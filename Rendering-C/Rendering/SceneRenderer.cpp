#include "DrawCall.h"
#include "../Windowing/WindowHandle.h"

__declspec(dllexport) void RenderFrame(WindowHandle handle, DrawCall* drawCalls, int length) {
    for (int i = 0; i < length; i++) {
        DrawCall& drawCall = drawCalls[i];
    }
}
