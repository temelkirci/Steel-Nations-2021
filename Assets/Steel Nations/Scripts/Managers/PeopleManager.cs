
public class PeopleManager : Singleton<PeopleManager>
{
    public Person CreatePerson(PERSON_TYPE personType, string personName, WorldMapStrategyKit.Country country)
    {
        Person person = new Person();

        person.PersonAge = GetPersonAgeByPersonType(personType);
        person.PersonType = personType;

        if (personName == string.Empty)
            person.PersonName = GetPersonNameByPersonType(country, personType);
        else
            person.PersonName = personName;

        return person;
    }

    int GetPersonAgeByPersonType(PERSON_TYPE personType)
    {
        if (personType == PERSON_TYPE.PRESIDENT)
            return UnityEngine.Random.Range(45, 75);
        if (personType == PERSON_TYPE.VICE_PRESIDENT)
            return UnityEngine.Random.Range(40, 70);
        if (personType == PERSON_TYPE.GENERAL)
            return UnityEngine.Random.Range(45, 65);
        if (personType == PERSON_TYPE.ADMIRAL)
            return UnityEngine.Random.Range(45, 65);
        if (personType == PERSON_TYPE.SUPREME_COMMANDER)
            return UnityEngine.Random.Range(50, 70);
        if (personType == PERSON_TYPE.CHIEF_OF_GENERAL_STAFF)
            return UnityEngine.Random.Range(55, 75);

        return 0;
    }

    string GetPersonNameByPersonType(WorldMapStrategyKit.Country country, PERSON_TYPE personType)
    {
        if (personType == PERSON_TYPE.PRESIDENT)
            return "President";
        if (personType == PERSON_TYPE.VICE_PRESIDENT)
            return "Vice President";
        if (personType == PERSON_TYPE.GENERAL)
            return "General";
        if (personType == PERSON_TYPE.ADMIRAL)
            return "Admiral";
        if (personType == PERSON_TYPE.CHIEF_OF_GENERAL_STAFF)
            return "Chief Of General Staff";
        if (personType == PERSON_TYPE.SUPREME_COMMANDER)
            return "Supreme Commander";

        return string.Empty;
    }
}