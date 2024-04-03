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
        static const AkUniqueID PLAY_LEFT_CLICK = 2830116340U;
        static const AkUniqueID PLAY_MENU_MUSIC = 2228153899U;
        static const AkUniqueID PLAY_MOUSE_HOVERING = 3807302544U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace DAY_NIGHT
        {
            static const AkUniqueID GROUP = 2666809638U;

            namespace STATE
            {
                static const AkUniqueID DAY = 311764537U;
                static const AkUniqueID MID = 1182670505U;
                static const AkUniqueID NIGHT = 1011622525U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace DAY_NIGHT

        namespace DRONESTATE
        {
            static const AkUniqueID GROUP = 3733145084U;

            namespace STATE
            {
                static const AkUniqueID ACTIVE = 58138747U;
                static const AkUniqueID IDLE = 1874288895U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace DRONESTATE

        namespace GAMESTATE
        {
            static const AkUniqueID GROUP = 4091656514U;

            namespace STATE
            {
                static const AkUniqueID GAME_ON = 2219001485U;
                static const AkUniqueID GAME_PAUSE = 2772308904U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace GAMESTATE

        namespace WATER
        {
            static const AkUniqueID GROUP = 2654748154U;

            namespace STATE
            {
                static const AkUniqueID ABOVE = 2432428U;
                static const AkUniqueID BELOW = 384800980U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace WATER

    } // namespace STATES

    namespace SWITCHES
    {
        namespace ISLANDHEART_XP
        {
            static const AkUniqueID GROUP = 1673590565U;

            namespace SWITCH
            {
                static const AkUniqueID LEVEL_UP = 2145291615U;
                static const AkUniqueID UNLEVELED = 1548389549U;
            } // namespace SWITCH
        } // namespace ISLANDHEART_XP

        namespace ITEM_TYPE
        {
            static const AkUniqueID GROUP = 1281648863U;

            namespace SWITCH
            {
                static const AkUniqueID BERRY = 302590163U;
                static const AkUniqueID JELLY_DEW = 333239846U;
                static const AkUniqueID PORTABLE_GENERATOR = 1759730654U;
            } // namespace SWITCH
        } // namespace ITEM_TYPE

        namespace MATERIAL
        {
            static const AkUniqueID GROUP = 3865314626U;

            namespace SWITCH
            {
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID WATER = 2654748154U;
            } // namespace SWITCH
        } // namespace MATERIAL

        namespace MENU_TYPE
        {
            static const AkUniqueID GROUP = 1819922903U;

            namespace SWITCH
            {
                static const AkUniqueID INVENTORY_MENU = 3090328911U;
                static const AkUniqueID MAIN_MENU = 2005704188U;
                static const AkUniqueID PAUSE_MENU = 3422541661U;
            } // namespace SWITCH
        } // namespace MENU_TYPE

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID GAME_MUSIC_VOLUME = 1850957680U;
        static const AkUniqueID ISLAND_HEART_XP = 2803209766U;
        static const AkUniqueID MASTER_VOLUME = 4179668880U;
        static const AkUniqueID SFX_VOLUME = 1564184899U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID GENERAL = 133642231U;
        static const AkUniqueID MAIN_MENU = 2005704188U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID ENV = 529726550U;
        static const AkUniqueID GAME_MUSIC = 258110631U;
        static const AkUniqueID JELLIES = 2607341427U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PAUSE_MENU_MUSIC = 2095980153U;
        static const AkUniqueID PLAYER = 1069431850U;
        static const AkUniqueID SFX = 393239870U;
        static const AkUniqueID UI = 1551306167U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
