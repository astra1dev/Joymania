#include <windows.h>
#include "gui.cpp"

HANDLE hCurrentUIThread = nullptr;

static HMODULE realDll = nullptr;
typedef HRESULT (WINAPI *PFN_DirectInput8Create)(HINSTANCE hinst, DWORD dwVersion, REFIID riid, LPVOID* ppvOut, LPUNKNOWN punkOuter);
static PFN_DirectInput8Create real_DirectInput8Create = nullptr;

/// DLL entry point, see https://learn.microsoft.com/en-us/windows/win32/dlls/dllmain
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

extern "C" HRESULT __stdcall DirectInput8Create(HINSTANCE hinst, DWORD dwVersion, REFIID riid, LPVOID* ppvOut, LPUNKNOWN punkOuter)
{
    Logger::Log(
        "DirectInput8Create called (hinst=" + FormatHex((uintptr_t)hinst) + ", dwVersion=" + FormatHex(dwVersion) +
        ", riid=?, ppvOut=" + FormatHex((uintptr_t)ppvOut) + ", punkOuter=" + FormatHex((uintptr_t)punkOuter) + ")");

    if (!realDll) {
        // Only load once
        WCHAR sysDir[MAX_PATH];
        GetSystemDirectoryW(sysDir, MAX_PATH);  // C:\Windows\System32 (64-bit), or SysWOW64 for 32-bit processes
        wcscat(sysDir, L"\\dinput8.dll");

        realDll = LoadLibraryW(sysDir);
        if (!realDll)
        {
            Logger::Log("Failed to load dinput8.dll from: " + std::string(sysDir, sysDir + wcslen(sysDir)) + ", last error: " + std::to_string(GetLastError()));
            return E_FAIL;
        }
        Logger::Log("Loaded dinput8.dll from: " + std::string(sysDir, sysDir + wcslen(sysDir)));

        real_DirectInput8Create = (PFN_DirectInput8Create)GetProcAddress(realDll, "DirectInput8Create");
        // const auto realfun = reinterpret_cast<decltype(&DirectInput8Create)>(GetProcAddress(realDll, "DirectInput8Create")); // from nathan baggs
        if (!real_DirectInput8Create)
        {
            Logger::Log("Failed to get address of DirectInput8Create");
            return E_FAIL;
        }
    }

    auto result = real_DirectInput8Create(hinst, dwVersion, riid, ppvOut, punkOuter);
    Logger::Log("Original DirectInput8Create returned: " + FormatHex(result));

    return result;
}
