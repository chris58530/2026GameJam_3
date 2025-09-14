using System;
using DG.Tweening;
using UnityEngine;

public class MemberBase : MonoBehaviour
{
    public int hp;
    public int skillCooldown;
    public float speed;

    public bool isDashing;
    public bool isKnocked;
    public bool canMove = true;
    public bool canDash = true;
    public bool isInSpotLight;
    public GameColor gameColor;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Glass glassPrefab;

    public Action<MemberBase> removeMemberCallback;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    public virtual void Move(Vector3 direction)
    {
    }

    public virtual void Die()
    {
    }
    public virtual void ResetView()
    {
        removeMemberCallback?.Invoke(this);
        isKnocked = false;
        isDashing = false;
        canMove = false;
        canDash = false;
        rb.linearVelocity = Vector2.zero;
        StopAllCoroutines();
        DOTween.Kill(GetHashCode());
    }
    public virtual void OnCheckPoint(GameColor color)
    {
    }

    public virtual void UseSkill()
    {
    }

    public virtual void UpdateColor(GameColor gameColor)
    {

    }

    public virtual void ResetColor()
    {
        spriteRenderer.color = Color.white;
        gameColor = GameColor.white;
    }

    public void ThrowOutGlass()
    {
        if (gameColor == GameColor.white) return;
        if (glassPrefab != null)
        {
            Glass thrownGlass = Instantiate(glassPrefab, transform.position, Quaternion.identity);
            thrownGlass.SetColor(gameColor);
            ResetColor();
        }
    }

    public virtual void Knock(Transform target)
    {
        Debug.Log($"[MemberBase] {name}: 被擊退");
        isKnocked = true;
        canMove = false;

        // 計算被擊退的方向（從目標指向自己）
        Vector3 knockDirection = (transform.position - target.position).normalized;

        // 設定擊退距離和時間
        float knockDistance = 2f;
        float knockDuration = 0.3f;

        // 計算目標位置
        Vector3 knockTarget = transform.position + knockDirection * knockDistance;

        // 使用DOTween移動到目標位置
        transform.DOMove(knockTarget, knockDuration).SetEase(Ease.OutQuad).SetId(GetHashCode());

        // 0.5秒後恢復正常狀態
        DOVirtual.DelayedCall(0.5f, () =>
        {
            isKnocked = false;
            canMove = true;
        }).SetId(GetHashCode());

        AudioManager.Instance.PlaySFX("impact");

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<MemberBase>(out MemberBase member))
        {
            if (!isDashing) return;
            Debug.Log($"[MemberBase] {name}: 碰撞到 {member.name}，並擊退對方");
            member.Knock(this.transform);
            member.ThrowOutGlass();
            member.ResetColor();
        }
    }
}
