#ifndef SHADERLOADER_H
#define SHADERLOADER_H

#include "ShaderInfo.h"

#ifdef _WIN32
  #define DLL_EXPORT __declspec(dllexport)
#else
  #define DLL_EXPORT
#endif

#ifdef __cplusplus
extern "C" {
#endif

    DLL_EXPORT ShaderInfo loadModelShader(const char* vertexSource, const char* fragmentSource);

#ifdef __cplusplus
}
#endif

#endif // SHADERLOADER_H
