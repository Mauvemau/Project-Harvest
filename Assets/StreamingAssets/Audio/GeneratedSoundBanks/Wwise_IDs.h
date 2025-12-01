/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID GAME_LOSE = 3425053597U;
        static const AkUniqueID GAME_WIN = 3218375656U;
        static const AkUniqueID LAWNMOWER_END = 773559505U;
        static const AkUniqueID LAWNMOWER_HIT = 188167277U;
        static const AkUniqueID LAWNMOWER_START = 3994001794U;
        static const AkUniqueID MANDRAKE_STEPS = 282088238U;
        static const AkUniqueID PLAY_BEEP = 2063165418U;
        static const AkUniqueID PLAY_BLACKBERRYHIT = 2538688676U;
        static const AkUniqueID PLAY_BOOMERANGTHROW = 2116017054U;
        static const AkUniqueID PLAY_BUTTONHOVER = 479606568U;
        static const AkUniqueID PLAY_BUTTONPAUSE = 2750761056U;
        static const AkUniqueID PLAY_BUTTONPRESS = 2652178615U;
        static const AkUniqueID PLAY_FLAME = 1506124227U;
        static const AkUniqueID PLAY_MANDRAGORAHEALERSPAWN = 255250776U;
        static const AkUniqueID PLAY_MANDRAGORASHOOTER = 3211203716U;
        static const AkUniqueID PLAY_MANDRAGORASHOOTERDEATH = 3898840282U;
        static const AkUniqueID PLAY_MANDRAKEEXPLOSION = 568556130U;
        static const AkUniqueID PLAY_MANDRAKEEXPLOSIONBEEP = 2058664570U;
        static const AkUniqueID PLAY_PLAYERRECEIVEDAMAGE = 1724187815U;
        static const AkUniqueID PLAY_SCHYTHEATTACK = 126154U;
        static const AkUniqueID PLAY_SFX_HEALTH = 645491656U;
        static const AkUniqueID PLAY_SHOOTERMANDRAKECRY = 1803350251U;
        static const AkUniqueID PLAY_SHOOTERMANDRAKERECIBEDAMAGE = 1618169002U;
        static const AkUniqueID PLAY_SICKLE_HIT = 14983643U;
        static const AkUniqueID PLAY_SMALLMANDRAKECRY = 3814284364U;
        static const AkUniqueID PLAY_SMALLMANDRAKERECIBEDAMAGE = 1332341351U;
        static const AkUniqueID PLAY_SXF_XPEXTRA = 2569333368U;
        static const AkUniqueID PLAY_TOROMANDRAKECRY = 4031256493U;
        static const AkUniqueID PLAY_TOROMANDRAKERECIBEDAMAGE = 1971773272U;
        static const AkUniqueID PLAY_XPBAR = 3421228173U;
        static const AkUniqueID START = 1281810935U;
        static const AkUniqueID STOP_MANDRAKEEXPLOSIONBEEP = 3019132932U;
        static const AkUniqueID TARANTULA_ATTACK = 4165231998U;
        static const AkUniqueID TARANTULA_HIT = 3994421637U;
        static const AkUniqueID WITCH_STEPS = 2001225474U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GAMEPLAYSITUATION
        {
            static const AkUniqueID GROUP = 2775940631U;

            namespace STATE
            {
                static const AkUniqueID HARVEST_COMBAT = 4182149263U;
                static const AkUniqueID HARVEST_UPGRADEMENU = 2802927180U;
                static const AkUniqueID LOSE = 221232726U;
                static const AkUniqueID MAINMENU = 3604647259U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID WIN = 979765101U;
            } // namespace STATE
        } // namespace GAMEPLAYSITUATION

    } // namespace STATES

    namespace SWITCHES
    {
        namespace ATTACK_TARANTULA
        {
            static const AkUniqueID GROUP = 2860818536U;

            namespace SWITCH
            {
                static const AkUniqueID HIT = 1116398592U;
                static const AkUniqueID LOOP = 691006007U;
            } // namespace SWITCH
        } // namespace ATTACK_TARANTULA

        namespace BOOMERANG
        {
            static const AkUniqueID GROUP = 1198215643U;

            namespace SWITCH
            {
                static const AkUniqueID BOOMERANG_LEV3 = 1218330042U;
                static const AkUniqueID BOOMERANG_LEV12 = 152774810U;
            } // namespace SWITCH
        } // namespace BOOMERANG

        namespace LAWNMOWER
        {
            static const AkUniqueID GROUP = 2132422351U;

            namespace SWITCH
            {
                static const AkUniqueID OFF = 930712164U;
                static const AkUniqueID ON = 1651971902U;
            } // namespace SWITCH
        } // namespace LAWNMOWER

        namespace MANDRAKE_TORO
        {
            static const AkUniqueID GROUP = 1113477063U;

            namespace SWITCH
            {
                static const AkUniqueID NO = 1668749452U;
                static const AkUniqueID YES = 979470758U;
            } // namespace SWITCH
        } // namespace MANDRAKE_TORO

        namespace PLAYER_LIFE_SWITCH
        {
            static const AkUniqueID GROUP = 2520085654U;

            namespace SWITCH
            {
                static const AkUniqueID HEALTHY = 2874639328U;
                static const AkUniqueID NEARLY_DEFEATED = 1898225071U;
                static const AkUniqueID WOUNDED = 1764828697U;
            } // namespace SWITCH
        } // namespace PLAYER_LIFE_SWITCH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID BEEPSPEED = 1101615186U;
        static const AkUniqueID MELODY_VOLUME = 363245510U;
        static const AkUniqueID MUSIC_VOLUME = 1006694123U;
        static const AkUniqueID PLAYER_HEALTH = 215992295U;
        static const AkUniqueID TORO_DISTANCE = 1585544231U;
        static const AkUniqueID XPBAR = 1013953904U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID GENERAL = 133642231U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID ATAQUES_SPAWN = 3213435389U;
        static const AkUniqueID GRITOS = 89996055U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSICBUS = 2886307548U;
        static const AkUniqueID SFXBUS = 3803850708U;
        static const AkUniqueID UI = 1551306167U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
