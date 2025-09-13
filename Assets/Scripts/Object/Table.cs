using UnityEngine;
using DG.Tweening;

public class Table : MonoBehaviour
{
    [SerializeField] private GameColor gameColor;
    [SerializeField] private int glassAmount = 10;
    [SerializeField] private float getGlassTime = 1f;
    public bool HasGlass => glassAmount > 0;

    public GameColor GetGlass()
    {
        if (glassAmount > 0)
        {
            glassAmount--;
            return gameColor;
        }
        else return GameColor.none;
    }
}