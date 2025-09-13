using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver
    }
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }
}

