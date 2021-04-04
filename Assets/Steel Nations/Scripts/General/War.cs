using WorldMapStrategyKit;
using System;

public class War
{
    Country attackCountry;
    Country guardCountry;

    DateTime startWarDate;

    public void CreateWar(Country country1, Country country2)
    {
        if(country1 == null || country2 == null)
        {

        }
        else
        {
            attackCountry = country1;
            guardCountry = country2;

            startWarDate = GameEventHandler.Instance.GetToday();
        }
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
        attackCountry = null;
        guardCountry = null;
    }
}
