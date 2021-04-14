using System.Collections.Generic;

public class Policy
{
    public int policyID;
    public string policyName;
    public string policyDescription;
    public int requiredDefenseBudget;
    public int requiredGSYH;
    public int costPermenant;
    public POLICY_TYPE policyType;

    Dictionary<TRAIT, float> traits = new Dictionary<TRAIT, float>();

    public Policy()
    {
        policyID = -1;
        policyName = string.Empty;
        policyDescription = string.Empty;
        requiredDefenseBudget = 0;
        requiredGSYH = 0;
        costPermenant = 0;
        
        policyType = POLICY_TYPE.NONE;
    }

    public float GetValue(TRAIT traitEnum)
    {
        foreach(var trait in traits)
        {
            if (trait.Key == traitEnum)
                return trait.Value;
        }

        return 0;
    }

    public void AddTrait(TRAIT trait, float value)
    {
        traits.Add(trait, value);
    }

    public Dictionary<TRAIT, float> GetTraits()
    {
        return traits;
    }

    public void SetPolicyTypeByName(string policyTypeName)
    {
        if (policyTypeName == "ECONOMY")
            policyType = POLICY_TYPE.ECONOMY;
        if (policyTypeName == "MILITARY")
            policyType = POLICY_TYPE.MILITARY;
        if (policyTypeName == "POPULATION")
            policyType = POLICY_TYPE.POPULATION;
        if (policyTypeName == "CONSTRUCTION")
            policyType = POLICY_TYPE.CONSTRUCTION;
        if (policyTypeName == "HEALTH")
            policyType = POLICY_TYPE.HEALTH;
        if (policyTypeName == "EDUCATION")
            policyType = POLICY_TYPE.EDUCATION;
        if (policyTypeName == "RELIGION")
            policyType = POLICY_TYPE.RELIGION;
        if (policyTypeName == "DIPLOMACY")
            policyType = POLICY_TYPE.DIPLOMACY;
    }
}
