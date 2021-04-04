using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class Research
    {
        public WeaponTemplate techWeapon;
        bool completed;

        public int leftDays;
        public int totalResearchDay;
        public float percent;

        public List<Country> researchCountries = new List<Country>();
        bool isResearching;

        public bool IsCompleted()
        {
            return completed;
        }

        public void UpdateResearch()
        {
            if (leftDays > 0)
            {
                leftDays--;
            }

            if (leftDays == 0)
            {
                completed = true;
                isResearching = false;
            }
            else
            {
                isResearching = true;
            }
            percent = 100f - (leftDays * 100f) / totalResearchDay;
        }

        public bool IsResearching()
        {
            return isResearching;
        }

        public float GetProgress()
        {
            return percent;
        }
        public Research()
        {
            researchCountries.Clear();
            techWeapon = null;
            completed = false;
            leftDays = -1;
            totalResearchDay = -1;
            isResearching = false;
            percent = 0;
        }
    }
}
