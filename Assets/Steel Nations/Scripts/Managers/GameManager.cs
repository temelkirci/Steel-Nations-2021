using UnityEngine;
using Michsky.LSS;

public enum GAME_TYPE
{
    NONE,
    NEW_GAME,
    LOAD_GAME,
    BATTLE_ROYALE
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public LoadingScreenManager loadingManager;
    GAME_TYPE gameType;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public GAME_TYPE GameType
    {
        get { return gameType; }
        set { gameType = value; }
    }

    public void LoadGame()
    {
        GameType = GAME_TYPE.LOAD_GAME;

        loadingManager.LoadScene("Game Scene");
    }

    public void NewGame()
    {
        GameType = GAME_TYPE.NEW_GAME;

        loadingManager.LoadScene("Game Scene");
    }

}
