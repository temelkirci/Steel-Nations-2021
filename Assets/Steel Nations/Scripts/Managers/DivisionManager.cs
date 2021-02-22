using System.Collections.Generic;
using UnityEngine;

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

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            // Get a reference to the World Map API:
            map = WMSK.instance;
        }

        public void AddDivisionTemplate(DivisionTemplate divisionTemplate)
        {
            divisionTemplateList.Add(divisionTemplate);
        }
        public List<DivisionTemplate> GetDivisionTemplate()
        {
            return divisionTemplateList;
        }

        public void CreateDivisions(Country country)
        {
            CreateLandDivision(country, -1, DIVISION_TYPE.ARMORED_DIVISION);
            CreateLandDivision(country, -1, DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION);
            CreateLandDivision(country, -1, DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION);

            CreateAirDivision(country, -1, DIVISION_TYPE.AIR_DIVISION);

            CreateNavalDivision(country, -1, DIVISION_TYPE.CARRIER_DIVISION);
            CreateNavalDivision(country, -1, DIVISION_TYPE.SUBMARINE_DIVISION);
            CreateNavalDivision(country, -1, DIVISION_TYPE.DESTROYER_DIVISION);
        }

        public void CreateLandDivision(Country tempCountry, int number, DIVISION_TYPE divisionType)
        {
            if (tempCountry == null)
                return;

            if (tempCountry.GetArmy() == null)
                return;

            if (tempCountry.GetArmy().GetLandForces() == null)
                return;

            List<City> normalCity = tempCountry.GetSortedCityListByPopulation();
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
                Vector2 regionCapitalCityPosition = normalCity[i].unity2DLocation;
                GameObjectAnimator tank = CreateMilitaryUnit(TERRAIN_CAPABILITY.OnlyGround, regionCapitalCityPosition, divisionType, true);
                tank.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter
                tank.attrib["Color"] = tank.GetComponentInChildren<Renderer>().sharedMaterial.color;
                tank.autoRotation = true;

                if (divisionType == DIVISION_TYPE.ARMORED_DIVISION)
                    tank.name = (i+1) + "." + tempCountry.name + " Armored Division";
                if (divisionType == DIVISION_TYPE.MECHANIZED_INFANTRY_DIVISION)
                    tank.name = (i+1) + "." + tempCountry.name + " Mechanized Infantry Division";
                if (divisionType == DIVISION_TYPE.MOTORIZED_INFANTRY_DIVISION)
                    tank.name = (i+1) + "." + tempCountry.name + " Motorized Infantry Division";

                tempCountry.GetArmy().GetLandForces().CreateDivision(tank, tempDivisionTemplate);
                normalCity[i].AddDivisionToCity(tank);
            }
        }


        public void CreateAirDivision(Country tempCountry, int number, DIVISION_TYPE divisionType)
        {
            List<City> normalCity = tempCountry.GetSortedCityListByPopulation();
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
                GameObjectAnimator airPlane = CreateMilitaryUnit(TERRAIN_CAPABILITY.Any, cityPosition, divisionType, true);
                airPlane.name = (i+1) + "." + tempCountry.name + " Aviation";
                airPlane.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter  
                airPlane.attrib["Color"] = airPlane.GetComponentInChildren<Renderer>().sharedMaterial.color;
                airPlane.autoRotation = true;

                tempCountry.GetArmy().GetAirForces().CreateDivision(airPlane, airDivision);
                normalCity[i].AddDivisionToCity(airPlane);
            }
        }

        public void CreateNavalDivision(Country tempCountry, int number, DIVISION_TYPE divisionType)
        {
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
                GameObjectAnimator ship = CreateMilitaryUnit(TERRAIN_CAPABILITY.OnlyWater, cityPosition, divisionType, true);
                ship.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter  
                ship.attrib["Color"] = ship.GetComponentInChildren<Renderer>().sharedMaterial.color;
                ship.autoRotation = true;

                if (divisionType == DIVISION_TYPE.SUBMARINE_DIVISION)
                    ship.name = (i+1) + "." + tempCountry.name + " Submarine Fleet";
                if (divisionType == DIVISION_TYPE.CARRIER_DIVISION)
                    ship.name = (i+1) + "." + tempCountry.name + " Aircraft Fleet";
                if (divisionType == DIVISION_TYPE.DESTROYER_DIVISION)
                    ship.name = (i+1) + "." + tempCountry.name + " Destroyer Fleet";

                tempCountry.GetArmy().GetNavalForces().CreateDivision(ship, navalDivision);
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
                militaryGOA.type = (int)MY_UNIT_TYPE.TANK;
                militaryGOA.autoRotation = true;
                militaryGOA.terrainCapability = terrainCapability;
            }
            else if (terrainCapability == TERRAIN_CAPABILITY.Any)
            {
                mapPosition.x = mapPosition.x + 0.0005f;
                mapPosition.y = mapPosition.y - 0.0005f;

                // Create Airplane
                unitGO = Instantiate(fighterPrefab);
                unitGO.transform.localScale = Misc.Vector3one * 0.01f;
                militaryGOA = unitGO.WMSK_MoveTo(mapPosition);
                militaryGOA.type = (int)MY_UNIT_TYPE.AIRPLANE;              // this is completely optional, just used in the demo scene to differentiate this unit from other tanks and ships
                militaryGOA.terrainCapability = terrainCapability;  // ignores path-finding and can use a straight-line from start to destination
                militaryGOA.pivotY = 0.5f; // model is not ground based (which has a pivoty = 0, the default value, so setting the pivot to 0.5 will center vertically the model)
                militaryGOA.autoRotation = true;  // auto-head to destination when moving
                militaryGOA.rotationSpeed = 0.1f;  // speed of the rotation of auto-head to destination
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
                unitGO.transform.localScale = Misc.Vector3one * 0.005f;
                militaryGOA = unitGO.WMSK_MoveTo(mapPosition);
                militaryGOA.type = (int)MY_UNIT_TYPE.SHIP;
                militaryGOA.pivotY = 0.5f; // model is not ground based (which has a pivoty = 0, the default value, so setting the pivot to 0.5 will center vertically the model)
                militaryGOA.terrainCapability = terrainCapability;
                militaryGOA.autoRotation = true;
            }

            // Add circle of area of effect
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
    }
}