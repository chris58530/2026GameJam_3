using DG.Tweening;
using UnityEngine;

public class Glass : MonoBehaviour
{
    public GameColor color;
    // 使用Trigger判定碰撞如果是MemberBase就把自己銷毀

    private SpriteRenderer spriteRenderer;

    public bool coolDownComplete;

    [SerializeField] private Sprite[] randomSprites;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = randomSprites[Random.Range(0, randomSprites.Length)];
        coolDownComplete = false;
        DOVirtual.DelayedCall(1f, () => { coolDownComplete = true; });
    }

    public void SetColor(GameColor newColor)
    {
        color = newColor;
        switch (color)
        {
            case GameColor.Red:
                spriteRenderer.color = Color.red;
                break;
            case GameColor.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case GameColor.Green:
                spriteRenderer.color = Color.green;
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (coolDownComplete) return;
        if (other.TryGetComponent<MemberBase>(out MemberBase member))
        {
            member.UpdateColor(color);
            Destroy(gameObject);
        }
    }
}
