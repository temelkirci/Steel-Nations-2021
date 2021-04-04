using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace WorldMapStrategyKit
{
    public class DivisionManager : MonoBehaviour
    {
        private static DivisionManager instance;
        public static DivisionManager Instance
        {
            get { return instance; }
        }

        WMSK map;

        public GameObject tankPrefab, destroyerPrefab, fighterPrefab, armoredVehiclePrefab, artilleryPrefab, carrierPrefab, submarinePrefab;
        List<DivisionTemplate> divisionTemplateList;
        public GameObject shield;
        List<DIVISION_TYPE> divisionTypeList;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;

            divisionTypeList = Enum.GetValues(typeof(DIVISION_TYPE)).Cast<DIVISION_TYPE>().ToList();
            divisionTemplateList = new List<DivisionTemplate>();
        }

        public void Init()
        {

        }

        public void AddDivisionTemplate(DivisionTemplate divisionTemplate)
        {
            divisionTemplateList.Add(divisionTemplate);
        }
        public List<DivisionTemplate> GetDivisionTemplate()
        {
            return divisionTemplateList;
        }

        public void DivisionTransferToCountry(Country sourceCountry, Country targetCountry, Division division)
        {
            GameObjectAnimator GOA = sourceCountry.GetArmy().GetDivisionGOA(division);

            if (GetMilitaryForcesByDivisionType(GOA.GetDivision().divisionTemplate.divisionType) == MILITARY_FORCES_TYPE.LAND_FORCES)
                targetCountry.GetArmy().GetLandForces().AddDivisionToMilitaryForces(GOA);

            if (GetMilitaryForcesByDivisionType(GOA.GetDivision().divisionTemplate.divisionType) == MILITARY_FORCES_TYPE.AIR_FORCES)
                targetCountry.GetArmy().GetAirForces().AddDivisionToMilitaryForces(GOA);

            if (GetMilitaryForcesByDivisionType(GOA.GetDivision().divisionTemplate.divisionType) == MILITARY_FORCES_TYPE.NAVAL_FORCES)
                targetCountry.GetArmy().GetNavalForces().AddDivisionToMilitaryForces(GOA);
        }

        public MILITARY_FORCES_TYPE GetMilitaryForcesByDivisionType(DIVISION_TYPE divisionType)
        {
            if (divisionType == DIVISION_TYPE.ARMORED_DIVISION ||
                divisionType == DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION ||
                divisionType == DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION)
                return MILITARY_FORCES_TYPE.LAND_FORCES;

            if (divisionType == DIVISION_TYPE.AIR_DIVISION)
                return MILITARY_FORCES_TYPE.AIR_FORCES;

            if (divisionType == DIVISION_TYPE.CARRIER_DIVISION ||
                divisionType == DIVISION_TYPE.DESTROYER_DIVISION ||
                divisionType == DIVISION_TYPE.SUBMARINE_DIVISION)
                return MILITARY_FORCES_TYPE.NAVAL_FORCES;

            return MILITARY_FORCES_TYPE.NONE;
        }

        public bool IsPossibleToCreateDivision(Country country)
        {
            if (country == null)
                return false;

            if (country.GetArmy() == null)
                return false;

            if (country.GetArmy().GetLandForces() == null)
                return false;

            return true;
        }

        public void CreateDivisions(Country country)
        {
            foreach (DIVISION_TYPE divisionType in divisionTypeList)
            {
                if (GetMilitaryForcesByDivisionType(divisionType) == MILITARY_FORCES_TYPE.LAND_FORCES)
                    CreateLandDivision(country, -1, divisionType);

                if (GetMilitaryForcesByDivisionType(divisionType) == MILITARY_FORCES_TYPE.AIR_FORCES)
                    CreateAirDivision(country, -1, divisionType);

                if (GetMilitaryForcesByDivisionType(divisionType) == MILITARY_FORCES_TYPE.NAVAL_FORCES)
                    CreateNavalDivision(country, -1, divisionType);
            }
        }

        public void RandomPositionDivisionsInCountry(Country sourceCountry, Country targetCountry)
        {
            foreach(GameObjectAnimator division in sourceCountry.GetArmy().GetAllDivisionInArmy())
            {
                if(division.enterCountry == targetCountry)
                {
                    division.MoveTo(map.GetCityRandom(sourceCountry).unity2DLocation, 0.1f);
                    division.enterCountry = sourceCountry;
                }
            }
        }

        public string GetDivisionName(int index, DIVISION_TYPE divisionType)
        {
            string divisionName = string.Empty;

            index = index + 1;

            if (divisionType == DIVISION_TYPE.ARMORED_DIVISION)
                divisionName = index + "." + "Armored Division";
            if (divisionType == DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION)
                divisionName = index + "." + "Mechanized Infantry Division";
            if (divisionType == DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION)
                divisionName = index + "." + "Motorized Infantry Division";

            if(divisionType == DIVISION_TYPE.AIR_DIVISION)
                divisionName = index + "." + "Aviation";

            if (divisionType == DIVISION_TYPE.SUBMARINE_DIVISION)
                divisionName = index + "." + "Submarine Fleet";
            if (divisionType == DIVISION_TYPE.CARRIER_DIVISION)
                divisionName = index + "." + "Aircraft Fleet";
            if (divisionType == DIVISION_TYPE.DESTROYER_DIVISION)
                divisionName = index + "." + "Destroyer Fleet";

            return divisionName;
        }

        public void CreateLandDivision(Country tempCountry, int number, DIVISION_TYPE divisionType)
        {
            if (IsPossibleToCreateDivision(tempCountry) == false)
                return;

            bool IsMyDivision = false;
            if (tempCountry == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                IsMyDivision = true;

            int countryIndex = map.GetCountryIndex(tempCountry);

            List<City> normalCity = CountryManager.Instance.GetSortedCityListByPopulation(tempCountry);
            DivisionTemplate tempDivisionTemplate = DivisionManagerPanel.GetDivisionTemplateByType(divisionType);
            int landDivisionNumber = tempCountry.GetArmy().GetLandForces().PossibleDivisionNumber(tempDivisionTemplate);

            if (number == -1)
            {
                number = landDivisionNumber;
            }
            else
            {
                if (number > landDivisionNumber)
                    number = landDivisionNumber;
            }

            if (number > normalCity.Count)
                number = normalCity.Count;

            for (int i = 0; i < number; i++)
            {
                Vector2 cityPosition = normalCity[i].unity2DLocation;
                GameObjectAnimator tank = CreateMilitaryUnit(TERRAIN_CAPABILITY.OnlyGround, cityPosition, divisionType, IsMyDivision);
                tank.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter
                tank.autoRotation = true;
                tank.player = countryIndex;

                tank.name = GetDivisionName(i, tempDivisionTemplate.divisionType);

                CreateDivision(tempCountry.GetArmy().GetLandForces(), tank, tempDivisionTemplate);
                //normalCity[i].AddDivisionToCity(tank);
            }
        }


        public void CreateAirDivision(Country tempCountry, int number, DIVISION_TYPE divisionType)
        {
            if (IsPossibleToCreateDivision(tempCountry) == false)
                return;

            bool IsMyDivision = false;
            if (tempCountry == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                IsMyDivision = true;

            int countryIndex = map.GetCountryIndex(tempCountry);

            List<City> normalCity = CountryManager.Instance.GetSortedCityListByPopulation(tempCountry);
            DivisionTemplate airDivision = DivisionManagerPanel.GetDivisionTemplateByType(divisionType);
            int airDivisionNumber = tempCountry.GetArmy().GetAirForces().PossibleDivisionNumber(airDivision);

            if (number == -1)
            {
                number = airDivisionNumber;
            }
            else
            {
                if (number > airDivisionNumber)
                    number = airDivisionNumber;
            }

            if (number > normalCity.Count)
                number = normalCity.Count;

            for (int i = 0; i < number; i++)
            {
                Vector2 cityPosition = normalCity[i].unity2DLocation;
                GameObjectAnimator airPlane = CreateMilitaryUnit(TERRAIN_CAPABILITY.Any, cityPosition, divisionType, IsMyDivision);

                airPlane.name = GetDivisionName(i, airDivision.divisionType);

                airPlane.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter  
                airPlane.autoRotation = true;
                airPlane.player = countryIndex;

                CreateDivision(tempCountry.GetArmy().GetAirForces(), airPlane, airDivision);
                //normalCity[i].AddDivisionToCity(airPlane);
            }
        }

        public void CreateNavalDivision(Country tempCountry, int number, DIVISION_TYPE divisionType)
        {
            if (IsPossibleToCreateDivision(tempCountry) == false)
                return;

            bool IsMyDivision = false;
            if (tempCountry == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                IsMyDivision = true;

            int countryIndex = map.GetCountryIndex(tempCountry);

            List<Vector2> normalCity = map.GetCountryCoastalPoints(map.GetCountryIndex(tempCountry));
            DivisionTemplate navalDivision = DivisionManagerPanel.GetDivisionTemplateByType(divisionType);
            int navalDivisionNumber = tempCountry.GetArmy().GetNavalForces().PossibleDivisionNumber(navalDivision);

            if (number == -1)
            {
                number = navalDivisionNumber;
            }
            else
            {
                if (number > navalDivisionNumber)
                    number = navalDivisionNumber;
            }

            if (number > normalCity.Count)
                number = normalCity.Count;

            for (int i = 0; i < number; i++)
            {
                Vector2 cityPosition = normalCity[i];
                GameObjectAnimator ship = CreateMilitaryUnit(TERRAIN_CAPABILITY.OnlyWater, cityPosition, divisionType, IsMyDivision);
                ship.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter  
                ship.autoRotation = true;
                ship.player = countryIndex;

                ship.name = GetDivisionName(i, navalDivision.divisionType);

                CreateDivision(tempCountry.GetArmy().GetNavalForces(), ship, navalDivision);
            }
        }

        public GameObjectAnimator CreateMilitaryUnit(TERRAIN_CAPABILITY terrainCapability, Vector2 mapPosition, DIVISION_TYPE divisionType, bool addAOE)
        {
            GameObject unitGO = null;
            GameObjectAnimator militaryGOA = null;

            if (terrainCapability == TERRAIN_CAPABILITY.OnlyGround)
            {
                if (divisionType == DIVISION_TYPE.ARMORED_DIVISION)
                {
                    unitGO = Instantiate(tankPrefab);
                }
                if (divisionType == DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION)
                {
                    unitGO = Instantiate(armoredVehiclePrefab);
                }
                if (divisionType == DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION)
                {
                    unitGO = Instantiate(artilleryPrefab);
                }

                unitGO.transform.localScale = Misc.Vector3one * 0.025f;

                mapPosition.x = mapPosition.x - 0.0005f;
                mapPosition.y = mapPosition.y + 0.0005f;

                militaryGOA = unitGO.WMSK_MoveTo(mapPosition);
                militaryGOA.autoRotation = true;
                militaryGOA.terrainCapability = terrainCapability;
                militaryGOA.enableBuoyancyEffect = false;

            }
            else if (terrainCapability == TERRAIN_CAPABILITY.Any)
            {
                mapPosition.x = mapPosition.x + 0.0005f;
                mapPosition.y = mapPosition.y - 0.0005f;

                // Create Airplane
                unitGO = Instantiate(fighterPrefab);
                unitGO.transform.localScale = Misc.Vector3one * 0.01f;
                militaryGOA = unitGO.WMSK_MoveTo(mapPosition);
                //militaryGOA.type = (int)MY_UNIT_TYPE.AIRPLANE;              // this is completely optional, just used in the demo scene to differentiate this unit from other tanks and ships
                militaryGOA.terrainCapability = terrainCapability;  // ignores path-finding and can use a straight-line from start to destination
                militaryGOA.pivotY = 0.5f; // model is not ground based (which has a pivoty = 0, the default value, so setting the pivot to 0.5 will center vertically the model)
                militaryGOA.autoRotation = true;  // auto-head to destination when moving
                militaryGOA.rotationSpeed = 0.1f;  // speed of the rotation of auto-head to destination
                militaryGOA.enableBuoyancyEffect = false;
            }
            else if (terrainCapability == TERRAIN_CAPABILITY.OnlyWater)
            {
                if (divisionType == DIVISION_TYPE.CARRIER_DIVISION)
                    unitGO = Instantiate(carrierPrefab);
                if (divisionType == DIVISION_TYPE.DESTROYER_DIVISION)
                    unitGO = Instantiate(destroyerPrefab);
                if (divisionType == DIVISION_TYPE.SUBMARINE_DIVISION)
                    unitGO = Instantiate(submarinePrefab);

                // Create ship
                if(divisionType != DIVISION_TYPE.CARRIER_DIVISION)
                    unitGO.transform.localScale = Misc.Vector3one * 0.005f;

                militaryGOA = unitGO.WMSK_MoveTo(mapPosition);
                militaryGOA.pivotY = 0.5f; // model is not ground based (which has a pivoty = 0, the default value, so setting the pivot to 0.5 will center vertically the model)
                militaryGOA.terrainCapability = terrainCapability;
                militaryGOA.autoRotation = true;
                militaryGOA.enableBuoyancyEffect = true;
            }


            if (addAOE)
            {
                GameObject circle = map.AddCircle(militaryGOA.currentMap2DLocation, 50, 0.8f, 0.9f, new Color(255, 255, 0, 0.33f));

                // Hook event OnMove to sync circle position and destroy it when ship no longer exists
                militaryGOA.OnMove += (tank) => circle.transform.localPosition = new Vector3(tank.currentMap2DLocation.x, tank.currentMap2DLocation.y, 0);

                // Show/hide also with ship
                militaryGOA.OnVisibleChange += (tank) => circle.SetActive(tank.isVisibleInViewport);

                // Optionally hook OnKilled - so we don't have to remember to remove the circle when ship is destroyed
                militaryGOA.OnKilled += (tank) => Destroy(circle);
            }

            return militaryGOA;
        }



        public void CreateDivision(MilitaryForces militaryForces, GameObjectAnimator tempDivision, DivisionTemplate divisionTemplate)
        {
            tempDivision.CreateDivisionGameObject();
            tempDivision.GetDivision().divisionTemplate = divisionTemplate;
            tempDivision.GetDivision().divisionName = tempDivision.name;

            List<Weapon> mainUnitList = new List<Weapon>();
            foreach (int weaponID in divisionTemplate.mainUnitIDList)
            {
                List<Weapon> weaponList = militaryForces.GetWeaponListInMilitaryForcesInventory(weaponID);
                if (weaponList.Count > 0)
                {
                    mainUnitList.AddRange(weaponList);
                    tempDivision.GetDivision().MainWeapon = weaponID;
                    tempDivision.GetDivision().MainWeaponNumber = weaponList.Count;

                    break;
                }
            }

            List<Weapon> secondUnitList = new List<Weapon>();
            foreach (int weaponID in divisionTemplate.secondUnitList)
            {
                List<Weapon> weaponList = militaryForces.GetWeaponListInMilitaryForcesInventory(weaponID);
                if (weaponList.Count > 0)
                {
                    secondUnitList.AddRange(weaponList);
                    tempDivision.GetDivision().SecondWeapon = weaponID;
                    tempDivision.GetDivision().SecondWeaponNumber = weaponList.Count;

                    break;
                }
            }

            List<Weapon> thirdUnitList = new List<Weapon>();
            foreach (int weaponID in divisionTemplate.thirdUnitList)
            {
                List<Weapon> weaponList = militaryForces.GetWeaponListInMilitaryForcesInventory(weaponID);
                if (weaponList.Count > 0)
                {
                    thirdUnitList.AddRange(weaponList);
                    tempDivision.GetDivision().ThirdWeapon = weaponID;
                    tempDivision.GetDivision().ThirdWeaponNumber = weaponList.Count;
                    break;
                }
            }

            if (tempDivision.GetDivision().MainWeaponNumber >= divisionTemplate.mainUnitMaximum)
            {
                for (int i = 0; i < tempDivision.GetDivision().MainWeaponNumber; i++)
                {
                    tempDivision.GetDivision().AddWeaponToDivision(mainUnitList[i]);
                    militaryForces.RemoveWeaponFromMilitaryForces(mainUnitList[i]);
                }

                /*
                if (secondUnitList.Count < divisionTemplate.secondUnitMaximum)
                {
                    secondUnitNumber = secondUnitList.Count;
                }
                if (thirdUnitList.Count < divisionTemplate.thirdUnitMaximum)
                {
                    thirdUnitNumber = thirdUnitList.Count;
                }
                */

                for (int i = 0; i < tempDivision.GetDivision().SecondWeaponNumber; i++)
                {
                    tempDivision.GetDivision().AddWeaponToDivision(secondUnitList[i]);
                    militaryForces.RemoveWeaponFromMilitaryForces(secondUnitList[i]);
                }

                for (int i = 0; i < tempDivision.GetDivision().ThirdWeaponNumber; i++)
                {
                    tempDivision.GetDivision().AddWeaponToDivision(thirdUnitList[i]);
                    militaryForces.RemoveWeaponFromMilitaryForces(thirdUnitList[i]);
                }

                militaryForces.AddDivisionToMilitaryForces(tempDivision);
            }
            else
            {
                //Debug.Log(divisionTemplate.mainUnit + " -> " + divisionTemplate.mainUnitMaximum);
            }
        }


        public void AddDivisionToCity(City city, GameObjectAnimator division)
        {
            if (city.GetAllDivisionsInCity().Count < city.GetBuildingNumber(BUILDING_TYPE.GARRISON))
            {
                division.visible = false;
                city.GetAllDivisionsInCity().Add(division);
            }
            else
            {
                // you have reached maximum garrison in city
            }
        }
    }
}