public class Policy
{
    public int policyID;
    public string policyName;
    public string policyDescription;
    public string policyBonus;
    public int policyBonusValue;
    public int requiredDefenseBudget;
    public int requiredGSYH;
    public int tradeBonus;
    public int costPermenant;
    public POLICY_TYPE policyType;

    public Policy()
    {
        policyID = -1;
        policyName = string.Empty;
        policyDescription = string.Empty;
        policyBonus = string.Empty;
        policyBonusValue = -1;
        requiredDefenseBudget = -1;
        requiredGSYH = -1;
        tradeBonus = -1;
        costPermenant = -1;
        policyType = POLICY_TYPE.NONE;
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
