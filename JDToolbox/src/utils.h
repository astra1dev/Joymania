#ifndef JDTOOLBOX_UTILS_H
#define JDTOOLBOX_UTILS_H

#include <vector>
#include <windows.h>

template<typename T>
bool WriteMemory(uintptr_t addr, const T& value);

static std::string FormatHex(uintptr_t value);

uintptr_t GetModuleBase(const wchar_t* moduleName);

uintptr_t ResolvePointer(uintptr_t base, const std::vector<uintptr_t>& offsets);

bool IsGameRunningAsAdmin();

std::vector<HMODULE> GetLoadedModules();


#endif //JDTOOLBOX_UTILS_H
