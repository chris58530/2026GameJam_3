using System;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public GameColor CurrentColor { get; private set; } = GameColor.Red;

    [Header("StartCanvas")]
    [SerializeField] private GameUICanvasView startCanvas;

    [Header("View")]
    [SerializeField] private SpotLightView spotLightView;
    [SerializeField] private MemberView memberView;
    [SerializeField] private BartendView bartendView;

    [HideInInspector] public ColorSetting[] colorSetting;
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
                Debug.Log("========== MainMenu ");
                startCanvas.ShowMenu();
                break;
            case GameState.PreStart:
                Debug.Log("========== PreStart ");
                ChangeState(GameState.Playing);//todo 之後加入遊戲開始前的表演
                break;
            case GameState.Playing:
                Debug.Log("========== Playing ");
                OnPlayingState();
                break;
            case GameState.GameOver:
                Debug.Log("========== GameOver ");
                OnGameOver();
                break;
        }
    }

    public void ChangeColor(GameColor newColor)
    {
        CurrentColor = newColor;
        OnCheckPoint();
        Debug.Log("ChangeColor to " + newColor);
    }

    public void OnPlayingState()
    {

        //memberView
        memberView.StartNPCSpawn(onMemberDie: OnNPCDie);
        memberView.Init(() => ChangeState(GameState.GameOver));
        GameProxy.currentMember = memberView.GetMemberCount();

        //spotLightView
        spotLightView.isPlaying = true;
        spotLightView.StartColorProgress((GameColor color) =>
        {
            ChangeColor(color);
        }, ShowRoundGlass);
    }

    public void OnGameOver()
    {
        spotLightView.ResetView();
        memberView.ResetView();
        bartendView.ResetView();
        ChangeState(GameState.MainMenu);
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

        //只剩下玩家
        if (GameProxy.GetCurrentMember() <= 1)
        {
            Debug.Log("只剩下玩家 win");
            ChangeState(GameState.GameOver);
        }
    }

    public void OnCheckPoint()
    {
        memberView.CheckAllMembersPoint(CurrentColor);
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
