using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class Research
    {
        public WeaponTemplate techWeapon;

        public int totalResearchDay;
        public int leftDays;

        public List<Country> researchCountries = new List<Country>();

        public Research()
        {
            researchCountries.Clear();
            techWeapon = null;
            totalResearchDay = -1;

            leftDays = -1;
        }
    }
}
