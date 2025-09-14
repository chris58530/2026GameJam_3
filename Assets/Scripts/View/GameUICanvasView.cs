using UnityEngine;

public class GameUICanvasView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField]
    private GameObject infoObject;

    public void Start()
    {
        root.SetActive(true);
        infoObject.SetActive(false);
    }
    public void ClickStartButton()
    {
        GameStateManager.Instance.ChangeState(GameState.PreStart);
        root.SetActive(false);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            infoObject.SetActive(false);
        }
    }
    public void ShowMenu()
    {
        root.SetActive(true);
    }

    public void ShowInfo()
    {
        infoObject.SetActive(true);
    }

    public void HideInfo()
    {
        infoObject.SetActive(false);
    }
}
