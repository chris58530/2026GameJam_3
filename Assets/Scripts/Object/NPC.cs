using System;
using UnityEngine;

public class NPC : MemberBase
{
    public Action onDie;
    public void Init(Action action)
    {
        onDie = action;
    }

    public override void OnCheckPoint(GameColor color)
    {
        if (color != gameColor)
        {
            Die();
        }
    }
    public override void Die()
    {
        ResetView();
        onDie?.Invoke();
        Destroy(gameObject);
    }


    private void UpdateColor(GameColor gameColor)
    {
        ColorSetting[] color = GameStateManager.Instance.colorSetting;
        foreach (var c in color)
        {
            if (gameColor == c.gameColor)
            {
                spriteRenderer.color = c.color;
                Debug.Log("NPC Color Changed to: " + gameColor.ToString());
                return;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Table>(out Table table))
        {
            if (table.HasGlass)
            {
                GameColor color = table.GetGlass();
                UpdateColor(color);
            }
        }
        if (collision.TryGetComponent<MemberBase>(out MemberBase member))
        {
            if (member != this)
            {
                Knock(this.transform);
            }
        }
        if (collision.TryGetComponent<SpotLight>(out SpotLight spotLight))
        {
            isInSpotLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<SpotLight>(out SpotLight spotLight))
        {
            isInSpotLight = false;
        }
    }
}
