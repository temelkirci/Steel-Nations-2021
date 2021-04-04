using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RESOURCE_TYPE
{
    MINERAL,
    WEAPON,
    BUILDING,
    ORGANIZATION,
    FLAG
}

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    public static ResourceManager Instance
    {
        get { return instance; }
    }

    /*
     //Load a text file (Assets/Resources/Text/textFile01.txt)
        var textFile = Resources.Load<TextAsset>("Text/textFile01");

        //Load text from a JSON file (Assets/Resources/Text/jsonFile01.json)
        var jsonTextFile = Resources.Load<TextAsset>("Text/jsonFile01");
        //Then use JsonUtility.FromJson<T>() to deserialize jsonTextFile into an object

        //Load an AudioClip (Assets/Resources/Audio/audioClip01.mp3)
        var audioClip = Resources.Load<AudioClip>("Audio/audioClip01");
    */

    void Start()
    {
        instance = this;
    }

    public Sprite LoadSprite(string spriteName)
    {
        //Load a Sprite (Assets/Resources/Sprites/sprite01.png)
        var sprite = Resources.Load<Sprite>("Sprites/sprite01");

        return sprite;
    }

    public GameObject LoadGameObject(string objectName)
    {
        return Instantiate(Resources.Load<GameObject>(objectName));
    }

    public Texture2D LoadTexture(RESOURCE_TYPE resourceType, string fileName)
    {
        string root = GetResourcePath(resourceType);
        //Load a Texture (Assets/Resources/Textures/texture01.png)
        var texture = Resources.Load<Texture2D>(root + fileName);

        return texture;
    }

    string GetResourcePath(RESOURCE_TYPE resourceType)
    {
        if (resourceType == RESOURCE_TYPE.BUILDING)
            return "Buildings/";
        if (resourceType == RESOURCE_TYPE.WEAPON)
            return "Weapons/";
        if (resourceType == RESOURCE_TYPE.FLAG)
            return "Flags/";
        if (resourceType == RESOURCE_TYPE.MINERAL)
            return "Minerals/";
        if (resourceType == RESOURCE_TYPE.ORGANIZATION)
            return "Organizations/";

        return string.Empty;
    }
}
