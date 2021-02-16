using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelligenceAgency
{
    string intelligenceAgencyName;
    int intelligenceAgencyLevel;
    int intelligenceAgencyBudget;

    public void SetIntelligenceAgencyName(string name)
    {
        intelligenceAgencyName = name;
    }
    public string GetIntelligenceAgencyName()
    {
        return intelligenceAgencyName;
    }

    public void SetIntelligenceAgencyLevel(int level)
    {
        intelligenceAgencyLevel = level;
    }
    public int GetIntelligenceAgencyLevel()
    {
        return intelligenceAgencyLevel;
    }

    public void SetIntelligenceAgencyBudget(int budget)
    {
        intelligenceAgencyBudget = budget;
    }
    public int GetIntelligenceAgencyBudget()
    {
        return intelligenceAgencyBudget;
    }


    public int GetAssassinationOfPresident()
    {
        return intelligenceAgencyLevel * 5; 
    }
    public int GetStoleTechnology()
    {
        return intelligenceAgencyLevel * 8;
    }
}
