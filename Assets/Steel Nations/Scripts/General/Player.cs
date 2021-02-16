using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class Player
    {
        List<GameObjectAnimator> selectedUnits;
        Country MyCountry = null;

        GameObjectAnimator selectedDivision;
        GameObjectAnimator selectedBuilding;
        GameObjectAnimator mouseOverUnit;
        City selectedCity;
        Country selectedCountry;

        public Player()
        {
            selectedUnits = new List<GameObjectAnimator>();
        }

        public Country GetMyCountry()
        {
            return MyCountry;
        }
        public void SetMyCountry(Country country)
        {
            MyCountry = country;
        }

        public GameObjectAnimator GetSelectedDivision()
        {
            return selectedDivision;
        }
        public void SelectDivision(GameObjectAnimator GOA)
        {
            selectedDivision = GOA;
        }


        public GameObjectAnimator GetMouseOverUnit()
        {
            return mouseOverUnit;
        }
        public void SetMouseOverUnit(GameObjectAnimator GOA)
        {
            mouseOverUnit = GOA;
        }


        public List<GameObjectAnimator> GetSelectedUnits()
        {
            return selectedUnits;
        }
        public void SetSelectedUnits(List<GameObjectAnimator> GOA)
        {
            selectedUnits = GOA;
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

        public City GetSelectedCity()
        {
            return selectedCity;
        }
        public void SetSelectedCity(City city)
        {
            selectedCity = city;
        }


    }
}