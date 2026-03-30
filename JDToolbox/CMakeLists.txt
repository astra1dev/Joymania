cmake_minimum_required(VERSION 3.30)
project(JDToolbox LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 20)

# Cross-compiling setup for Windows
set(CMAKE_SYSTEM_NAME Windows)
set(CMAKE_C_COMPILER i686-w64-mingw32-gcc)
set(CMAKE_CXX_COMPILER i686-w64-mingw32-g++)

# Create the DLL
add_library(JDToolbox SHARED
        src/entrypoint.cpp
)

target_sources(JDToolbox PRIVATE
        imgui/imgui.cpp
        imgui/imgui_draw.cpp
        imgui/imgui_tables.cpp
        imgui/imgui_widgets.cpp
        imgui/imgui_demo.cpp
        imgui/backends/imgui_impl_win32.cpp
        imgui/backends/imgui_impl_dx9.cpp
)

# Ensure correct exports
target_compile_definitions(JDToolbox PRIVATE UNICODE _UNICODE)

# Specify include directories
target_include_directories(JDToolbox PUBLIC imgui imgui/backends)

# Export symbols (optional, more control)
#[[set_target_properties(JDToolbox PROPERTIES
        WINDOWS_EXPORT_ALL_SYMBOLS YES
)]]

# Force DLL extension for Windows cross-compilation
set(CMAKE_SHARED_LIBRARY_SUFFIX ".dll")
set_target_properties(JDToolbox PROPERTIES
        PREFIX ""
        SUFFIX ".dll"
        OUTPUT_NAME "dinput8")

# Pass the .def file and enable stdcall fixup
target_link_options(JDToolbox PRIVATE
        dinput8.def
        -Wl,--enable-stdcall-fixup
        -static # I hate this because it increases file size by 10MB, but without this the game crashes in Proton
        #-static-libgcc
        #-static-libstdc++
)

target_link_libraries(JDToolbox PRIVATE d3d9 user32 gdi32 dwmapi)
