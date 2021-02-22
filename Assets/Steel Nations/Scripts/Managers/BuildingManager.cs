using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class BuildingManager : MonoBehaviour
    {
        private static BuildingManager instance;
        public static BuildingManager Instance
        {
            get { return instance; }
        }

        WMSK map;

        public GameObject CapitalBuilding;
        public GameObject HarborBuilding;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;
        }

        public void CreateBuildings()
        {
            foreach (Country tempCountry in CountryManager.Instance.GetAllCountries())
            {
                int harborPopulation = 0;

                if (tempCountry.GetArmy() != null)
                {
                    if (tempCountry.GetArmy().GetDefenseBudget() >= 500000)
                        harborPopulation = 100000;
                    if (tempCountry.GetArmy().GetDefenseBudget() < 500000 && tempCountry.GetArmy().GetDefenseBudget() >= 100000)
                        harborPopulation = 250000;
                    if (tempCountry.GetArmy().GetDefenseBudget() < 100000 && tempCountry.GetArmy().GetDefenseBudget() >= 20000)
                        harborPopulation = 500000;
                    if (tempCountry.GetArmy().GetDefenseBudget() < 20000 && tempCountry.GetArmy().GetDefenseBudget() >= 1000)
                        harborPopulation = 750000;
                    if (tempCountry.GetArmy().GetDefenseBudget() < 1000)
                        harborPopulation = 1000000;

                    foreach (City city in tempCountry.GetAllCitiesInCountry())
                    {
                        if (city.GetConstructibleDockyardAreaNumber() > 0 && city.GetPopulation() > harborPopulation)
                        {
                            //tempCountry.totalDockyard = tempCountry.totalDockyard + 1;
                            city.SetCurrentBuildingNumberInCity(BUILDING_TYPE.DOCKYARD, 1);

                            AddBuilding(city, MY_UNIT_TYPE.DOCKYARD);
                        }

                        if (city.cityClass == CITY_CLASS.COUNTRY_CAPITAL)
                            AddBuilding(city, MY_UNIT_TYPE.COUNTRY_CAPITAL_BUILDING);

                        //if (city.GetCurrentBuildingNumberInCity(BUILDING_TYPE.AIRPORT) > 0)
                        //AddSpriteAtPosition(city);
                    }
                }
            }
        }

        public void AddBuilding(City city, MY_UNIT_TYPE unitType)
        {
            GameObject go = null;

            if (MY_UNIT_TYPE.DOCKYARD == unitType)
            {
                go = Instantiate(HarborBuilding);

                go.transform.localScale = Misc.Vector3one * 0.0005f;

                GameObjectAnimator anim = go.WMSK_MoveTo(city.unity2DLocation);
                anim.name = city.name + " Dockyard";
                anim.type = (int)unitType;
                anim.pivotY = 0.5f;

                anim.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter 

                city.SetDockyard(anim);
            }
            else if (unitType == MY_UNIT_TYPE.COUNTRY_CAPITAL_BUILDING)
            {
                go = Instantiate(CapitalBuilding);

                go.transform.localScale = Misc.Vector3one * 0.005f;

                GameObjectAnimator anim = go.WMSK_MoveTo(city.unity2DLocation);
                anim.name = city.name + " Capital";
                anim.type = (int)unitType;
                anim.pivotY = 0.5f;

                anim.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter 
            }
        }
    }
}
