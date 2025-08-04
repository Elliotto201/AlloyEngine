@echo off
setlocal

:: === CONFIG ===
set CONFIG=Debug
set DIST_DIR=Build
set CS_SOLUTION=Rendering-C#\AlloyEngine3D\AlloyEngine3D.sln
set CMAKE_DIR=Rendering-C
set CMAKE_BUILD_DIR=%CMAKE_DIR%\build
set VCPKG_CACHE_FILE=vcpkg_path.txt

:: === LOAD OR ASK FOR VCPKG PATH ===
if exist "%VCPKG_CACHE_FILE%" (
    set /p VCPKG_ROOT=<%VCPKG_CACHE_FILE%
    echo Using cached vcpkg path: %VCPKG_ROOT%
) else (
    set /p VCPKG_ROOT=Enter full path to your vcpkg folder ^(e.g. C:\dev\vcpkg^): 
    echo %VCPKG_ROOT% > %VCPKG_CACHE_FILE%
)

if not exist "%VCPKG_ROOT%\scripts\buildsystems\vcpkg.cmake" (
    echo ERROR: vcpkg toolchain file not found at "%VCPKG_ROOT%\scripts\buildsystems\vcpkg.cmake"
    pause
    exit /b 1
)
set VCPKG_TOOLCHAIN=%VCPKG_ROOT%\scripts\buildsystems\vcpkg.cmake

:: === CLEAN DIST ===
echo Cleaning build directory...
if exist "%DIST_DIR%" rd /s /q "%DIST_DIR%"
mkdir "%DIST_DIR%"

:: === BUILD C PROJECT ===
echo Building C project...
if exist "%CMAKE_BUILD_DIR%" rd /s /q "%CMAKE_BUILD_DIR%"
mkdir "%CMAKE_BUILD_DIR%"
pushd "%CMAKE_BUILD_DIR%"
cmake .. -DCMAKE_BUILD_TYPE=%CONFIG% -DCMAKE_TOOLCHAIN_FILE=%VCPKG_TOOLCHAIN%
if errorlevel 1 (
    echo CMake configuration failed.
    popd
    pause
    exit /b 1
)
cmake --build . --config %CONFIG%
if errorlevel 1 (
    echo CMake build failed.
    popd
    pause
    exit /b 1
)
popd

:: === COPY C DLLs ===
echo Copying C output...
xcopy /y /s /i "%CMAKE_BUILD_DIR%\%CONFIG%\*.dll" "%DIST_DIR%\" >nul 2>&1
xcopy /y /s /i "%CMAKE_BUILD_DIR%\*.dll" "%DIST_DIR%\" >nul 2>&1

:: === BUILD C# PROJECT ===
echo Building C# project...
dotnet build "%CS_SOLUTION%" -c %CONFIG%
if errorlevel 1 (
    echo C# build failed.
    pause
    exit /b 1
)

:: === COPY C# DLLs/EXEs ===
echo Copying C# output...
xcopy /y /s /i "Rendering-C#\AlloyEngine3D\bin\%CONFIG%\net8.0\*.dll" "%DIST_DIR%\" >nul 2>&1
xcopy /y /s /i "Rendering-C#\AlloyEngine3D\bin\%CONFIG%\net8.0\*.exe" "%DIST_DIR%\" >nul 2>&1

echo.
echo Build complete. Files are in: %DIST_DIR%
pause

endlocal
