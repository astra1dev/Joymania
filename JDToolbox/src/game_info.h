#ifndef JDTOOLBOX_GAME_INFO_H
#define JDTOOLBOX_GAME_INFO_H

#include <cstdint>
#include <string>
#include <vector>

struct GameInfo
{
    std::string gameName;
    std::string processName;
    uint64_t fileHash;
    uintptr_t baseAddress;
    std::vector<uintptr_t> healthOffsets;
    std::vector<uintptr_t> jumpOffsets;
};

inline std::vector<GameInfo> games = {
    { "Santa Claus in Trouble", "SantaClausInTrouble.exe", 0,
        0x0009FE1C, { 0x14, 0x50 }, { 0x14, 0x58 } },

    { "Rosso Rabbit in Trouble", "RossoRabbitInTrouble.exe", 0,
        0x000C00A8, { 0x14, 0x54 } },

    { "Santa Claus in Trouble... again!", "SantaClaus2.exe", 0,
        0x000B26C8, { 0x14, 0xA0 } },

    { "Santa Claus in Trouble (HD)", "SantaClausInTrouble.exe", 0}
};

#endif //JDTOOLBOX_GAME_INFO_H
