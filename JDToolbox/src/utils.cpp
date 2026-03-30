#include "utils.h"
#include "logger.h"
#include <sstream>
#include <cstring>
#include <psapi.h>

/// <summary>Write a value to a specified memory address</summary>
/// <param name="addr">Target memory address</param>
/// <param name="value">Value to write</param>
/// <returns>True on success, false on failure</returns>
template<typename T>
bool WriteMemory(const uintptr_t addr, const T& value)
{
    if (addr == 0) return false;
    if (IsBadWritePtr(reinterpret_cast<void*>(addr), sizeof(T)))
    {
        Logger::Log("WriteMemory: Invalid write pointer at address " + FormatHex(addr));
        return false;
    }
    std::memcpy(reinterpret_cast<void*>(addr), &value, sizeof(T));
    return true;
}

/// <summary>Helper to format pointer / address as hex string</summary>
/// <param name="value">Pointer or address value</param>
/// <returns>Formatted hex string (e.g., "0xDEADBEEF")</returns>
static std::string FormatHex(const uintptr_t value)
{
    std::ostringstream ss;
    ss << "0x" << std::hex << std::uppercase << value;
    return ss.str();
}

/// <summary>Helper to get module base address</summary>
/// <param name="moduleName">Name of the module (e.g., "game.exe")</param>
/// <returns>Base address of the module or 0 if not found</returns>
uintptr_t GetModuleBase(const wchar_t* moduleName) {
    HMODULE hModule = GetModuleHandleW(moduleName);

    char moduleNameA[MAX_PATH]; // Convert to ANSI for logging
    wcstombs(moduleNameA, moduleName, MAX_PATH);

    if (!hModule) {
        Logger::Log(std::string("GetModuleHandleW failed for module: ") + moduleNameA);
        return 0;
    }
    Logger::Log(std::string("GetModuleBase: ") + moduleNameA);
    return reinterpret_cast<uintptr_t>(hModule);
}

/// <summary>Resolve a multi-level pointer chain</summary>
/// <param name="base">Base address (e.g., module base + static offset)</param>
/// <param name="offsets">Vector of offsets to traverse</param>
/// <returns>Final resolved address or 0 on failure</returns>
uintptr_t ResolvePointer(const uintptr_t base, const std::vector<uintptr_t>& offsets)
{
    if (base == 0) return 0;
    if (offsets.empty()) return base;

    uintptr_t ptr = 0;

    // Read the initial pointer stored at `base` (e.g., module + baseAddress)
    if (IsBadReadPtr(reinterpret_cast<void*>(base), sizeof(uintptr_t))) {
        Logger::Log("ResolvePointer: Invalid read at base " + FormatHex(base));
        return 0;
    }
    // use memcpy to avoid UB from directly dereferencing arbitrary addresses
    std::memcpy(&ptr, reinterpret_cast<void*>(base), sizeof(ptr));
    if (ptr == 0) {
        Logger::Log("ResolvePointer: Null initial pointer read from " + FormatHex(base));
        return 0;
    }

    // Walk the chain: for each offset except the last, read the pointer at (ptr + offset)
    for (size_t i = 0; i + 1 < offsets.size(); ++i) {
        const uintptr_t addr = ptr + offsets[i];
        if (IsBadReadPtr(reinterpret_cast<void*>(addr), sizeof(uintptr_t))) {
            Logger::Log("ResolvePointer: Invalid read at " + FormatHex(addr));
            return 0;
        }
        std::memcpy(&ptr, reinterpret_cast<void*>(addr), sizeof(ptr));
        if (ptr == 0) {
            Logger::Log("ResolvePointer: Null pointer read from " + FormatHex(addr));
            return 0;
        }
    }

    // Final address points to value: ptr + lastOffset
    const uintptr_t finalAddr = ptr + offsets.back();
    if (IsBadReadPtr(reinterpret_cast<void*>(finalAddr), 1)) {
        Logger::Log("ResolvePointer: Invalid final address " + FormatHex(finalAddr));
        return 0;
    }

    return finalAddr;
}

/// <summary>Check if the current process is running with admin privileges</summary>
/// <returns>True if running as admin, false otherwise</returns>
bool IsGameRunningAsAdmin()
{
    BOOL elevated = FALSE;
    HANDLE token = nullptr;
    if (OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &token)) {
        TOKEN_ELEVATION te;
        DWORD ret = 0;
        if (GetTokenInformation(token, TokenElevation, &te, sizeof(te), &ret))
            elevated = static_cast<bool>(te.TokenIsElevated);
        CloseHandle(token);
    }
    return elevated == TRUE;
}

/// <summary>Get a list of all loaded modules in the current process</summary>
/// <returns>Vector of HMODULE handles for loaded modules</returns>
std::vector<HMODULE> GetLoadedModules()
{
    std::vector<HMODULE> modules(1024);
    DWORD needed = 0;
    if (EnumProcessModules(GetCurrentProcess(), modules.data(), modules.size() * sizeof(HMODULE), &needed))
    {
        modules.resize(needed / sizeof(HMODULE));
        return modules;
    }
    return {};
}
