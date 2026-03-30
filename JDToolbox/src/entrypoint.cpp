#include <windows.h>
#include "gui.cpp"

HANDLE hCurrentUIThread = nullptr;

/// <summary>DLL entry point.</summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/dlls/dllmain">Documentation</see>
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
    switch (fdwReason)
    {
        case DLL_PROCESS_ATTACH:
            MessageBoxA(NULL, "[JDToolbox] DLL Loaded Successfully!", "Notification", MB_OK | MB_ICONINFORMATION);
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
            TerminateThread(hCurrentUIThread, 0);
            break;
    }
    return TRUE; // Successful initialization
}


typedef HRESULT (WINAPI *PFN_DirectInput8Create)(HINSTANCE hinst, DWORD dwVersion, REFIID riid, LPVOID* ppvOut, LPUNKNOWN punkOuter);
static HMODULE realDll = NULL;
static PFN_DirectInput8Create real_DirectInput8Create = NULL;

/// <summary>Proxy function for <c>DirectInput8Create</c></summary>
extern "C" HRESULT __stdcall DirectInput8Create(HINSTANCE hinst, DWORD dwVersion, REFIID riid, LPVOID* ppvOut, LPUNKNOWN punkOuter)
{
    if (!realDll) {
        // Only load once
        WCHAR sysDir[MAX_PATH];
        GetSystemDirectoryW(sysDir, MAX_PATH);  // C:\Windows\System32 (64-bit), or SysWOW64 for 32-bit processes
        wcscat(sysDir, L"\\dinput8.dll");
        Logger::Log("Loading DirectInput8 from: " + std::string(sysDir, sysDir + wcslen(sysDir)));

        realDll = LoadLibraryW(sysDir);
        if (!realDll) return E_FAIL;

        real_DirectInput8Create = (PFN_DirectInput8Create)GetProcAddress(realDll, "DirectInput8Create");
        // const auto realfun = reinterpret_cast<decltype(&DirectInput8Create)>(GetProcAddress(realDll, "DirectInput8Create")); // from nathan baggs
        if (!real_DirectInput8Create) return E_FAIL;
    }
    Logger::Log("DirectInput8Create called, forwarding to real function.");

    return real_DirectInput8Create(hinst, dwVersion, riid, ppvOut, punkOuter);
}
