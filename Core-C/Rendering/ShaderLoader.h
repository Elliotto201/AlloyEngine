#ifndef SHADERLOADER_H
#define SHADERLOADER_H

#include "ShaderInfo.h"

#ifdef __cplusplus
extern "C" {
#endif

    ShaderInfo loadModelShader(const char* vertexSource, const char* fragmentSource);

#ifdef __cplusplus
}
#endif

#endif // SHADERLOADER_H
