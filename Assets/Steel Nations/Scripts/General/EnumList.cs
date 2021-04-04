public enum VOICE_TYPE
{
    ATTACK,
    ROGER_THAT,
    AFFIRMATIVE,
    BOMB,
    YES_SIR,
    FIGHTER,
    TANK,
    MENU_BACKGROUND,
    GAME_BACKGROUND,
}

public enum CITY_CLASS : byte
{
    CITY = 1,
    REGION_CAPITAL = 2,
    COUNTRY_CAPITAL = 4
}

public enum MY_UNIT_TYPE
{
    TANK,
    SHIP,
    AIRPLANE,
    DOCKYARD,
    CITY_BUILDING,
    REGION_CAPITAL_BUILDING,
    COUNTRY_CAPITAL_BUILDING,
    ARMORED_VEHICLE,
    ARTILLERY
}


public enum PERSON_TYPE
{
    NONE,
    PRESIDENT,
    VICE_PRESIDENT,
    GENERAL,
    ADMIRAL,
    SUPREME_COMMANDER,
    CHIEF_OF_GENERAL_STAFF
}

public enum DIVISION_TYPE
{
    ARMORED_DIVISION,
    MECHANIZED_INFANTRY_DIVISION,
    MOTORIZED_INFANTRY_DIVISION,

    BOMBER_DIVISION,
    AIR_DIVISION,

    SUBMARINE_DIVISION,
    DESTROYER_DIVISION,
    CARRIER_DIVISION
}

public enum MINERAL_TYPE
{
    NONE,
    OIL,
    IRON,
    STEEL,
    URANIUM,
    ALUMINIUM
}

public enum ACTION_CATEGORY
{
    NONE,
    POLITIKS,
    INTELLIGENCE_AGENCY,
    MILITARY, 
    TRADE,
    SUPPORT,
    REGION
}

public enum BUILDING_TYPE
{
    NONE,
    AIRPORT,
    MILITARY_FACTORY,
    HOSPITAL,
    UNIVERSITY,
    FACTORY,
    MINERAL_FACTORY,
    OIL_RAFINERY,
    NUCLEAR_FACILITY,
    TRADE_PORT,
    GARRISON,
    DOCKYARD,
    MILITARY_BASE
}

public enum WEAPON_TYPE
{
    NONE,
    RIFLE,
    TANK,
    ARMORED_VEHICLE,
    TOWED_ARTILLERY,
    SELF_PROPELLED_ARTILLERY,
    ROCKET_PROJECTOR,
    LASER_GUN,

    FIGHTER,
    HELICOPTER,
    DRONE,

    COASTAL_PATROL,
    CORVETTE,
    FRIGATE,
    SUBMARINE,
    DESTROYER,
    AIRCRAFT_CARRIER,

    ICBM,
    NEUTRON_BOMB,
    ANTI_MATTER_BOMB
}

public enum MILITARY_FORCES_TYPE
{
    NONE,
    LAND_FORCES,
    AIR_FORCES,
    NAVAL_FORCES
}

public enum ACTION_TYPE
{
    NONE,

    Change_System_Of_Government,

    Assassination_Of_President,

    Steal_Technology,

    Make_A_Military_Coup,

    Sign_A_Peace_Treaty,
    Begin_Nuclear_War,
    Declare_War,

    Purchase_Weapon,

    Ask_For_Money_Support,
    Give_Money_Support,

    Ask_For_Military_Access,
    Cancel_Military_Access,
    Give_Military_Access,

    Request_Garrison_Support,
    Give_Garrison_Support,

    Give_Gun_Support,
    Ask_For_Gun_Support,

    Request_License_Production,

    Place_Arms_Embargo,
    Place_Trade_Embargo,

    Ask_For_Control_Of_Region,
    Give_Control_Of_Region,

    Buy_Mineral
}

public enum POLICY_TYPE
{
    NONE = 0,
    ECONOMY = 1,
    POPULATION = 2,
    MILITARY = 3,
    HEALTH = 4,
    CONSTRUCTION = 5,
    EDUCATION = 6,
    RELIGION = 7,
    DIPLOMACY = 8
}