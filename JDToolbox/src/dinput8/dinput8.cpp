#include <windows.h>
#include <iostream>
#include "../logger.h"
#include "../utils.h"

static HMODULE realDll = nullptr;
typedef HRESULT (WINAPI *PFN_DirectInput8Create)(HINSTANCE hinst, DWORD dwVersion, REFIID riid, LPVOID* ppvOut, LPUNKNOWN punkOuter);
static PFN_DirectInput8Create real_DirectInput8Create = nullptr;

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
