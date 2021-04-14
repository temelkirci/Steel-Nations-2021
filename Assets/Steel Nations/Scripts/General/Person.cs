using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Person
{
    Texture2D personPicture;
    PERSON_TYPE personType;

    string personName;
    int personAge;
    int startDate;
    string birthDate;

    public Person()
    {
        personAge = 0;
        personPicture = null;
        personType = PERSON_TYPE.NONE;
        personName = string.Empty;
    }

    public int PersonAge
    {
        get { return personAge; }
        set { personAge = value; }
    }

    public int RoleStartDate
    {
        get { return startDate; }
        set { startDate = value; }
    }

    public string PersonName
    {
        get { return personName; }
        set { personName = value; }
    }

    public string BirthDate
    {
        get { return birthDate; }
        set { birthDate = value; }
    }

    public PERSON_TYPE PersonType
    {
        get { return personType; }
        set { personType = value; }
    }

    public Texture2D PersonPicture
    {
        get { return personPicture; }
        set { personPicture = value; }
    }
}
