
public class IntelligenceAgency
{
    string intelligenceAgencyName;
    int intelligenceAgencyBudget;

    int militaryCoup;
    int reverseEnginering;
    int assassination;

    public void Init()
    {
        
    }

    public string IntelligenceAgencyName
    {
        get { return intelligenceAgencyName; }
        set { intelligenceAgencyName = value; }
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
