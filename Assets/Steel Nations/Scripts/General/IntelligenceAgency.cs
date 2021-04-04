
public class IntelligenceAgency
{
    string intelligenceAgencyName;
    int intelligenceAgencyLevel;
    int intelligenceAgencyBudget;

    int militaryCoup;
    int reverseEnginering;
    int assassination;

    public void Init()
    {
        reverseEnginering = intelligenceAgencyLevel * intelligenceAgencyLevel;
        assassination = intelligenceAgencyLevel * intelligenceAgencyLevel;
        militaryCoup = (intelligenceAgencyLevel * intelligenceAgencyLevel) / 2;
    }

    public string IntelligenceAgencyName
    {
        get { return intelligenceAgencyName; }
        set { intelligenceAgencyName = value; }
    }

    public int IntelligenceAgencyLevel
    {
        get { return intelligenceAgencyLevel; }
        set { intelligenceAgencyLevel = value; }
    }

    public int IntelligenceAgencyBudget
    {
        get { return intelligenceAgencyBudget; }
        set { intelligenceAgencyBudget = value; }
    }

    public int Assassination
    {
        get { return assassination; }
        set { assassination = value; }
    }

    public int ReverseEnginering
    {
        get { return reverseEnginering; }
        set { reverseEnginering = value; }
    }

    public int MilitaryCoup
    {
        get { return militaryCoup; }
        set { militaryCoup = value; }
    }
}
