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

        // 直接設定速度，不使用AddForce
        rb.linearVelocity = movement * speed;
    }

    private void UpdateColor(GameColor gameColor)
    {
        ColorSetting[] color = GameStateManager.Instance.colorSetting;
        foreach (var c in color)
        {
            if (gameColor == c.gameColor)
            {
                spriteRenderer.color = c.color;
                Debug.Log("Player Color Changed to: " + gameColor.ToString());
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
    }

}
