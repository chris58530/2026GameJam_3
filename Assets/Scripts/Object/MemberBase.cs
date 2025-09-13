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
    // public override void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.TryGetComponent<Table>(out Table table))
    //     {
    //         if (table.HasGlass)
    //         {
    //             table.AddQueue(this);
    //         }
    //     }
    // }
    // public override void OnTriggerExit2D(Collider2D other)
    // {
    //     // if (other.gameObject.TryGetComponent<Table>(out Table table))
    //     // {
    //     // }
    // }
}
