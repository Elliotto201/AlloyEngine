#include "SceneRenderer.h"

extern "C" __declspec(dllexport)
void RenderFrame(DrawCall* drawCalls, int length) {
    for (int i = 0; i < length; i++) {
        DrawCall& drawCall = drawCalls[i];
    }
}
