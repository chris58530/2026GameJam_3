using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public GameColor CurrentColor { get; private set; } = GameColor.Red;

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }

    public void ChangeColor(GameColor newColor)
    {
        CurrentColor = newColor;
    }

}
public enum GameState
{
    MainMenu,
    PreStart,
    Playing,
    GameOver
}
public enum GameColor
{
    Red,
    Blue,
    Green,
}
