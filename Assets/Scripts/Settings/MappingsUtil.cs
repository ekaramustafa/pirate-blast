using System;
using System.Collections.Generic;
using UnityEngine;

public static class MappingUtils
{
    public static readonly Dictionary<string, Func<UnitType>> stringToUnitTypeMapping = new Dictionary<string, Func<UnitType>>
    {
        { "r", () => UnitType.Block },
        { "g", () => UnitType.Block },
        { "b", () => UnitType.Block },
        { "y", () => UnitType.Block },
        { "t", () => UnitType.TNT },
        { "bo", () => UnitType.Ice },
        { "s", () => UnitType.Stone },
        { "v", () => UnitType.Chocolate },
        { "rand", () => GetRandomUnitType()  }
    };
    private static UnitType GetRandomUnitType()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return UnitType.Random;
        }
#endif
        return UnitType.Block;
    }

    public static readonly Dictionary<UnitType, string> unitTypeToStringToMapping = new Dictionary<UnitType, string>
    {
        { UnitType.TNT, "t"},
        { UnitType.Ice , "bo"},
        { UnitType.Stone, "s"},
        { UnitType.Chocolate , "v" },
        { UnitType.Random, "rand"}
    };

    public static readonly Dictionary<string, Func<BlockColor>> stringToBlockColorMapping = new Dictionary<string, Func<BlockColor>>
    {
        { "r", () => BlockColor.Red },
        { "g", () => BlockColor.Green },
        { "b", () => BlockColor.Blue },
        { "y", () => BlockColor.Yellow },
        { "rand", () => BlockColorExtensions.GetRandomBlockColor() }
    };

    public static readonly Dictionary<BlockColor, string> blockColorToStringMapping = new Dictionary<BlockColor, string>
    {
        { BlockColor.Red , "r"},
        { BlockColor.Green, "g"},
        { BlockColor.Blue, "b"},
        { BlockColor.Yellow, "y"}
    };

}
