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
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        // 開始閃爍效果
        StartBlinking();

        DOVirtual.DelayedCall(1f, () =>
        {
            coolDownComplete = true;
            StopBlinking();
        });
    }

    private void StartBlinking()
    {
        // 使用 DOTween 創建閃爍效果
        spriteRenderer.DOFade(0.3f, 0.2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(GetHashCode()); // 使用唯一ID便於停止
    }

    private void StopBlinking()
    {
        // 停止閃爍並恢復正常透明度
        DOTween.Kill(GetHashCode());
        spriteRenderer.DOFade(1f, 0.1f);
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
        if (!coolDownComplete) return;

        if (other.TryGetComponent<MemberBase>(out MemberBase member))
        {
            member.UpdateColor(color);
            Destroy(gameObject);
        }
    }
}
