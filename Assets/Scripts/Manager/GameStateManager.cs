using System;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public GameColor CurrentColor { get; private set; } = GameColor.Red;

    [Header("View")]
    [SerializeField] private SpotLightView spotLightView;
    [SerializeField] private MemberView memberView;




    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        switch (newState)
        {
            case GameState.MainMenu:
                Debug.Log("Enter MainMenu State");
                break;
            case GameState.PreStart:
                Debug.Log("Enter PreStart State");
                ChangeState(GameState.Playing);//todo 之後加入倒數三二一
                break;
            case GameState.Playing:
                Debug.Log("Enter Playing State");
                OnPlayingState();
                break;
            case GameState.GameOver:
                Debug.Log("Enter GameOver State");
                OnGameOver();
                break;
        }
    }

    public void ChangeColor(GameColor newColor)
    {
        CurrentColor = newColor;
        Debug.Log("ChangeColor to " + newColor);
    }

    public void OnPlayingState()
    {
        //spotLightView
        spotLightView.isPlaying = true;
        spotLightView.StartColorProgress((GameColor color) =>
        {
            ChangeColor(color);
        });

        //memberView
        memberView.StartNPCSpawn(onMemberDie: OnNPCDie);
    }

    public void OnGameOver()
    {
        spotLightView.EndColorProgress();
    }

    public void OnNPCDie()
    {
        GameProxy.currentMember--;

        if (GameProxy.GetCurrentMember() <= 0)
        {
            ChangeState(GameState.GameOver);
        }
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
