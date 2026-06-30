#ifndef JDTOOLBOX_UTILS_H
#define JDTOOLBOX_UTILS_H

#include <vector>
#include <windows.h>
#include "game_info.h"

static std::string FormatHex(uintptr_t value);

GameInfo* DetectGame();

uintptr_t ResolvePointer(uintptr_t base, const std::vector<uintptr_t>& offsets);

bool IsGameRunningAsAdmin();

std::vector<HMODULE> GetLoadedModules();


#endif //JDTOOLBOX_UTILS_H
