using System;
using System.Collections.Generic;
using UnityEngine;

public class DivisionTemplate
{
    public DIVISION_TYPE divisionType;

    public int minimumSoldier;
    public int maximumSoldier;

    public List<int> mainUnitIDList = new List<int>();
    public int mainUnitMaximum;

    public List<int> secondUnitList = new List<int>();
    public int secondUnitMaximum;

    public List<int> thirdUnitList = new List<int>();
    public int thirdUnitMaximum;

    public void SetDivisionMainWeaponByWeaponName(string[] tempWeaponID, int maxUnit)
    {
        foreach (string ID in tempWeaponID)
        {
            if(ID != string.Empty)
                mainUnitIDList.Add(Int32.Parse(ID));
        }
        mainUnitMaximum = maxUnit;
    }
    public void SetDivisionSecondWeaponByWeaponName(string[] tempWeaponID, int maxUnit)
    {
        foreach (string ID in tempWeaponID)
        {
            if (ID != string.Empty)
                secondUnitList.Add(Int32.Parse(ID));
        }
        secondUnitMaximum = maxUnit;
    }
    public void SetDivisionThirdWeaponByWeaponName(string[] tempWeaponID, int maxUnit)
    {
        foreach (string ID in tempWeaponID)
        {
            if (ID != string.Empty)
                thirdUnitList.Add(Int32.Parse(ID));
        }
        thirdUnitMaximum = maxUnit;
    }

    public void SetDivisionTypeByDivisionName(string tempDivisionName)
    {
        if (tempDivisionName == "Armored Infantry Division")
        {
            divisionType = DIVISION_TYPE.ARMORED_DIVISION;
        }
        if (tempDivisionName == "Mechanized Infantry Division")
        {
            divisionType = DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION;
        }
        if (tempDivisionName == "Motorized Infantry Division")
        {
            divisionType = DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION;
        }
        if (tempDivisionName == "Air Division")
        {
            divisionType = DIVISION_TYPE.AIR_DIVISION;
        }
        if (tempDivisionName == "Bomber Aviation Division")
        {
            divisionType = DIVISION_TYPE.BOMBER_DIVISION;
        }
        if (tempDivisionName == "Submarine Division")
        {
            divisionType = DIVISION_TYPE.SUBMARINE_DIVISION;
        }
        if (tempDivisionName == "Destroyer Division")
        {
            divisionType = DIVISION_TYPE.DESTROYER_DIVISION;
        }
        if (tempDivisionName == "Carrier Division")
        {
            divisionType = DIVISION_TYPE.CARRIER_DIVISION;
        }
    }
}
