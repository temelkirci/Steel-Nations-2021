using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Person
{
    RawImage personPicture;
    PERSON_TYPE personType;

    string personName;
    int personAge;

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

    public string PersonName
    {
        get { return personName; }
        set { personName = value; }
    }

    public PERSON_TYPE PersonType
    {
        get { return personType; }
        set { personType = value; }
    }

    public RawImage PersonPicture
    {
        get { return personPicture; }
        set { personPicture = value; }
    }
}
