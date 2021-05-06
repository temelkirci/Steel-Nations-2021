using WorldMapStrategyKit;
using System;

public class War
{
    Country attackCountry;
    Country guardCountry;

    DateTime startWarDate;

    GameObjectAnimator[] attackDivisions;
    GameObjectAnimator[] guardDivisions;

    public void CreateWar(Country country1, Country country2)
    {
        attackCountry = country1;
        guardCountry = country2;

        if (attackCountry.IsAllDivisionsCreated() == false)
            attackCountry.CreateAllDivisions();

        if (guardCountry.IsAllDivisionsCreated() == false)
            guardCountry.CreateAllDivisions();

        WarManager.Instance.UpdateDivisions(attackCountry);
        WarManager.Instance.UpdateDivisions(guardCountry);


        startWarDate = GameEventHandler.Instance.GetToday();
    }

    public GameObjectAnimator[] GetDivisionsInAttackCountry()
    {
        return attackDivisions;
    }

    public GameObjectAnimator[] GetDivisionsInGuardCountry()
    {
        return guardDivisions;
    }

    public Country GetEnemyCountry(Country country)
    {
        if (country == guardCountry)
            return attackCountry;
        else
            return guardCountry;
    }

    public War()
    {

    }
}
