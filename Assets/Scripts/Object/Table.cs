using UnityEngine;
using DG.Tweening;

public class Table : MonoBehaviour
{
    [SerializeField] private GameColor gameColor;
    [SerializeField] private int glassAmount = 10;
    [SerializeField] private float getGlassTime = 1f;
    [SerializeField] private GameObject[] glassObjects;
    public bool HasGlass => glassAmount > 0;

    public void SetTableGlassAmount(int amount)
    {
        glassAmount = amount;
        for (int i = 0; i < glassObjects.Length; i++)
        {
            glassObjects[i].SetActive(i < glassAmount);
        }
    }

    public GameColor GetGlass()
    {
        if (glassAmount > 0)
        {
            glassAmount--;
            return gameColor;
        }
        else return GameColor.Red;
    }

    public void ResetView()
    {
    }
}