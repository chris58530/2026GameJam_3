using UnityEngine;

public class GameUICanvasView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    public void ClickStartButton()
    {
        GameStateManager.Instance.ChangeState(GameState.PreStart);
        root.SetActive(false);
    }

    public void ShowMenu()
    {
        root.SetActive(true);
    }
}
