#include <windows.h>
#include "gui.cpp"
#include "dinput8/dinput8.cpp"

HANDLE hCurrentUIThread = nullptr;

// DLL entry point, see https://learn.microsoft.com/en-us/windows/win32/dlls/dllmain
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
    switch (fdwReason)
    {
        case DLL_PROCESS_ATTACH:
            //MessageBoxA(NULL, "[JDToolbox] DLL Loaded Successfully!", "Notification", MB_OK | MB_ICONINFORMATION);
            hCurrentUIThread = CreateThread(NULL, 0, ImGuiThread, NULL, 0, NULL);
            Logger::Log("DllMain called with DLL_PROCESS_ATTACH");
            break;
        case DLL_THREAD_ATTACH:
            // Thread-specific initialization code here
            break;
        case DLL_THREAD_DETACH:
            // Thread-specific cleanup code here
            break;
        case DLL_PROCESS_DETACH:
            Logger::Log("DllMain called with DLL_PROCESS_DETACH (lpvReserved=" + FormatHex((uintptr_t)lpvReserved) + ")");

            TerminateThread(hCurrentUIThread, 0);

            if (realDll != nullptr)
            {
                real_DirectInput8Create = nullptr;

                FreeLibrary(realDll);
                realDll = nullptr;
            }

            break;
    }
    return TRUE; // Successful initialization
}
