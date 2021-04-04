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
    public GameObject tankBullet;

    void Start()
    {
        instance = this;
        map = WMSK.instance;
    }

    public void DeclareWar(Country attack, Country guard)
    {
        if (attack == null)
            Debug.Log("attack == null");

        if(guard == null)
            Debug.Log("guard == null");

        if(attack == null || guard == null)
        {

        }
        else
        {
            War war = new War();

            if (war == null)
                Debug.Log("war == null");
            else
            {
                war.CreateWar(attack, guard);
                attack.AddWar(war);
                guard.AddWar(war);

                UpdateDivisions(attack);
                UpdateDivisions(guard);
                GovernmentPanel.Instance.HidePanel();
            }
        }
    }

    public void AttackToEnemy(GameObjectAnimator attack, GameObjectAnimator guard )
    {
        if (attack == null || guard == null)
        {

        }
        else
        {
            Division attackDivision = attack.GetDivision();
            List<Weapon> attackDivisionWeapon = attackDivision.GetWeaponsInDivision();
            int attackDivisionWeaponCount = attackDivisionWeapon.Count;

            Division guardDivision = guard.GetDivision();
            List<Weapon> guardDivisionWeapon = guardDivision.GetWeaponsInDivision();
            int guardDivisionWeaponCount = guardDivisionWeapon.Count;

            if (attackDivisionWeaponCount <= 0 || guardDivisionWeaponCount <= 0)
            {
                /*
                if (attackDivisionWeaponCount <= 0)
                    DestroyUnit(attack);

                if (guardDivisionWeaponCount <= 0)
                    DestroyUnit(guard);
                */
                return;
            }
            else
            {
                List<Weapon> willBeRemoved = new List<Weapon>();
                willBeRemoved.Clear();
                foreach (Weapon weapon in attackDivisionWeapon)
                {
                    if (weapon != null && attack != null && guard != null)                    
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
                                weapon.weaponLeftHealth -= WeaponManager.Instance.GetWeaponTemplateByID(enemyWeapon.weaponTemplateID).weaponAttack;
                                enemyWeapon.weaponLeftHealth -= WeaponManager.Instance.GetWeaponTemplateByID(weapon.weaponTemplateID).weaponAttack;                             

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
                    if(attack == null || guard == null)
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
                /*
                if(attackDivision.GetWeaponsInDivision().Count == 0)
                    DestroyUnit(attack);
                if (guardDivision.GetWeaponsInDivision().Count == 0)
                    DestroyUnit(guard);
                */
                attackDivision.onBattle = false;
                guardDivision.onBattle = false;
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

        if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy().GetAllDivisionInArmy().Contains(GOA) == true)
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
            //AudioManager.Instance.PlayVoice(VOICE_TYPE.ATTACK);
            if(division.GetDivision().onBattle == false && targetDivision.GetDivision().onBattle == false)
            {
                if(division.terrainCapability != TERRAIN_CAPABILITY.Any)
                    division.Stop();

                AttackToEnemy(division, targetDivision);
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
                    GOA.MoveTo(targetPos, GOA.GetDivision().GetDivisionSpeed());
            }
            else
            {
                if (GOA.isMoving == false)
                    GOA.MoveTo(enemy.currentMap2DLocation, GOA.GetDivision().GetDivisionSpeed());
            }
        }
        else
        {
            if (GetDistance(GOA.currentMap2DLocation, targetPos) > GOA.GetDivision().GetDivisionMaximumAttackRange())
            {
                //HUDManager.Instance.PrivateNotification("Out of range");
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
                            GOA.MoveTo(targetPos, GOA.GetDivision().GetDivisionSpeed());
                    }
                    else
                    {
                        if (GOA.isMoving == false)
                            GOA.MoveTo(enemy.currentMap2DLocation, GOA.GetDivision().GetDivisionSpeed());
                    }
                }
            }
        }

        GOA.OnMove += (GameObjectAnimator anim) =>
        {
            if (anim.GetShield() != null)
                anim.GetShield().transform.localPosition = new Vector3(anim.currentMap2DLocation.x, anim.currentMap2DLocation.y, 0);

            CheckDistanceAndAttack(GOA, enemy);
        };
        GOA.OnCountryEnter += (GameObjectAnimator anim) =>
        {
            GOA.enterCountry = map.GetCountry(GOA.currentMap2DLocation);
        };
        GOA.OnProvinceEnter += (GameObjectAnimator anim) =>
        {
            GOA.enterProvince = map.GetProvince(GOA.currentMap2DLocation);
        };
        GOA.OnKilled += (GameObjectAnimator anim) =>
        {
            if (anim == null)
                return;

            if (anim.GetShield() != null)
                Destroy(anim.GetShield());
        };
        GOA.OnMoveStart += (GameObjectAnimator anim) =>
        {
            if(GOA.terrainCapability == TERRAIN_CAPABILITY.Any)
            {
                if (GOA.comeBack == false && GOA.startingMap2DLocation != GOA.destination)
                {
                    GOA.altitudeEnd = 0.1f;
                    GOA.altitudeStart = 1.5f;
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
   
    
    public IEnumerator War(Country attackCountry, Country defenseCountry, int wave)
    {
        float start = Time.time;
        while (Time.time - start < wave)
        {
            yield return null;
        }

        StartCoroutine(LaunchMissile(2f, attackCountry.name, defenseCountry.name, Color.yellow));
        //StartCoroutine (LaunchMissile (3f, defenseCountry.name, attackCountry.name, Color.black));
    }


    IEnumerator LaunchMissile(float delay, string countryOrigin, string countryDest, Color color)
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

        Vector2 origin = map.cities[cityOrigin].unity2DLocation;
        Vector2 dest = map.cities[cityDest].unity2DLocation;
        float elevation = 20f;
        float width = 0.2f;
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

        // Triggers explosion
        StartCoroutine(AddCircleExplosion(4f, dest, Color.yellow));
    }


    public void BeginNuclearWar(Country attackCountry, Country defenseCountry)
    {
        StartCoroutine(War(attackCountry, defenseCountry, 1));
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

    public void WarStrategy(Country country)
    {
        if (country.GetArmy() == null)
            return;
        else
        { 
            UpdateDivisions(country);

            if (country == GameEventHandler.Instance.GetPlayer().GetMyCountry())
            {
                if (GameEventHandler.Instance.GetPlayer().IsLeadWar() == true)
                    return;                 
            }


            foreach (War war in country.GetWarList().ToArray())
            {
                if (country.IsAllDivisionsCreated() == false)
                    country.CreateAllDivisions();
                else
                {
                    Country enemy = war.GetEnemyCountry(country);

                    //if (country.GetNuclearWarHead() > 0)
                        //BeginNuclearWar(country, enemy);

                    List<Province> myProvinces = MapManager.Instance.FindBorderProvinces(country, enemy);

                    if (country.GetArmy().GetAllDivisionInArmy().Count <= 0 || enemy.GetArmy().GetAllDivisionInArmy().Count <= 0)
                    {
                        /*
                        // sign peace treaty
                        if (enemy.GetArmy().GetAllDivisionInArmy().Count <= 0)
                            Debug.Log(enemy.name + " wants to sign peace treaty");
                        if (country.GetArmy().GetAllDivisionInArmy().Count <= 0)
                            Debug.Log(country.name + " wants to sign peace treaty");
                        */
                    }
                    else
                    {
                        /*
                        int attackLandPower = country.GetArmy().GetLandForces().GetMilitaryPower();
                        int attackAirPower = country.GetArmy().GetAirForces().GetMilitaryPower();
                        int attackNavalPower = country.GetArmy().GetNavalForces().GetMilitaryPower();

                        int enemyLandPower = enemy.GetArmy().GetLandForces().GetMilitaryPower();
                        int enemyAirPower = enemy.GetArmy().GetAirForces().GetMilitaryPower();
                        int enemyNavalPower = enemy.GetArmy().GetNavalForces().GetMilitaryPower();

                        Debug.Log(country.name + " Victory Chance on ground : " + (100 * attackLandPower)/ (attackLandPower + enemyLandPower));
                        Debug.Log(country.name + " Victory Chance on air : " + (100 * attackAirPower) / (attackAirPower + enemyAirPower));
                        Debug.Log(country.name + " Victory Chance on naval : " + (100 * attackNavalPower) / (attackLandPower + enemyNavalPower));
                        */
                        //FollowAndAttack(country);

                        if (myProvinces.Count > 0)
                        {
                            //TotalWar(country, enemy, TERRAIN_CAPABILITY.OnlyGround);
                            AttackUnitsOnProvinces(country, enemy, myProvinces);
                        }
                        else // no land border
                        {

                        }

                        TotalWar(country, enemy, TERRAIN_CAPABILITY.Any);
                        TotalWar(country, enemy, TERRAIN_CAPABILITY.OnlyWater);
                    }
                }
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
            }
        }
    }

    public void FollowAndAttack(Country country)
    {
        GameObjectAnimator[] allDivisions = country.GetArmy().GetAllDivisionInArmy().ToArray();

        foreach (GameObjectAnimator GOA in allDivisions)
        {
            if (GOA != null)
            {
                if (GOA.enterCountry != country && GOA.enterCountry != null)
                {
                    GameObjectAnimator enemyDivision = FindNearestDivision(GOA, GOA.enterCountry, false);

                    if (enemyDivision != null)
                        MoveAndAttack(GOA, enemyDivision, true, Vector3.zero);
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
        Debug.Log(unit.name + " is looking for dockyard");
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
        int distance = 100000000;
        GameObjectAnimator GOA = null;

        if (unit == null)
            return null;

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

    
    public GameObjectAnimator GetDivisionOnProvince(Country country, Province province)
    {
        GameObjectAnimator[] allDivisions = country.GetArmy().GetAllDivisionInArmy().ToArray();

        foreach (GameObjectAnimator GOA in allDivisions)
        {
            if (GOA != null)
            {
                Province tempProvince = map.GetProvince(GOA.currentMap2DLocation);
                if (tempProvince != null)
                {
                    if (tempProvince == province)
                    {
                        return GOA;
                    }
                }
            }
        }

        return null;
    }

    
    public void AttackUnitsOnProvinces(Country country, Country enemy, List<Province> myProvinces)
    {
        foreach (Province province in myProvinces)
        {
            GameObjectAnimator myDivision = GetDivisionOnProvince(country, province);

            GameObjectAnimator enemyDivision = FindNearestDivision(myDivision, enemy, true);

            if (myDivision != null && enemyDivision != null)
            {
                MoveAndAttack(myDivision, enemyDivision, true, Vector3.zero);
            }
        }
    }
    



}
