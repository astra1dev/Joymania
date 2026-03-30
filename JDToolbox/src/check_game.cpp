#include "check_game.h"
#include <windows.h>
#include "logger.h"
#include <vector>
#include <string>

struct GameInfo;

bool DetectHash()
{
    return true;
}

bool DetectProductName()
{
    return true;
}

bool DetectProcessName()
{
    return true;
}

bool DetectGame(GameInfo*& outGame) {
    wchar_t processPath[MAX_PATH];
    if (GetModuleFileNameW(NULL, processPath, MAX_PATH) == 0) {
        Logger::Log("DetectGame: GetModuleFileNameW failed");
        return false;
    }

    std::wstring processNameW = processPath;
    size_t lastBackslash = processNameW.find_last_of(L"\\/");
    if (lastBackslash != std::wstring::npos) {
        processNameW = processNameW.substr(lastBackslash + 1);
    }
    std::string processName(processNameW.begin(), processNameW.end());
    std::transform(processName.begin(), processName.end(), processName.begin(), ::tolower);

    Logger::Log("DetectGame: Current process name: " + processName);

    for (auto& game : games) {
        std::string gameProcessName = game.processName;
        std::transform(gameProcessName.begin(), gameProcessName.end(), gameProcessName.begin(), ::tolower);
        if (processName == gameProcessName) {
            outGame = &game;
            Logger::Log("DetectGame: Detected game: " + game.gameName);
            return true;
        }
    }

    Logger::Log("DetectGame: No matching game found");
    return false;
}

std::vector<std::string>* GetCurrentGame() {
    if (!DetectHash())
    {

    }
    return &GameNames;
}
