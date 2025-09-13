using UnityEngine;
using DG.Tweening;

public class Table : MonoBehaviour
{
    [SerializeField] private int glassAmount = 10;
    [SerializeField] private float getGlassTime = 1f;
    public bool HasGlass => glassAmount > 0;
    // private List<MemberBase> glassQueue = new List<MemberBase>();

    public void GetGlass()
    {
        if (glassAmount > 0)
        {
            glassAmount--;
        }
    }
    public void AddQueue(MemberBase member)
    {
        // glassQueue.Add(member);
        // if (HasGlass)
        // {
        // }

    }
    public void RemoveQueue(MemberBase member)
    {
        // glassQueue.Remove(member);
    }
}