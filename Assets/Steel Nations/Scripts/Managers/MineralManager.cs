using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralManager : Singleton<MineralManager>
{
    public string GetMineralNameByType(MINERAL_TYPE mineralType)
    {
        if (mineralType == MINERAL_TYPE.IRON)
            return "Iron";
        if (mineralType == MINERAL_TYPE.OIL)
            return "Oil";
        if (mineralType == MINERAL_TYPE.ALUMINIUM)
            return "Aluminium";
        if (mineralType == MINERAL_TYPE.URANIUM)
            return "Uranium";
        if (mineralType == MINERAL_TYPE.STEEL)
            return "Steel";
        return string.Empty;
    }


    public MINERAL_TYPE GetMineralTypeByMineralName(string mineralName)
    {
        if (mineralName == "Iron")
            return MINERAL_TYPE.IRON;
        if (mineralName == "Oil")
            return MINERAL_TYPE.OIL;
        if (mineralName == "Aluminium")
            return MINERAL_TYPE.ALUMINIUM;
        if (mineralName == "Uranium")
            return MINERAL_TYPE.URANIUM;
        if (mineralName == "Steel")
            return MINERAL_TYPE.STEEL;
        return MINERAL_TYPE.NONE;
    }

    public int GetMineralCost(string mineralName)
    {
        if (mineralName == "Iron")
            return 3;
        if (mineralName == "Oil")
            return 5;
        if (mineralName == "Aluminium")
            return 1;
        if (mineralName == "Uranium")
            return 8;
        if (mineralName == "Steel")
            return 4;
        return 0;
    }

    public int GetMineralCost(MINERAL_TYPE mineralType)
    {
        if (mineralType == MINERAL_TYPE.OIL)
            return 5;
        if (mineralType == MINERAL_TYPE.IRON)
            return 3;
        if (mineralType == MINERAL_TYPE.STEEL)
            return 4;
        if (mineralType == MINERAL_TYPE.ALUMINIUM)
            return 1;
        if (mineralType == MINERAL_TYPE.URANIUM)
            return 8;

        return 0;
    }


}
