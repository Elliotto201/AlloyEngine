#ifndef MESHRENDERER_H
#define MESHRENDERER_H

#include <list>

#include "DrawCall.h"

#ifdef __cplusplus
extern "C" {
#endif

    void RenderFrame(DrawCall* drawCalls, int length);

#ifdef __cplusplus
}
#endif

#endif
