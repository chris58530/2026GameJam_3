using System;
using UnityEngine;

public class Player : MemberBase
{

    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.2f;

    private Vector2 lastMoveDirection = Vector2.right;
    public Action onGameOver;

    public void Update()
    {
        if (canMove && !isDashing)
            UseArrowMove();

        if (Input.GetKeyDown(KeyCode.Space))
            UseSkill();
    }

    public void Init(Action onGameOver)
    {
        transform.position = Vector2.zero;
        this.onGameOver += onGameOver;
        canMove = true;
        canDash = true;
    }

    public override void Die()
    {
        ResetView();
        onGameOver?.Invoke();
    }

    public override void UseSkill()
    {
        if (!canDash || isDashing) return;

        StartCoroutine(DashCoroutine());
    }

    public override void OnCheckPoint(GameColor color)
    {
        if (color != gameColor)
        {
            Debug.Log($"[Player] {color}: 顏色不匹配 {gameColor}，死亡");
            Die();
        }
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

    public override void UpdateColor(GameColor gameColor)
    {
        this.gameColor = gameColor;
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
        AudioManager.Instance.PlaySFX("drink");

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
                Debug.Log($"[Player] {name}: 碰撞到 {member.name}，並擊退對方");
                member.Knock(this.transform);
            }
        }
        if (collision.TryGetComponent<SpotLight>(out SpotLight spotLight))
        {
            isInSpotLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<SpotLight>(out SpotLight spotLight))
        {
            isInSpotLight = false;
        }
    }



}
