using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class Production
    {
        public WeaponTemplate techWeapon;

        public int totalProductionDay;
        public int leftDays;

        public List<Country> productionCountries = new List<Country>();

        public Production()
        {
            productionCountries.Clear();
            techWeapon = null;
            totalProductionDay = -1;

            leftDays = -1;
        }
    }
}
