#ifndef LEVELEDITORWINDOW_H
#define LEVELEDITORWINDOW_H

#include <string>
#include <GLFW/glfw3.h>

class LevelEditorWindow {
public:
    LevelEditorWindow(int width, int height, const char* title);
    ~LevelEditorWindow();

    bool ShouldClose() const;
    void SwapBuffers();
    void PollEvents() const;
    void Clear(float r, float g, float b, float a) const;

private:
    GLFWwindow* window;

    // Prevent copying
    LevelEditorWindow(const LevelEditorWindow&) = delete;
    LevelEditorWindow& operator=(const LevelEditorWindow&) = delete;
};

#endif // LEVELEDITORWINDOW_H
