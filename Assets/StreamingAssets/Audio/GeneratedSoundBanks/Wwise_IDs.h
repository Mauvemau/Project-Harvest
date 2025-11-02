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
        static const AkUniqueID PLAY_BLACKBERRYHIT = 2538688676U;
        static const AkUniqueID PLAY_BOOMERANGTHROW = 2116017054U;
        static const AkUniqueID PLAY_BUTTONHOVER = 479606568U;
        static const AkUniqueID PLAY_BUTTONPRESS = 2652178615U;
        static const AkUniqueID PLAY_MANDRAGORASHOOTER = 3211203716U;
        static const AkUniqueID PLAY_SCHYTHEATTACK = 126154U;
        static const AkUniqueID PLAY_TARANTULAATTACK = 3483513970U;
        static const AkUniqueID START = 1281810935U;
        static const AkUniqueID TARANTULA_HIT = 3994421637U;
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
                static const AkUniqueID NONE = 748895195U;
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
        static const AkUniqueID MUSIC_VOLUME = 1006694123U;
        static const AkUniqueID PLAYER_HEALTH = 215992295U;
        static const AkUniqueID TORO_DISTANCE = 1585544231U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID GENERAL = 133642231U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSICBUS = 2886307548U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
