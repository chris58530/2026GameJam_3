using UnityEngine;

public class Player : MemberBase
{
    public bool canMove = true;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void Update()
    {
        if (canMove)
            UseArrowMove();
    }

    public void UseArrowMove()
    {
        if (rb == null) return;

        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) movement.x = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) movement.x = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) movement.y = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) movement.y = -1f;

        if (movement != Vector2.zero)
        {
            rb.AddForce(movement * speed, ForceMode2D.Force);
        }
    }

    private void UpdateColor(GameColor gameColor)
    {
        Color color = GameStateManager.Instance.colorSetting[(int)gameColor].color;
        spriteRenderer.color = color;
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
    }

}
