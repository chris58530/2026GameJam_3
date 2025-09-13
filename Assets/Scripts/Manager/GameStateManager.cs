using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }

}
public enum GameState
{
    MainMenu,
    Playing,
    GameOver
}