using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using WorldMapStrategyKit;

public class WarManager : MonoBehaviour
{
    private static WarManager instance;
    public static WarManager Instance
    {
        get { return instance; }
    }

    WMSK map;

    public GameObject NuclearWarSprite;
    public GameObject NukeExplosion;

    void Start()
    {
        instance = this;
        map = WMSK.instance;
    }

    public void DeclareWar(Country attack, Country guard)
    {
        if (attack == null)
            return;

        if (guard == null)
            return;

        if (attack == guard)
            return;

        War war = new War();

        war.CreateWar(attack, guard);
        attack.AddWar(war);
        guard.AddWar(war);

        NotificationManager.Instance.CreatePublicNotification(attack.name + " has declared war to " + guard.name);
        NotificationManager.Instance.CreatePublicNotification(guard.name + " has declared war to " + attack.name);
    }

    public void AttackToEnemy(GameObjectAnimator attack, GameObjectAnimator guard )
    {
        if (attack != null && guard != null)
        {
            Division attackDivision = attack.GetDivision();
            List<Weapon> attackDivisionWeapon = attackDivision.GetWeaponsInDivision();
            int attackDivisionWeaponCount = attackDivisionWeapon.Count;

            if (guard.isBuilding())
            {
                DestroyUnit(guard);
            }

            if (guard.isDivision())
            {
                Division guardDivision = guard.GetDivision();
                List<Weapon> guardDivisionWeapon = guardDivision.GetWeaponsInDivision();
                int guardDivisionWeaponCount = guardDivisionWeapon.Count;

                if (attackDivisionWeaponCount > 0 || guardDivisionWeaponCount > 0)
                {
                    List<Weapon> willBeRemoved = new List<Weapon>();
                    willBeRemoved.Clear();

                    if (attack.terrainCapability == TERRAIN_CAPABILITY.OnlyGround || guard.terrainCapability == TERRAIN_CAPABILITY.OnlyGround)
                    {
                        Country battleFieldCountry = map.GetCountry(attack.currentMap2DLocation);
                        Province battleFieldProvince = map.GetProvince(attack.currentMap2DLocation);

                        if (battleFieldCountry == null)
                            battleFieldCountry = map.GetCountry(guard.currentMap2DLocation);

                        if (battleFieldProvince == null)
                            battleFieldProvince = map.GetProvince(guard.currentMap2DLocation);

                        if (battleFieldCountry != null && battleFieldProvince != null)
                        {
                            foreach (City city in map.GetCities(battleFieldCountry))
                            {
                                if (city.province == battleFieldProvince.name)
                                {
                                    city.population = city.population - ((city.population) / 5);
                                    CountryManager.Instance.RemoveBuildingsInCity(city, 10);
                                }
                            }
                        }
                    }

                    foreach (Weapon weapon in attackDivisionWeapon)
                    {
                        if (weapon != null)
                        {
                            attackDivision.onBattle = true;
                            guardDivision.onBattle = true;

                            if (guardDivisionWeaponCount > 0)
                            {
                                Weapon enemyWeapon = guardDivision.GetRandomWeaponsInDivision();
                                if (enemyWeapon == null || willBeRemoved.Contains(enemyWeapon))
                                {

                                }
                                else
                                {
                                    int attackHitChance = 0;
                                    int guardHitChance = 0;

                                    WeaponTemplate attackWeapon = WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID);
                                    WeaponTemplate guardWeapon = WeaponManager.Instance.GetWeaponTemplateByID(enemyWeapon.weaponTemplateID);


                                    if (guardWeapon.weaponTerrainType == 1)
                                    {
                                        attackHitChance = attackWeapon.landHitChance;
                                    }
                                    if (guardWeapon.weaponTerrainType == 2)
                                    {
                                        attackHitChance = attackWeapon.navalHitChance;
                                    }
                                    if (guardWeapon.weaponTerrainType == 3)
                                    {
                                        attackHitChance = attackWeapon.airHitChance;
                                    }


                                    if (attackWeapon.weaponTerrainType == 1)
                                    {
                                        guardHitChance = guardWeapon.landHitChance;
                                    }
                                    if (attackWeapon.weaponTerrainType == 2)
                                    {
                                        guardHitChance = guardWeapon.navalHitChance;
                                    }
                                    if (attackWeapon.weaponTerrainType == 3)
                                    {
                                        guardHitChance = guardWeapon.airHitChance;
                                    }

                                    int attackWeaponDamage = WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttack;
                                    int guardWeaponDamage = WeaponManager.Instance.GetWeaponTemplateByID(enemyWeapon.weaponTemplateID).weaponAttack;


                                    weapon.weaponLeftHealth -= ( ( guardWeaponDamage * guardHitChance ) / 100 );
                                    enemyWeapon.weaponLeftHealth -= ( ( attackWeaponDamage * attackHitChance ) / 100);

                                    if (enemyWeapon.weaponLeftHealth <= 0)
                                        willBeRemoved.Add(enemyWeapon);

                                    if (weapon.weaponLeftHealth <= 0)
                                        willBeRemoved.Add(weapon);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    foreach (Weapon removeWeapon in willBeRemoved)
                    {
                        if (attack == null || guard == null)
                        {

                        }
                        else
                        {
                            if (attackDivisionWeapon.Contains(removeWeapon))
                                attackDivision.DeleteWeaponInDivision(removeWeapon);

                            else if (guardDivisionWeapon.Contains(removeWeapon))
                                guardDivision.DeleteWeaponInDivision(removeWeapon);
                        }
                    }

                    attackDivision.onBattle = false;
                    guardDivision.onBattle = false;
                }
            }
            
        }            
    }

    public void UpdateDivisions(Country country)
    {
        foreach(GameObjectAnimator GOA in country.GetArmy().GetAllDivisionInArmy())
        {
            if(GOA != null)
            {
                if (GOA.GetDivision().GetWeaponsInDivision().Count <= 0)
                    DestroyUnit(GOA);
            }
        }
    }

    public void DestroyUnit(GameObjectAnimator GOA)
    {
        if (GOA == null)
            return;

        Country country = map.GetCountry(GOA.player);

        if (country != null)
        {
            if(GOA.isBuilding())
            {
                
            }

            if (GOA.isDivision())
            {
                foreach (GameObjectAnimator removeDivision in country.GetArmy().GetAllDivisionInArmy().ToArray())
                {
                    if (removeDivision == this)
                    {
                        if (country.GetArmy().GetLandForces().GetAllDivisionInMilitaryForces().Contains(GOA))
                        {
                            country.GetArmy().GetLandForces().RemoveDivisionInMilitaryForces(GOA);
                            break;
                        }

                        else if (country.GetArmy().GetAirForces().GetAllDivisionInMilitaryForces().Contains(GOA))
                        {
                            country.GetArmy().GetAirForces().RemoveDivisionInMilitaryForces(GOA);
                            break;
                        }

                        else if (country.GetArmy().GetNavalForces().GetAllDivisionInMilitaryForces().Contains(GOA))
                        {
                            country.GetArmy().GetNavalForces().RemoveDivisionInMilitaryForces(GOA);
                            break;
                        }
                    }
                }
            }
        }

        if (GameEventHandler.Instance.GetPlayer().GetMyCountry() == country)
            HUDManager.Instance.PrivateNotification("You lost " + GOA.name);

        GOA.enabled = false;
        DestroyImmediate(GOA.gameObject);
    }

    void CheckDistanceAndAttack(GameObjectAnimator division, GameObjectAnimator targetDivision)
    {
        if (division == null || targetDivision == null)
            return;

        int range = division.GetDivision().GetDivisionMaximumAttackRange();

        if (GetDistance(division.currentMap2DLocation, targetDivision.currentMap2DLocation) < range)
        {
            if(targetDivision.isBuilding())
            {
                if (division.terrainCapability != TERRAIN_CAPABILITY.Any)
                    division.Stop();

                AttackToEnemy(division, targetDivision);
            }

            if (targetDivision.isDivision())
            {
                //AudioManager.Instance.PlayVoice(VOICE_TYPE.ATTACK);
                if (division.GetDivision().onBattle == false && targetDivision.GetDivision().onBattle == false)
                {
                    if (division.terrainCapability != TERRAIN_CAPABILITY.Any)
                        division.Stop();

                    AttackToEnemy(division, targetDivision);
                }
            }
        }
    }
    
    public void MoveAndAttack(GameObjectAnimator GOA, GameObjectAnimator enemy, bool attack, Vector3 targetPos)
    {
        if (GOA == null)
            return;

        if (GOA.terrainCapability != TERRAIN_CAPABILITY.Any)
        {
            if (enemy == null)
            {
                if(GOA.isMoving == false)
                {
                    float speed = GOA.GetDivision().GetDivisionSpeed();

                    if (speed > 0)
                        GOA.MoveTo(targetPos, speed);
                    else
                        DestroyUnit(GOA);
                }
            }
            else
            {
                if (GOA.isMoving == false)
                {
                    float speed = GOA.GetDivision().GetDivisionSpeed();

                    if(speed > 0)
                        GOA.MoveTo(enemy.currentMap2DLocation, speed);
                    else
                        DestroyUnit(GOA);
                }
            }
        }
        else
        {
            if (GOA.isMoving == false)
            {
                GOA.arcMultiplier = 1.5f;     // tempCountry is the arc for the plane trajectory
                GOA.easeType = EASE_TYPE.SmootherStep;    // make it an easy-in-out movement

                GOA.comeBack = false;

                if (attack)
                    GOA.comeBack = true;

                GOA.altitudeEnd = 1.5f;
                GOA.altitudeStart = 0.1f;

                if (enemy == null)
                {
                    if (GOA.isMoving == false)
                    {
                        float speed = GOA.GetDivision().GetDivisionSpeed();

                        if (speed > 0)
                            GOA.MoveTo(targetPos, speed);
                        else
                            DestroyUnit(GOA);
                    }
                }
                else
                {
                    if (GOA.isMoving == false)
                    {
                        float speed = GOA.GetDivision().GetDivisionSpeed();

                        if (speed > 0)
                            GOA.MoveTo(enemy.currentMap2DLocation, speed);
                        else
                            DestroyUnit(GOA);
                    }
                }
            }
        }

        GOA.OnMove += (GameObjectAnimator anim) =>
        {
            CheckDistanceAndAttack(GOA, enemy);
        };

        GOA.OnProvinceEnter += (GameObjectAnimator anim) =>
        {
            if (GOA.terrainCapability != TERRAIN_CAPABILITY.OnlyGround)
                return;

            Country enterCountry = map.GetCountry(GOA.currentMap2DLocation);
            Country unitCountry = map.GetCountry(GOA.player);

            if (unitCountry == null || enterCountry == null)
                return;

            if (unitCountry != enterCountry)
            {
                if (CountryManager.Instance.GetAtWarCountryList(unitCountry).Contains(enterCountry)) // at war
                {
                    Province enterProvince = map.GetProvince(GOA.currentMap2DLocation);

                    if ( GetDivisionsOnProvince(enterCountry, enterProvince).Count == 0 )
                    {
                        foreach (City city in map.GetCities(enterCountry))
                        {
                            if(city != null && enterProvince != null)
                            {
                                if (city.province == enterProvince.name)
                                {
                                    if (city.cityClass == WorldMapStrategyKit.CITY_CLASS.COUNTRY_CAPITAL)
                                    {
                                        FinishWar(enterCountry, CountryManager.Instance.GetWarCountry(unitCountry, enterCountry));
                                        return;
                                    }
                                }
                            }
                        }
                        // conquer province
                        int targetCountryIndex = map.GetCountryIndex(unitCountry);

                        map.CountryTransferProvinceRegion(targetCountryIndex, enterProvince.mainRegion, false);

                        map.ToggleProvinceSurface(enterProvince.mainRegionIndex, true, unitCountry.SurfaceColor);
                    }
                }
                else
                {
                    int leftMilitaryAccess = 0;
                    unitCountry.GetMilitaryAccess().TryGetValue(enterCountry, out leftMilitaryAccess);

                    if (leftMilitaryAccess > 0)
                    {

                    }
                    else
                    {
                        // no military access
                    }
                }
            }
        };

        GOA.OnKilled += (GameObjectAnimator anim) =>
        {

        };

        GOA.OnMoveStart += (GameObjectAnimator anim) =>
        {
            if(GOA.terrainCapability == TERRAIN_CAPABILITY.Any)
            {
                if (GOA.comeBack == false && GOA.startingMap2DLocation != GOA.destination)
                {
                    GOA.altitudeEnd = 0.1f;
                    GOA.altitudeStart = 0.1f;
                }
            }
        };

        GOA.OnMoveEnd += (GameObjectAnimator anim) =>
        {
            if (GOA.terrainCapability == TERRAIN_CAPABILITY.Any)
            {
                if (GOA.startingMap2DLocation != GOA.destination && GOA.comeBack == true)
                {
                    GOA.comeBack = false;

                    GOA.altitudeEnd = 0.1f;
                    GOA.altitudeStart = 1.5f;

                    GOA.MoveTo(GOA.startingMap2DLocation, GOA.GetDivision().GetDivisionSpeed());

                    CheckDistanceAndAttack(GOA, enemy);

                    ShowExplosion(GOA.currentMap2DLocation);
                }
                else
                {
                    GOA.altitudeStart = 0.1f;
                    GOA.currentAltitude = 0.1f;
                    GOA.altitudeEnd = 0.1f;
                }
            }
            
        };    // once the movement has finished, stop following the unit
    }
   
    
    public IEnumerator War(Weapon weapon, Country attackCountry, Country defenseCountry, int wave)
    {
        float start = Time.time;
        while (Time.time - start < wave)
        {
            yield return null;
        }

        StartCoroutine(LaunchMissile(weapon, 2f, attackCountry.name, defenseCountry.name, Color.yellow));
        //StartCoroutine (LaunchMissile (3f, defenseCountry.name, attackCountry.name, Color.black));
    }


    IEnumerator LaunchMissile(Weapon weapon, float delay, string countryOrigin, string countryDest, Color color)
    {
        float start = Time.time;
        while (Time.time - start < delay)
        {
            yield return null;
        }

        // Initiates line animation
        int cityOrigin = map.GetCityIndexRandom(map.GetCountry(countryOrigin));
        int cityDest = map.GetCityIndexRandom(map.GetCountry(countryDest));
        if (cityOrigin < 0 || cityDest < 0)
            yield break;

        City targetCity = map.GetCity(cityDest);

        Vector2 origin = map.cities[cityOrigin].unity2DLocation;
        Vector2 dest = map.cities[cityDest].unity2DLocation;
        float elevation = 5f;
        float width = 0.1f;
        LineMarkerAnimator lma = map.AddLine(origin, dest, color, elevation, width);
        lma.dashInterval = 0.003f;
        lma.dashAnimationDuration = 0.5f;
        lma.drawingDuration = 4f;
        lma.autoFadeAfter = 1f;

        // Add flashing target
        GameObject sprite = Instantiate(NuclearWarSprite) as GameObject;
        sprite.GetComponent<SpriteRenderer>().material.color = color * 0.9f;
        map.AddMarker2DSprite(sprite, dest, 0.003f);
        MarkerBlinker.AddTo(sprite, 4, 0.1f, 0.5f, true);

        WeaponTemplate weaponTemplate = WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID);
        int damage = weaponTemplate.weaponAttack / 100;

        if (weaponTemplate.weaponType == WEAPON_TYPE.ICBM)
        {
            targetCity.population = targetCity.population - (targetCity.population * damage)/100;
            CountryManager.Instance.RemoveBuildingsInCity(targetCity, damage);
        }
        if (weaponTemplate.weaponType == WEAPON_TYPE.NEUTRON_BOMB)
        {
            targetCity.population = targetCity.population - (targetCity.population * damage) / 100;
        }
        if (weaponTemplate.weaponType == WEAPON_TYPE.ANTI_MATTER_BOMB)
        {
            targetCity.population = 0;
            CountryManager.Instance.RemoveBuildingsInCity(targetCity, damage);
        }

        weapon = null;

        // Triggers explosion
        StartCoroutine(AddCircleExplosion(4f, dest, Color.yellow));
    }


    public void BeginMissileWar(Country attackCountry, Country defenseCountry)
    {
        Weapon[] missile = attackCountry.GetArmy().GetMissile().ToArray();

        if(missile.Length > 0)
            StartCoroutine(War(missile[0], attackCountry, defenseCountry, 1));
    }

    IEnumerator AddCircleExplosion(float delay, Vector2 mapPos, Color color)
    {
        float start = Time.time;
        while (Time.time - start < delay)
        {
            yield return null;
        }

        ShowExplosion(mapPos);
    }

    public void ShowExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(NukeExplosion);
        explosion.transform.localScale = Misc.Vector3one * 0.1f;

        GameObjectAnimator anim = explosion.WMSK_MoveTo(position);
        anim.autoScale = true;
        anim.gameObject.hideFlags = HideFlags.HideInHierarchy; // don't show in hierarchy to avoid clutter 

        AudioManager.Instance.PlayVoice(VOICE_TYPE.BOMB);
        Destroy(anim.gameObject, 3f);
    }

    public void FinishWar(Country country, War war)
    {
        Country winnerCountry = war.GetEnemyCountry(country);

        if (country.Budget > 0)
            winnerCountry.Budget += country.Budget;

        if (country.Defense_Budget > 0)
            winnerCountry.Budget += country.Defense_Budget;


        foreach (Weapon weapon in country.GetArmy().GetLandForces().GetAllWeaponsInMilitaryForces())
        {
            winnerCountry.GetArmy().GetLandForces().AddWeaponToMilitaryForces(weapon);
        }
        foreach (Weapon weapon in country.GetArmy().GetAirForces().GetAllWeaponsInMilitaryForces())
        {
            winnerCountry.GetArmy().GetAirForces().AddWeaponToMilitaryForces(weapon);
        }
        foreach (Weapon weapon in country.GetArmy().GetNavalForces().GetAllWeaponsInMilitaryForces())
        {
            winnerCountry.GetArmy().GetNavalForces().AddWeaponToMilitaryForces(weapon);
        }

        foreach(GameObjectAnimator GOA in country.GetArmy().GetAllDivisionInArmy().ToArray())
        {
            DestroyUnit(GOA);
        }

        map.CountryTransferCountry(map.GetCountryIndex(winnerCountry), map.GetCountryIndex(country), true);

        DestroyUnit(map.GetCity(country.capitalCityIndex).Capital_Building);

        MapManager.Instance.ReColorizeAllCountries();

        winnerCountry.GetWarList().Remove(war);
        country.GetWarList().Remove(war);
    }

    public void WarStrategy(Country country)
    {
        List<War> warList = country.GetWarList();

        if (warList.Count > 0)
        {
            UpdateDivisions(country);

            if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
            {
                if (GameEventHandler.Instance.GetPlayer().LeadWar == true)
                    return;                 
            }

            foreach (War war in warList.ToArray())
            {
                Country enemy = war.GetEnemyCountry(country);

                BeginMissileWar(country, enemy);

                if (country.GetArmy().GetAllDivisionInArmy().Count <= 0)
                {
                    FinishWar(country, war);
                    return;
                }
                if (enemy.GetArmy().GetAllDivisionInArmy().Count <= 0)
                {
                    FinishWar(enemy, war);
                    return;
                }

                TotalWar(country, enemy, TERRAIN_CAPABILITY.OnlyGround);
                TotalWar(country, enemy, TERRAIN_CAPABILITY.Any);
                TotalWar(country, enemy, TERRAIN_CAPABILITY.OnlyWater);
            }
        } 
    }

    public void TotalWar(Country attack, Country guard, TERRAIN_CAPABILITY unitType)
    {
        GameObjectAnimator[] allDivisions = null;

        if (unitType == TERRAIN_CAPABILITY.OnlyGround)
            allDivisions = attack.GetArmy().GetLandForces().GetAllDivisionInMilitaryForces().ToArray();
        if (unitType == TERRAIN_CAPABILITY.OnlyWater)
            allDivisions = attack.GetArmy().GetNavalForces().GetAllDivisionInMilitaryForces().ToArray();
        if (unitType == TERRAIN_CAPABILITY.Any)
            allDivisions = attack.GetArmy().GetAirForces().GetAllDivisionInMilitaryForces().ToArray();

        foreach (GameObjectAnimator GOA in allDivisions)
        {
            if (GOA != null)         
            {
                GameObjectAnimator enemyDivision = FindNearestDivision(GOA, guard, false);

                if (enemyDivision != null)
                {
                    MoveAndAttack(GOA, enemyDivision, true, Vector3.zero);
                }
                else
                {
                    if(GOA.terrainCapability == TERRAIN_CAPABILITY.OnlyWater)
                    {
                        GameObjectAnimator dockyard = FindNearestDockyard(GOA, guard);

                        if(dockyard != null)
                        {
                            MoveAndAttack(GOA, dockyard, true, Vector3.zero);
                        }
                    }
                    GOA.Stop();
                }
            }
        }
    }

    public int GetDistance(Vector3 sourcePos, Vector3 targetPos)
    {
        int distance = Convert.ToInt32(map.calc.Distance(sourcePos, targetPos) / 1000);
        return distance;
    }

    public GameObjectAnimator FindNearestDockyard(GameObjectAnimator unit, Country country)
    {
        if (unit == null)
            return null;

        int distance = 100000000;
        GameObjectAnimator GOA = null;

        foreach(City city in CountryManager.Instance.GetAllCitiesInCountry(country))
        {
            GameObjectAnimator dockyard = city.Dockyard;
            if (dockyard != null)
            {
                int tempDistance = GetDistance(unit.currentMap2DLocation, dockyard.currentMap2DLocation);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    GOA = dockyard;
                }

            }
        }

        return GOA;
    }

    public GameObjectAnimator FindNearestDivision(GameObjectAnimator unit, Country targetCountry, bool sameTerrain)
    {
        if (unit == null)
            return null;

        int distance = 100000000;
        GameObjectAnimator GOA = null;

        GameObjectAnimator[] allDivisions = null;

        if (sameTerrain)
        {
            if(unit.terrainCapability == TERRAIN_CAPABILITY.OnlyGround)
            {
                allDivisions = targetCountry.GetArmy().GetLandForces().GetAllDivisionInMilitaryForces().ToArray();
            }
            if(unit.terrainCapability == TERRAIN_CAPABILITY.OnlyWater)
            {
                allDivisions = targetCountry.GetArmy().GetNavalForces().GetAllDivisionInMilitaryForces().ToArray();
            }
            if(unit.terrainCapability == TERRAIN_CAPABILITY.Any)
            {
                allDivisions = targetCountry.GetArmy().GetAirForces().GetAllDivisionInMilitaryForces().ToArray();
            }
        }
        else
        {
            allDivisions = targetCountry.GetArmy().GetAllDivisionInArmy().ToArray();
        }

        foreach (GameObjectAnimator division in allDivisions)
        {
            if (division != null && division.GetDivision().GetWeaponsInDivision().Count > 0 && division.GetDivision().onBattle == false)
            {
                int tempDistance = GetDistance(unit.currentMap2DLocation, division.currentMap2DLocation);
                if (tempDistance <= distance)
                {
                    distance = tempDistance;
                    GOA = division;
                }

            }
        }

        return GOA;
    }

    
    public List<GameObjectAnimator> GetDivisionsOnProvince(Country country, Province province)
    {
        GameObjectAnimator[] allDivisions = country.GetArmy().GetAllDivisionInArmy().ToArray();

        List<GameObjectAnimator> allDivisionsOnProvince = new List<GameObjectAnimator>();

        foreach (GameObjectAnimator GOA in allDivisions)
        {
            if (GOA != null)
            {
                Province tempProvince = map.GetProvince(GOA.currentMap2DLocation);
                if (tempProvince != null)
                {
                    if (tempProvince == province)
                    {
                        allDivisionsOnProvince.Add(GOA);
                    }
                }
            }
        }

        return allDivisionsOnProvince;
    }

    
    public void AttackUnitsOnProvinces(Country country, Country enemy, List<Province> myProvinces)
    {
        foreach (Province province in myProvinces)
        {
            List<GameObjectAnimator> divisionList = GetDivisionsOnProvince(country, province);

            foreach(GameObjectAnimator GOA in divisionList)
            {
                GameObjectAnimator enemyDivision = FindNearestDivision(GOA, enemy, true);

                if (GOA != null && enemyDivision != null)
                {
                    MoveAndAttack(GOA, enemyDivision, true, Vector3.zero);
                }
            }
        }
    }
    

}
