using UnityEngine;

public class BartendView : MonoBehaviour
{
    [SerializeField] private Table[] tables;

    public void SetTableGlassAmount(int currentMember)
    {
        for (int i = 0; i < tables.Length; i++)
        {
            tables[i].SetTableGlassAmount(currentMember);
        }
    }

    public void ResetView()
    {
        foreach (var table in tables)
        {
            table.ResetView();
        }
    }
}
