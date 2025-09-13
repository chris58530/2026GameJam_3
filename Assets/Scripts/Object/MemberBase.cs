using UnityEngine;

public class MemberBase : MonoBehaviour
{
    public int hp;
    public int skillCooldown;
    public int speed;

    public virtual void Move(Vector3 direction)
    {
        transform.position += direction * speed * Time.deltaTime;
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
}
