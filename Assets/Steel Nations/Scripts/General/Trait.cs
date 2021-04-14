using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trait
{
    int traitID;
    string traitName;
    string traitDescription;
    int minValue;
    int maxValue;

    TRAIT traitType;

    public TRAIT Trait_Type
    {
        get { return traitType; }
        set { traitType = value; }
    }

    public int Trait_ID
    {
        get { return traitID; }
        set { traitID = value; }
    }
    public string Trait_Name
    {
        get { return traitName; }
        set { traitName = value; }
    }
    public string Trait_Description
    {
        get { return traitDescription; }
        set { traitDescription = value; }
    }
    public int Min_Value
    {
        get { return minValue; }
        set { minValue = value; }
    }
    public int Max_Value
    {
        get { return maxValue; }
        set { maxValue = value; }
    }
}
