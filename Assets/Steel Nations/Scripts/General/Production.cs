using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class Production
    {
        public WeaponTemplate techWeapon;

        public int leftDays;
        bool completed;

        public int number;
        public List<Country> productionCountries = new List<Country>();

        public bool IsCompleted()
        {
            return completed;
        }

        public void UpdateProduction()
        {
            if(leftDays > 0)
            {
                leftDays--;
            }

            if (leftDays == 0)
                completed = true;

        }
        public Production()
        {
            productionCountries.Clear();
            techWeapon = null;
            completed = false;

            leftDays = -1;
            number = 0;
        }
    }
}
