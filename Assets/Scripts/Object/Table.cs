using UnityEngine;
using DG.Tweening;

public class Table : MonoBehaviour
{
    public GameColor gameColor;
    public int glassAmount = 10;
    [SerializeField] private float getGlassTime = 1f;
    [SerializeField] private GameObject[] glassObjects;
    public bool HasGlass => glassAmount > 0;

    public void SetTableGlassAmount(int amount)
    {
        glassAmount = amount;
        Debug.Log($"SetTableGlassAmount: {glassAmount} for {gameColor}");
        for (int i = 0; i < glassObjects.Length; i++)
        {
            glassObjects[i].SetActive(i < glassAmount);
        }
    }

    public void UpdateGlassAmount()
    {
        if (glassAmount >= 0 && glassAmount < glassObjects.Length)
        {
            glassObjects[glassAmount].SetActive(false);
        }
    }

    public GameColor GetGlass()
    {
        if (glassAmount > 0)
        {
            glassAmount--;
            UpdateGlassAmount();
            return gameColor;
        }
        else return GameColor.Red;
    }

    public void ResetView()
    {
    }
}