using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class Player
    {
        List<GameObjectAnimator> selectedDivisions;
        Country MyCountry = null;

        GameObjectAnimator selectedBuilding;
        GameObjectAnimator mouseOverUnit;
        City selectedCity;
        City mouseOverCity;
        Province selectedProvince;
        bool leadWar;
        Country selectedCountry;

        public Player()
        {
            selectedDivisions = new List<GameObjectAnimator>();
            leadWar = false;
        }

        public bool LeadWar
        {
            get { return leadWar; }
            set { leadWar = value; }
        }

        public Country GetMyCountry()
        {
            return MyCountry;
        }
        public void SetMyCountry(Country country)
        {
            MyCountry = country;
        }

        public GameObjectAnimator GetMouseOverUnit()
        {
            return mouseOverUnit;
        }
        public void SetMouseOverUnit(GameObjectAnimator GOA)
        {
            mouseOverUnit = GOA;
        }


        public List<GameObjectAnimator> GetSelectedDivisions()
        {
            return selectedDivisions;
        }
        public void AddSelectedDivisions(GameObjectAnimator GOA)
        {
            selectedDivisions.Add(GOA);
        }
        public void ClearSelectedDivisions()
        {
            selectedDivisions.Clear();
        }

        public int GetSelectedDivisionNumber()
        {
            return selectedDivisions.Count;
        }
        public bool IsMyDivision(GameObjectAnimator GOA)
        {
            foreach (GameObjectAnimator division in MyCountry.GetArmy().GetAllDivisionInArmy())
                if (GOA == division)
                    return true;
            return false;
        }
        public GameObjectAnimator GetSelectedBuilding()
        {
            return selectedBuilding;
        }
        public void SetSelectedBuilding(GameObjectAnimator GOA)
        {
            selectedBuilding = GOA;
        }


        public Country GetSelectedCountry()
        {
            return selectedCountry;
        }
        public void SetSelectedCountry(Country country)
        {
            selectedCountry = country;
        }

        public void SetSelectedProvince(Province province)
        {
            selectedProvince = province;
        }
        public Province GetSelectedProvince()
        {
            return selectedProvince;
        }

        public City GetSelectedCity()
        {
            return selectedCity;
        }

        public void SetSelectedCity(City city)
        {
            selectedCity = city;
        }
        public City GetMouseOverCity()
        {
            return mouseOverCity;
        }
        public void SetMouseOverCity(City city)
        {
            mouseOverCity = city;
        }
    }
}