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
    public Action<GameColor, bool, MemberBase> checkPointAction;
    public GameColor gameColor;
    public virtual void Move(Vector3 direction)
    {
    }

    public virtual void Die()
    {
    }

    public virtual void OnCheckPoint(GameColor color)
    {
    }

    public virtual void UseSkill()
    {
    }
    public virtual void Knock(Transform target)
    {
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
        transform.DOMove(knockTarget, knockDuration).SetEase(Ease.OutQuad);

        // 0.5秒後恢復正常狀態
        DOVirtual.DelayedCall(0.5f, () =>
        {
            isKnocked = false;
            canMove = true;
        }).SetId(GetHashCode());
    }
}
