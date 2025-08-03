#ifndef DRAWCALL_H
#define DRAWCALL_H

#include<cglm/cglm.h>

#include "ShaderInfo.h"

typedef struct {
    mat4 model;
    void* meshPtr;
    void* materialPtr;
    ShaderInfo shaderInfo;
} DrawCall;

#endif
