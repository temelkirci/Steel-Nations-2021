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
        List<DivisionTemplate> divisionTemplateList = new List<DivisionTemplate>();
        public GameObject shield;
        List<DIVISION_TYPE> divisionTypeList;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            map = WMSK.instance;

            divisionTypeList = Enum.GetValues(typeof(DIVISION_TYPE)).Cast<DIVISION_TYPE>().ToList();
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
            if (country.GetArmy().GetAirForces() == null)
                return false;
            if (country.GetArmy().GetNavalForces() == null)
                return false;

            return true;
        }

        public void CreateDivisions(Country country)
        {
            CreateLandDivision(country, DIVISION_TYPE.ARMORED_DIVISION);
            CreateLandDivision(country, DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION);
            CreateLandDivision(country, DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION);

            CreateAirDivision(country, DIVISION_TYPE.AIR_DIVISION);

            CreateNavalDivision(country, DIVISION_TYPE.CARRIER_DIVISION);
            CreateNavalDivision(country, DIVISION_TYPE.SUBMARINE_DIVISION);
            CreateNavalDivision(country, DIVISION_TYPE.DESTROYER_DIVISION);
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

        public DivisionTemplate GetDivisionTemplateByType(DIVISION_TYPE tempDivisionType)
        {
            foreach (DivisionTemplate divisionTemplate in GetDivisionTemplate())
            {
                if (divisionTemplate.divisionType == tempDivisionType)
                {
                    return divisionTemplate;
                }
            }

            return null;
        }

        public void CreateLandDivision(Country country, DIVISION_TYPE divisionType)
        {
            if (IsPossibleToCreateDivision(country) == false)
                return;

            bool IsMyDivision = false;
            if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                IsMyDivision = true;

            MilitaryForces militaryForces = country.GetArmy().GetLandForces();

            if (militaryForces == null)
                return;

            DivisionTemplate divisionTemplate = GetDivisionTemplateByType(divisionType);

            if (divisionTemplate == null)
                return;

            int countryIndex = map.GetCountryIndex(country);

            int i = 0;

            while(true)
            {
                if (CountryManager.Instance.GetEmptyGarrisonNumberInCountry(country) == 0)
                    return;

                Division division = CreateDivision(militaryForces, divisionTemplate);

                if (division != null)
                {
                    City city = CountryManager.Instance.GetRandomEmptyGarrisonInCountry(country);

                    Vector2 cityPosition = city.unity2DLocation;
                    GameObjectAnimator tank = CreateMilitaryUnit(TERRAIN_CAPABILITY.OnlyGround, cityPosition, divisionType, IsMyDivision);

                    tank.SetDivision(division);
                    militaryForces.AddDivisionToMilitaryForces(tank);

                    tank.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter
                    tank.autoRotation = true;
                    tank.player = countryIndex;

                    tank.name = GetDivisionName(i, divisionType);
                    division.divisionName = tank.name;

                    city.AddDivisionToGarrison(tank);
                    i++;
                }
                else
                {
                    break;
                }
            }
        }


        public void CreateAirDivision(Country country, DIVISION_TYPE divisionType)
        {
            if (IsPossibleToCreateDivision(country) == false)
                return;

            bool IsMyDivision = false;
            if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                IsMyDivision = true;

            MilitaryForces militaryForces = country.GetArmy().GetAirForces();

            if (militaryForces == null)
                return;

            DivisionTemplate divisionTemplate = GetDivisionTemplateByType(divisionType);

            if(divisionTemplate == null)
                return;

            int countryIndex = map.GetCountryIndex(country);

            int i = 0;

            while(true)
            {
                if (CountryManager.Instance.GetEmptyGarrisonNumberInCountry(country) == 0)
                    return;

                Division division = CreateDivision(militaryForces, divisionTemplate);

                if (division != null)
                {
                    City city = CountryManager.Instance.GetRandomEmptyGarrisonInCountry(country);

                    Vector2 cityPosition = city.unity2DLocation;
                    GameObjectAnimator airPlane = CreateMilitaryUnit(TERRAIN_CAPABILITY.Any, cityPosition, divisionType, IsMyDivision);
                    
                    airPlane.SetDivision(division);
                    militaryForces.AddDivisionToMilitaryForces(airPlane);
                    airPlane.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter  
                    airPlane.autoRotation = true;
                    airPlane.player = countryIndex;

                    airPlane.name = GetDivisionName(i, divisionType);
                    division.divisionName = airPlane.name;

                    city.AddDivisionToGarrison(airPlane);
                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        public void CreateNavalDivision(Country country, DIVISION_TYPE divisionType)
        {
            if (IsPossibleToCreateDivision(country) == false)
                return;

            bool IsMyDivision = false;
            if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
                IsMyDivision = true;

            MilitaryForces militaryForces = country.GetArmy().GetNavalForces();

            if (militaryForces == null)
                return;

            DivisionTemplate navalDivision = GetDivisionTemplateByType(divisionType);

            if (navalDivision == null)
                return;

            int countryIndex = map.GetCountryIndex(country);
            List<Vector2> city = map.GetCountryCoastalPoints(countryIndex);

            int i = 0;

            while(true)
            {
                Division division = CreateDivision(militaryForces, navalDivision);

                if (division != null)
                {
                    int index = UnityEngine.Random.Range(0, city.Count);
                    Vector2 cityPosition = city[index];
                    GameObjectAnimator ship = CreateMilitaryUnit(TERRAIN_CAPABILITY.OnlyWater, cityPosition, divisionType, IsMyDivision);

                    ship.SetDivision(division);
                    militaryForces.AddDivisionToMilitaryForces(ship);

                    ship.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter  
                    ship.autoRotation = true;
                    ship.player = countryIndex;

                    ship.name = GetDivisionName(i, divisionType);
                    division.divisionName = ship.name;

                    i++;
                }
                else
                {
                    break;
                }
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

                mapPosition.x -= 0.0005f;
                mapPosition.y += 0.0005f;

                militaryGOA = unitGO.WMSK_MoveTo(mapPosition);
                militaryGOA.autoRotation = true;
                militaryGOA.terrainCapability = terrainCapability;
                militaryGOA.enableBuoyancyEffect = false;

            }
            else if (terrainCapability == TERRAIN_CAPABILITY.Any)
            {
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



        public Division CreateDivision(MilitaryForces militaryForces, DivisionTemplate divisionTemplate)
        {
            List<Weapon> mainUnitList = new List<Weapon>();
            List<Weapon> secondUnitList = new List<Weapon>();
            List<Weapon> thirdUnitList = new List<Weapon>();

            foreach (Weapon weapon in militaryForces.GetAllWeaponsInMilitaryForces())
            {
                WEAPON_TYPE weaponType = WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponType;

                if (weaponType == divisionTemplate.mainWeaponType)
                {
                    mainUnitList.Add(weapon);
                }
                if (weaponType == divisionTemplate.secondWeaponType)
                {
                    secondUnitList.Add(weapon);
                }
                if (weaponType == divisionTemplate.thirdWeaponType)
                {
                    thirdUnitList.Add(weapon);
                }
            }


            if ( mainUnitList.Count >= divisionTemplate.mainUnitMinimum )
            {              
                Division division = new Division();
                division.divisionTemplate = divisionTemplate;

                division.MainWeapon = mainUnitList[0].weaponTemplateID;

                if(secondUnitList.Count == 0)
                    division.SecondWeapon = WeaponManager.Instance.GetLowestGenerationWeaponIDByWeaponType(divisionTemplate.secondWeaponType);
                else
                    division.SecondWeapon = secondUnitList[0].weaponTemplateID;

                if (thirdUnitList.Count == 0)
                    division.ThirdWeapon = WeaponManager.Instance.GetLowestGenerationWeaponIDByWeaponType(divisionTemplate.thirdWeaponType);
                else
                    division.ThirdWeapon = thirdUnitList[0].weaponTemplateID;





                if (mainUnitList.Count > divisionTemplate.mainUnitMaximum)
                    division.MainWeaponNumber = divisionTemplate.mainUnitMaximum;
                else
                    division.MainWeaponNumber = mainUnitList.Count;


                if (secondUnitList.Count > divisionTemplate.secondUnitMaximum)
                    division.SecondWeaponNumber = divisionTemplate.secondUnitMaximum;
                else
                    division.SecondWeaponNumber = secondUnitList.Count;


                if (thirdUnitList.Count > divisionTemplate.thirdUnitMaximum)
                    division.ThirdWeaponNumber = divisionTemplate.thirdUnitMaximum;
                else
                    division.ThirdWeaponNumber = thirdUnitList.Count;


                for (int i = 0; i < division.MainWeaponNumber; i++)
                {
                    division.AddWeaponToDivision(mainUnitList[i]);
                    militaryForces.RemoveWeaponFromMilitaryForces(mainUnitList[i]);
                }

                for (int i = 0; i < division.SecondWeaponNumber; i++)
                {
                    division.AddWeaponToDivision(secondUnitList[i]);
                    militaryForces.RemoveWeaponFromMilitaryForces(secondUnitList[i]);
                }

                for (int i = 0; i < division.ThirdWeaponNumber; i++)
                {
                    division.AddWeaponToDivision(thirdUnitList[i]);
                    militaryForces.RemoveWeaponFromMilitaryForces(thirdUnitList[i]);
                }

                return division;
            }
            else
            {
                return null;
            }
        }

        public void CreateDivisionTemplate(string mainUnit,
            string secondUnit,
            string thirdUnit,
            int minMainUnit,
            int maxMainUnit,
            int minSecondUnit,
            int maxSecondUnit,
            int minThirdUnit,
            int maxThirdUnit,
            int soldierMinimum,
            int soldierMaximum,
            string divisionName)
        {
            WEAPON_TYPE mainWeaponType = WeaponManager.Instance.GetWeaponType(mainUnit);
            WEAPON_TYPE secondWeaponType = WeaponManager.Instance.GetWeaponType(secondUnit);
            WEAPON_TYPE thirdWeaponType = WeaponManager.Instance.GetWeaponType(thirdUnit);

            DivisionTemplate divisionTemplate = new DivisionTemplate();
            divisionTemplate.divisionType = GetDivisionTypeByDivisionName(divisionName);

            divisionTemplate.SetDivisionMainWeaponByWeaponName(mainWeaponType, minMainUnit, maxMainUnit);
            divisionTemplate.SetDivisionSecondWeaponByWeaponName(secondWeaponType, minSecondUnit, maxSecondUnit);
            divisionTemplate.SetDivisionThirdWeaponByWeaponName(thirdWeaponType, minThirdUnit, maxThirdUnit);

            divisionTemplate.minimumSoldier = soldierMinimum;
            divisionTemplate.maximumSoldier = soldierMaximum;

            AddDivisionTemplate(divisionTemplate);
        }

        public DIVISION_TYPE GetDivisionTypeByDivisionName(string tempDivisionName)
        {
            if (tempDivisionName == "Armored Infantry Division")
            {
                return DIVISION_TYPE.ARMORED_DIVISION;
            }
            if (tempDivisionName == "Mechanized Infantry Division")
            {
                return DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION;
            }
            if (tempDivisionName == "Motorized Infantry Division")
            {
                return DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION;
            }

            if (tempDivisionName == "Air Division")
            {
                return DIVISION_TYPE.AIR_DIVISION;
            }

            if (tempDivisionName == "Submarine Division")
            {
                return DIVISION_TYPE.SUBMARINE_DIVISION;
            }
            if (tempDivisionName == "Destroyer Division")
            {
                return DIVISION_TYPE.DESTROYER_DIVISION;
            }
            if (tempDivisionName == "Carrier Division")
            {
                return DIVISION_TYPE.CARRIER_DIVISION;
            }

            return DIVISION_TYPE.NONE;
        }
    }
}