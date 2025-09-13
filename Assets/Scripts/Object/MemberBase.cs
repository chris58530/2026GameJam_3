using UnityEngine;

public class MemberBase : MonoBehaviour
{
    public int hp;
    public int skillCooldown;
    public float speed;

    public virtual void Move(Vector3 direction)
    {
    }
    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
    }

    public virtual void UseSkill()
    {
    }
}
