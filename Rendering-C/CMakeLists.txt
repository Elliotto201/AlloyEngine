cmake_minimum_required(VERSION 3.15)
project(MyGLFWApp C CXX)

set(CMAKE_C_STANDARD 11)
set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -Wall -Wextra -Werror -std=c11")

find_package(OpenGL REQUIRED)
find_package(glfw3 REQUIRED)
find_package(cglm CONFIG REQUIRED)

add_library(MyGLFWApp SHARED
        glad/src/glad.c
        Rendering/SceneRenderer.cpp
        Rendering/DrawCall.h
        Rendering/ShaderLoader.c
        Rendering/ShaderLoader.h
        Rendering/ShaderInfo.h
        Windowing/WindowHandle.h
        Windowing/WindowHandler.c
)

target_include_directories(MyGLFWApp PRIVATE "${CMAKE_SOURCE_DIR}/glad/include")

target_compile_options(MyGLFWApp PRIVATE
        $<$<COMPILE_LANGUAGE:C>:-Wno-unused-parameter>
)

target_link_libraries(MyGLFWApp PRIVATE glfw OpenGL::GL cglm::cglm)
