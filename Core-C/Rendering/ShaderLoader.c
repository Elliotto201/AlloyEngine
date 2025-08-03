#include "ShaderLoader.h"

__declspec(dllexport)
ShaderInfo loadModelShader(const char* vertexSource, const char* fragmentSource) {
    (void)vertexSource;
    (void)fragmentSource;

    ShaderInfo shader = {0};

    return shader;
}
