#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include "LevelEditorWindow.h"
#include <stdexcept>
#include <cstdio>

LevelEditorWindow::LevelEditorWindow(int width, int height, const char* title) : window(nullptr) {
    if (!glfwInit()) {
        throw std::runtime_error("Failed to initialize GLFW");
    }

    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

    window = glfwCreateWindow(width, height, title, nullptr, nullptr);
    if (!window) {
        glfwTerminate();
        throw std::runtime_error("Failed to create GLFW window");
    }

    glfwMakeContextCurrent(window);

    if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress)) {
        glfwDestroyWindow(window);
        glfwTerminate();
        throw std::runtime_error("Failed to initialize GLAD");
    }
}

LevelEditorWindow::~LevelEditorWindow() {
    if (window) {
        glfwDestroyWindow(window);
    }
    glfwTerminate();
}

bool LevelEditorWindow::ShouldClose() const {
    return window ? glfwWindowShouldClose(window) : true;
}

void LevelEditorWindow::SwapBuffers() {
    if (window) {
        glfwSwapBuffers(window);
    }
}

void LevelEditorWindow::PollEvents() const {
    glfwPollEvents();
}

void LevelEditorWindow::Clear(float r, float g, float b, float a) const {
    glClearColor(r, g, b, a);
    glClear(GL_COLOR_BUFFER_BIT);
}
