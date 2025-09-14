using System;
using DG.Tweening;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public GameColor CurrentColor = GameColor.Red;
    public GameColor NextColor = GameColor.Red;

    [SerializeField] private float introTime = 5;


    [Header("StartCanvas")]
    [SerializeField] private GameUICanvasView startCanvas;
    [SerializeField] private GameObject introObject;

    [Header("View")]
    [SerializeField] private SpotLightView spotLightView;
    [SerializeField] private MemberView memberView;
    [SerializeField] private BartendView bartendView;

    [HideInInspector] public ColorSetting[] colorSetting;

    public Action<NPCState> onNPCStateChange;

    public bool canMove;
    public bool isCheckPoint = false;

    private void OnEnable()
    {
        colorSetting = spotLightView.GetColorSettings();
        AudioManager.Instance.PlayBGM("MenuBGM");
        introObject.SetActive(false);

    }


    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        switch (newState)
        {
            case GameState.MainMenu:
                Debug.Log("========== MainMenu ");
                startCanvas.ShowMenu();
                RestSetAllMemberColor();
                AudioManager.Instance.PlayBGM("MenuBGM");
                break;
            case GameState.PreStart:
                introObject.SetActive(true);
                Debug.Log("========== PreStart ");
                AudioManager.Instance.PlayBGM("GameBGM");
                memberView.StartNPCSpawn(onMemberDie: OnNPCDie);
                memberView.Init(() => ChangeState(GameState.GameOver));
                GameProxy.currentMember = memberView.GetMemberCount();
                DOVirtual.DelayedCall(introTime, () =>
                {
                    ChangeState(GameState.Playing);
                }).SetId(GetHashCode());
                break;
            case GameState.Playing:
                introObject.SetActive(false);

                Debug.Log("========== Playing ");
                OnPlayingState();
                AudioManager.Instance.PlaySFX2("char");

                break;
            case GameState.GameOver:
                Debug.Log("========== GameOver ");
                OnGameOver();
                AudioManager.Instance.PlaySFX2("GameOver");
                break;
        }
    }
    private void RestSetAllMemberColor()
    {
        memberView.AllMembersWhite();
    }
    public void ChangeColor(GameColor newColor)
    {
        CurrentColor = newColor;
        OnCheckPoint(newColor);
        Debug.Log("ChangeColor to " + newColor);
    }

    public void OnPlayingState()
    {

        //memberView


        //spotLightView
        spotLightView.isPlaying = true;
        spotLightView.StartColorProgress((GameColor color) =>
        {
            ChangeColor(color);
        }, ShowRoundGlass, RestSetAllMemberColor);

        DOVirtual.DelayedCall(1f, () =>
        {
            canMove = true;
        }).SetId(GetHashCode());
    }

    public void OnGameOver()
    {
        canMove = false;

        spotLightView.ResetView();
        memberView.ResetView();
        bartendView.ResetView();
        DOVirtual.DelayedCall(2f, () =>
        {
            AudioManager.Instance.StopAll();
            ChangeState(GameState.MainMenu);
        }).SetId(GetHashCode());
    }

    //每回合開始分發酒杯數量 數量為目前member數量 - 1
    public void ShowRoundGlass()
    {
        int currentMember = GameProxy.GetCurrentMember();
        bartendView.SetTableGlassAmount(currentMember);
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

    public void OnCheckPoint(GameColor newColor)
    {
        memberView.CheckAllMembersPoint(newColor);
    }

    public void ChangeNPCState(NPCState newState)
    {
        onNPCStateChange?.Invoke(newState);
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
    white,
    Red,
    Blue,
    Green,
}
