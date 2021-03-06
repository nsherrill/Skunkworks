﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public enum SportType
    {
        Baseball,
    }

    public enum PlayerDataType
    {
        Hitting,
        Pitching,
        StartingPitcher,
        Unknown,
        HotHitting,
    }

    public enum WeatherType
    {
        Clear,
        Cloudy,
        Drizzle,
        Indoors,
        Overcast,
        PartlyCloudy,
        PartlySunny,
        Rain,
        Sunny,
        Unknown
    }

    public enum RefereeType
    {
        HomePlate,
        FirstBase,
        SecondBase,
        ThirdBase,
    }

    public enum LeagueType
    {
        Auction,
        HeadToHead
    }

    public enum BaseballPosition
    {
        pos_P,
        pos_C,
        pos_1B,
        pos_2B,
        pos_3B,
        pos_SS,
        pos_OF,
        pos_DH,
    }

    public enum PlayerAvailability
    {
        a_Out,
        a_DayToDay,
        a_Available,
        a_Suspension,
        a_7DayDL,
        a_15DayDL,
        a_60DayDL,
    }

    public enum ConfigType
    {
       // TopAvailablePPG,
        //TopAvailablePPG_PitcherFirst,
       // TopAvailablePPGPerValue,
       // TopAvailableValue,
       // TopAvailableValue_HomeOnly,
        TopAvailablePointsPerABLast7_PPG,  //5/29
        //TopAvailableOBPLast7_PPG, // 5/16
    }

    public enum SourceType
    {
        ESPN,
        SportingCharts
    }
}
