using System;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public GameColor CurrentColor { get; private set; } = GameColor.Red;

    [Header("StartCanvas")]
    [SerializeField] private GameObject startCanvas;

    [Header("View")]
    [SerializeField] private SpotLightView spotLightView;
    [SerializeField] private MemberView memberView;
    [SerializeField] private BartendView bartendView;

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
                startCanvas.SetActive(true);
                break;
            case GameState.PreStart:
                Debug.Log("Enter PreStart State");
                ChangeState(GameState.Playing);//todo 之後加入遊戲開始前的表演
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
        }, ShowRoundGlass);

        //memberView
        memberView.StartNPCSpawn(onMemberDie: OnNPCDie);
        memberView.Init(() => ChangeState(GameState.GameOver));
    }

    public void OnGameOver()
    {
        spotLightView.ResetView();
        memberView.ResetView();
        bartendView.ResetView();
    }

    //每回合開始分發酒杯數量 數量為目前member數量 - 1
    public void ShowRoundGlass()
    {
        int currentMember = GameProxy.GetCurrentMember();
        bartendView.SetTableGlassAmount(currentMember - 1);
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
