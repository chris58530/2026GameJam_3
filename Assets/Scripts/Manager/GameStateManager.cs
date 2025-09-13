using System;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public GameColor CurrentColor { get; private set; } = GameColor.Red;

    [Header("View")]
    [SerializeField] private SpotLightView spotLightView;
    [SerializeField] private MemberView memberView;

    public ColorSetting[] colorSetting;
    public bool isCheckPoint = false;

    private void OnEnable()
    {
        colorSetting = spotLightView.GetColorSettings();
    }


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
        memberView.Init(() => ChangeState(GameState.GameOver));
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

    public void OnCheckPoint(GameColor color, bool isInSpotLight, MemberBase member)
    {
        if (color == CurrentColor && isInSpotLight)
        {
            Debug.Log("CheckPoint Success");
        }
        else
        {
            Debug.Log("CheckPoint Failed" + member.name);
            member.Die();
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
    none,
    Red,
    Blue,
    Green,
}
