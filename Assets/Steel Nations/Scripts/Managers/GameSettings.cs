using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.Shift;
using TMPro;
using WorldMapStrategyKit;

public class GameSettings : MonoBehaviour
{
    public enum GAME_SPEED
    {
        GAME_SPEED_x1,
        GAME_SPEED_x2,
        GAME_SPEED_x4,
        GAME_SPEED_x8
    }
    public enum START_RESOURCES
    {
        NONE,
        LOW,
        NORMAL,
        HIGH
    }
    public enum GAME_DIFFICULTY
    {
        NONE,
        EASY,
        NORMAL,
        HARD
    }
    public enum GAME_MODE
    {
        NONE,
        MAIN_SCENARIO,
        FREE_MODE,
        WORLD_DOMINATION,
        MONEY_TALK,
        QUIZ
    }
    public enum DATE_LIMIT
    {
        NONE,
        _2030,
        _2040,
        _2050,
        _2060,
        UNLIMITED
    }

    private static GameSettings instance;
    public static GameSettings Instance
    {
        get { return instance; }
    }

    WMSK map;

    public GameObject GameSettingsPanel;

    public SwitchManager enableTutorial;
    public SwitchManager limitedMineralResources;
    public HorizontalSelector gameDifficultySelector;
    public HorizontalSelector startingResourcesSelector;
    public HorizontalSelector finishDateSelector;

    public TextMeshProUGUI gameModeText;

    public TextMeshProUGUI enableTutorialText;
    public TextMeshProUGUI limitedMineralResourcesText;
    public TextMeshProUGUI gameDifficultyText;
    public TextMeshProUGUI startingResourcesText;
    public TextMeshProUGUI finishDateText;

    public Button confirmButton;

    GAME_MODE selectedGameMode;
    bool showTutorial;
    bool limitedResources;
    GAME_DIFFICULTY gameDifficulty;
    DATE_LIMIT dateLimit;
    START_RESOURCES startingResources;

    int startingResourcesMultipler = 1;
    float gameSpeedValue;
    int finishYear;

    void Awake()
    {
        instance = this;
        map = WMSK.instance;
        gameModeText.text = GetSelectedGameMode().ToString();
    }

    void Start()
    {
        instance = this;

        gameSpeedValue = 5.0f;

        selectedGameMode = GAME_MODE.NONE;
        gameDifficulty = GAME_DIFFICULTY.NONE;
        dateLimit = DATE_LIMIT.NONE;
        startingResources = START_RESOURCES.NONE;

        showTutorial = false;
        limitedResources = true;

        gameModeText.text = GetSelectedGameMode().ToString();
    }

    public void SetGameSpeed_X1()
    {
        map.timeSpeed = 1.0f;
        gameSpeedValue = 5.0f;
    }
    public void SetGameSpeed_X2()
    {
        map.timeSpeed = 1.25f;
        gameSpeedValue = 4.0f;
    }
    public void SetGameSpeed_X4()
    {
        map.timeSpeed = 1.5f;
        gameSpeedValue = 2.0f;
    }
    public void SetGameSpeed_X8()
    {
        map.timeSpeed = 1.75f;
        gameSpeedValue = 1.0f;
    }

    public float GetGameSpeed()
    {
        return gameSpeedValue;
    }

    public int GetFinishYear()
    {
        return finishYear;
    }

    public void Confirm()
    {
        if(gameDifficulty == GAME_DIFFICULTY.NONE || startingResources == START_RESOURCES.NONE || dateLimit == DATE_LIMIT.NONE)
        {

        }
        else
        {
            SetStartingResources(startingResourcesMultipler);
            GameSettingsPanel.SetActive(false);
            map.OnCountryClick += SelectCountry.Instance.OnCountryClick;
        }

    }

    public void SetStartingResources(int multipler)
    {
        foreach (Country country in map.countries)
        {
            country.AddMineral(MINERAL_TYPE.OIL, multipler * country.GetMineral(MINERAL_TYPE.OIL));
            country.AddMineral(MINERAL_TYPE.IRON, multipler * country.GetMineral(MINERAL_TYPE.IRON));
            country.AddMineral(MINERAL_TYPE.ALUMINIUM, multipler * country.GetMineral(MINERAL_TYPE.ALUMINIUM));
            country.AddMineral(MINERAL_TYPE.URANIUM, multipler * country.GetMineral(MINERAL_TYPE.URANIUM));
            country.AddMineral(MINERAL_TYPE.STEEL, multipler * country.GetMineral(MINERAL_TYPE.STEEL));
        }
    }

    public void ShowGameOptionsPanel(bool show)
    {
        GameSettingsPanel.SetActive(show);

        if(show)
            confirmButton.onClick.AddListener(Confirm);
        else
            confirmButton.onClick.RemoveAllListeners();
    }

    public void EnableTutorials()
    {
        showTutorial = enableTutorial.isOn;

        if (enableTutorial.isOn == true)
            enableTutorialText.text = "YES";
        else
            enableTutorialText.text = "NO";
    }
    public void LimitedResources()
    {
        limitedResources = limitedMineralResources.isOn;

        if (limitedMineralResources.isOn == true)
            limitedMineralResourcesText.text = "YES";
        else
            limitedMineralResourcesText.text = "NO";
    }

    public void SetGameDifficulty_EASY()
    {
        gameDifficulty = GAME_DIFFICULTY.EASY;
        gameDifficultyText.text = "Easy";
    }
    public void SetGameDifficulty_NORMAL()
    {
        gameDifficulty = GAME_DIFFICULTY.NORMAL;
        gameDifficultyText.text = "Normal";
    }
    public void SetGameDifficulty_HARD()
    {
        gameDifficulty = GAME_DIFFICULTY.HARD;
        gameDifficultyText.text = "Hard";
    }

    public void SetStartingResources_LOW()
    {
        startingResources = START_RESOURCES.LOW;
        startingResourcesText.text = "Low";
        startingResourcesMultipler = 1;
    }
    public void SetStartingResources_NORMAL()
    {
        startingResources = START_RESOURCES.NORMAL;
        startingResourcesText.text = "Avarage";
        startingResourcesMultipler = 10;
    }
    public void SetStartingResources_HIGH()
    {
        startingResources = START_RESOURCES.HIGH;
        startingResourcesText.text = "High";
        startingResourcesMultipler = 100;
    }


    public void SetDateLimit_2030()
    {
        dateLimit = DATE_LIMIT._2030;
        finishDateText.text = "2030";
        finishYear = 2030;
    }
    public void SetDateLimit_2040()
    {
        dateLimit = DATE_LIMIT._2040;
        finishDateText.text = "2040";
        finishYear = 2040;
    }
    public void SetDateLimit_2050()
    {
        dateLimit = DATE_LIMIT._2050;
        finishDateText.text = "2050";
        finishYear = 2050;
    }
    public void SetDateLimit_2060()
    {
        dateLimit = DATE_LIMIT._2060;
        finishDateText.text = "2060";
        finishYear = 2060;
    }
    public void SetDateLimit_UNLIMITED()
    {
        dateLimit = DATE_LIMIT.UNLIMITED;
        finishDateText.text = "UNLIMITED";
        finishYear = -1;
    }


    public GAME_MODE GetSelectedGameMode()
    {
        int mode = PlayerPrefs.GetInt("GAME_MODE");

        if (mode == 0)
            selectedGameMode = GAME_MODE.MAIN_SCENARIO;
        if (mode == 1)
            selectedGameMode = GAME_MODE.FREE_MODE;
        if (mode == 2)
            selectedGameMode = GAME_MODE.WORLD_DOMINATION;
        if (mode == 3)
            selectedGameMode = GAME_MODE.MONEY_TALK;
        if (mode == 4)
            selectedGameMode = GAME_MODE.QUIZ;

        //selectedGameMode = GAME_MODE.QUIZ;

        return selectedGameMode;
    }
}
