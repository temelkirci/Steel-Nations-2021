using UnityEngine;

public class PeopleManager : Singleton<PeopleManager>
{
    public Person CreatePerson(PERSON_TYPE personType, string personName)
    {
        Person person = new Person();

        person.PersonAge = GetPersonAgeByPersonType(personType);
        person.PersonType = personType;

        if (personName == string.Empty)
            person.PersonName = GetPersonNameByPersonType(personType);
        else
            person.PersonName = personName;

        return person;
    }

    public Person CreatePresident(string personName, string countryName, int since, string birthDate)
    {
        Person person = new Person();

        person.PersonType = PERSON_TYPE.PRESIDENT;
        person.PersonName = personName;
        person.PersonPicture = LoadPeoplePicture(countryName);
        person.RoleStartDate = since;
        person.BirthDate = birthDate;

        return person;
    }

    Texture2D LoadPeoplePicture(string dir)
    {
        return Resources.Load("People/President/" + dir) as Texture2D;
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

    string GetPersonNameByPersonType(PERSON_TYPE personType)
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