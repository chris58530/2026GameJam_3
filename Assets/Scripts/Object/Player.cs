using UnityEngine;

public class Player : MemberBase
{

    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (canMove && !isDashing)
            UseArrowMove();

        if (Input.GetKeyDown(KeyCode.Space))
            UseSkill();
    }

    public override void UseSkill()
    {
        if (!canDash || isDashing) return;

        StartCoroutine(DashCoroutine());
    }

    private System.Collections.IEnumerator DashCoroutine()
    {
        isDashing = true;
        canMove = false;

        Vector2 dashTarget = (Vector2)transform.position + lastMoveDirection.normalized * dashDistance;
        Vector2 startPos = transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / dashDuration;

            Vector2 currentPos = Vector2.Lerp(startPos, dashTarget, progress);
            rb.MovePosition(currentPos);

            yield return null;
        }

        isDashing = false;
        canMove = true;
    }
    public void UseArrowMove()
    {
        if (rb == null) return;

        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.A)) movement.x = -1f;
        if (Input.GetKey(KeyCode.D)) movement.x = 1f;
        if (Input.GetKey(KeyCode.W)) movement.y = 1f;
        if (Input.GetKey(KeyCode.S)) movement.y = -1f;

        if (movement != Vector2.zero)
        {
            lastMoveDirection = movement;
        }

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
        if (collision.TryGetComponent<MemberBase>(out MemberBase member))
        {
            if (member != this)
            {
                Knock(this.transform);
            }
        }
    }

}
