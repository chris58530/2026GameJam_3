using System;
using UnityEngine;

public class NPC : MemberBase
{
    public Action onDie;
    public NPCState NPCState;

    [Header("NPC Targets")]
    public Table targetTable;        // 目標桌子
    public MemberBase huntingTarget; // 狩獵目標
    public SpotLight targetSpotLight; // 目標聚光燈
    public GameObject targetGlass;   // 目標玻璃

    [Header("NPC Settings")]
    public float moveSpeed = 2f;     // 移動速度
    [SerializeField] private float dashDistance = 3f;    // 衝刺距離
    [SerializeField] private float dashDuration = 0.2f;  // 衝刺時間
    [SerializeField] private bool canSearchGlass = false; // 是否可以尋找獨立的玻璃物件

    private Vector2 lastMoveDirection = Vector2.right;    // 記錄最後移動方向
    private Vector3 lastNPCPosition; // 記錄最後位置
    private GameColor lastNPCColor;  // 記錄最後顏色

    public void Init(Action action)
    {
        onDie = action;
        targetSpotLight = FindFirstObjectByType<SpotLight>();
    }

    private void Update()
    {
        // 根據當前狀態執行對應的更新邏輯
        UpdateCurrentState();
    }

    private void UpdateCurrentState()
    {
        switch (NPCState)
        {
            case NPCState.SearchingTable:
                UpdateSearchingTable();
                break;
            case NPCState.MovingToTable:
                UpdateMovingToTable();
                break;
            case NPCState.MovingToSpotLight:
                UpdateMovingToSpotLight();
                break;
            case NPCState.WaitingForTurn:
                UpdateWaitingForTurn();
                break;
            case NPCState.HuntingTarget:
                UpdateHuntingTarget();
                break;
        }
    }

    private void UpdateSearchingTable()
    {
        if (!canMove || isDashing || isKnocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        GameColor targetColor = GameStateManager.Instance.NextColor;

        // 檢查是否已經有正確的顏色
        if (gameColor != GameColor.white && gameColor == targetColor)
        {
            ChangeState(NPCState.MovingToSpotLight);
            return;
        }

        // 尋找匹配顏色的桌子
        Table foundTable = FindTableWithColor(targetColor);
        if (foundTable != null)
        {
            SetTargetTable(foundTable);
            ChangeState(NPCState.MovingToTable);
        }
        else if (canSearchGlass)
        {
            // 如果沒有找到桌子且開關開啟，尋找獨立的玻璃
            GameObject foundGlass = FindGlassWithTargetColor();
            if (foundGlass != null)
            {
                MoveToGlass(foundGlass);
                ChangeState(NPCState.MovingToTable);
            }
            else
            {
                // 找不到玻璃就去衝撞玩家
                StartHuntingMode();
            }
        }
        else
        {
            // 關閉玻璃搜尋時直接去衝撞玩家
            StartHuntingMode();
        }
    }

    private Table FindTableWithColor(GameColor targetColor)
    {
        Table[] allTables = FindObjectsByType<Table>(FindObjectsSortMode.None);
        Table closestTable = null;
        float closestDistance = float.MaxValue;

        foreach (Table table in allTables)
        {
            if (table.HasGlass && table.gameColor == targetColor)
            {
                float distance = Vector2.Distance(transform.position, table.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTable = table;
                }
            }
        }

        return closestTable;
    }

    private GameObject FindGlassWithTargetColor()
    {
        GameColor targetColor = GameStateManager.Instance.NextColor;
        Glass[] allGlasses = FindObjectsByType<Glass>(FindObjectsSortMode.None);
        GameObject closestGlass = null;
        float closestDistance = float.MaxValue;

        foreach (Glass glass in allGlasses)
        {
            if (glass.color == targetColor)
            {
                float distance = Vector2.Distance(transform.position, glass.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGlass = glass.gameObject;
                }
            }
        }

        return closestGlass;
    }

    private void MoveToGlass(GameObject glass)
    {
        targetGlass = glass;
        targetTable = null;
    }

    private void DoIdleMovement()
    {
        // 每隔一段時間改變移動方向
        if (Time.time % 3f < 0.1f) // 每3秒改變一次方向
        {
            Vector2 randomDirection = new Vector2(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)
            ).normalized;

            lastMoveDirection = randomDirection; // 更新移動方向
            rb.linearVelocity = randomDirection * (moveSpeed * 0.5f); // 較慢的速度
        }

        // 隨機使用衝刺（低機率）
        if (canDash && UnityEngine.Random.Range(0f, 1f) < 0.01f) // 1% 機率每幀
        {
            UseSkill();
        }
    }

    private void UpdateMovingToTable()
    {
        if (!canMove || isDashing || isKnocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 處理移動到玻璃
        if (targetGlass != null)
        {
            HandleGlassMovement();
            return;
        }

        // 處理移動到桌子
        if (targetTable != null)
        {
            HandleTableMovement();
            return;
        }

        // 沒有目標，進入狩獵模式
        StartHuntingMode();
    }

    private void HandleGlassMovement()
    {
        if (targetGlass == null)
        {
            ChangeState(NPCState.SearchingTable);
            return;
        }

        MoveToTarget(targetGlass.transform.position);

        // 檢查是否獲得目標顏色
        GameColor targetColor = GameStateManager.Instance.NextColor;
        if (gameColor != GameColor.white && gameColor == targetColor)
        {
            rb.linearVelocity = Vector2.zero;
            targetGlass = null;
            ChangeState(NPCState.MovingToSpotLight);
        }
    }

    private void HandleTableMovement()
    {
        // 檢查桌子是否還有玻璃
        if (!targetTable.HasGlass)
        {
            targetTable = null;

            // 只有在開關開啟時才尋找獨立的玻璃
            if (canSearchGlass)
            {
                GameObject foundGlass = FindGlassWithTargetColor();
                if (foundGlass != null)
                {
                    MoveToGlass(foundGlass);
                }
                else
                {
                    StartHuntingMode();
                }
            }
            else
            {
                StartHuntingMode();
            }
            return;
        }

        // 檢查桌子顏色是否匹配
        GameColor targetColor = GameStateManager.Instance.NextColor;
        if (targetTable.gameColor != targetColor)
        {
            targetTable = null;
            ChangeState(NPCState.SearchingTable);
            return;
        }

        MoveToTarget(targetTable.transform.position);

        // 檢查是否獲得目標顏色
        if (gameColor != GameColor.white && gameColor == targetColor)
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(NPCState.MovingToSpotLight);
        }
    }

    private void MoveToTarget(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        lastMoveDirection = direction;
        rb.linearVelocity = direction * moveSpeed;

        float distance = Vector2.Distance(transform.position, targetPosition);

        // 隨機使用衝刺加速
        if (distance > 2f && distance < 5f && canDash && UnityEngine.Random.Range(0f, 1f) < 0.01f)
        {
            UseSkill();
        }
    }

    private void StartHuntingMode()
    {
        GameColor targetColor = GameStateManager.Instance.NextColor;
        MemberBase targetMember = FindMemberWithColor(targetColor);

        if (targetMember != null)
        {
            SetHuntingTarget(targetMember);
            ChangeState(NPCState.HuntingTarget);
        }
        else
        {
            ChangeState(NPCState.SearchingTable);
        }
    }

    private MemberBase FindMemberWithColor(GameColor targetColor)
    {
        System.Collections.Generic.List<MemberBase> memberList = new System.Collections.Generic.List<MemberBase>();

        // 優先檢查玩家
        Player player = FindFirstObjectByType<Player>();
        if (player != null && player != this)
        {
            // 如果玩家有目標顏色，優先攻擊
            if (player.gameColor == targetColor)
            {
                return player;
            }
            // 如果玩家沒有目標顏色但不是白色，也可以攻擊
            else if (player.gameColor != GameColor.white)
            {
                memberList.Add(player);
            }
        }

        // // 檢查其他NPC
        // NPC[] npcs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        // foreach (NPC npc in npcs)
        // {
        //     if (npc != this && npc.gameColor == targetColor)
        //     {
        //         memberList.Add(npc);
        //     }
        // }

        // 如果有其他可攻擊的目標，隨機選擇一個
        if (memberList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, memberList.Count);
            return memberList[randomIndex];
        }

        return null;
    }

    private void UpdateMovingToSpotLight()
    {
        if (gameColor == GameColor.white)
        {
            ChangeState(NPCState.SearchingTable);
            return;
        }

        GameColor targetColor = GameStateManager.Instance.NextColor;
        if (gameColor != targetColor)
        {
            StartHuntingMode();
            return;
        }

        if (targetSpotLight != null && canMove && !isDashing && !isKnocked)
        {
            Vector2 direction = (targetSpotLight.transform.position - transform.position).normalized;
            lastMoveDirection = direction;

            if (isInSpotLight)
            {
                // 在聚光燈內跟隨移動
                float distanceToCenter = Vector2.Distance(transform.position, targetSpotLight.transform.position);
                float followSpeed = distanceToCenter > 0.5f ? moveSpeed * 0.8f : moveSpeed * 0.3f;
                rb.linearVelocity = direction * followSpeed;
            }
            else
            {
                // 移動到聚光燈
                rb.linearVelocity = direction * moveSpeed;

                float distance = Vector2.Distance(transform.position, targetSpotLight.transform.position);
                if (distance > 3f && canDash && UnityEngine.Random.Range(0f, 1f) < 0.03f)
                {
                    UseSkill();
                }
            }
        }
        else if (targetSpotLight == null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void UpdateWaitingForTurn()
    {
        if (rb != null)
        {
            if (isInSpotLight && targetSpotLight != null)
            {
                Vector2 followDirection = (targetSpotLight.transform.position - transform.position).normalized;
                rb.linearVelocity = followDirection * (moveSpeed * 0.2f);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void UpdateHuntingTarget()
    {
        if (huntingTarget != null && canMove && !isDashing && !isKnocked)
        {
            Vector2 direction = (huntingTarget.transform.position - transform.position).normalized;
            lastMoveDirection = direction;
            rb.linearVelocity = direction * moveSpeed;

            float distance = Vector2.Distance(transform.position, huntingTarget.transform.position);
            if (distance < 2f && canDash)
            {
                UseSkill();
            }
        }
        else if (huntingTarget == null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetTargetTable(Table table)
    {
        targetTable = table;
    }

    public void SetHuntingTarget(MemberBase target)
    {
        huntingTarget = target;
    }

    public override void OnCheckPoint(GameColor color)
    {
        if (color != gameColor)
        {
            Die();
        }
    }

    public override void Die()
    {
        ResetView();
        onDie?.Invoke();
        Destroy(gameObject);
    }

    public override void ResetView()
    {
        base.ResetView();
        targetTable = null;
        huntingTarget = null;
        targetGlass = null;
        ChangeState(NPCState.SearchingTable);
        Destroy(gameObject);
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
                return;
            }
        }
        AudioManager.Instance.PlaySFX("drink");

    }

    public void ChangeState(NPCState newState)
    {
        NPCState = newState;

        switch (newState)
        {
            case NPCState.SearchingTable:
                OnEnterSearchingTable();
                break;
            case NPCState.MovingToTable:
                OnEnterMovingToTable();
                break;
            case NPCState.MovingToSpotLight:
                OnEnterMovingToSpotLight();
                break;
            case NPCState.WaitingForTurn:
                OnEnterWaitingForTurn();
                break;
            case NPCState.HuntingTarget:
                OnEnterHuntingTarget();
                break;
        }
    }

    // 各狀態進入時的處理方法
    private void OnEnterSearchingTable()
    {
        targetTable = null;
        targetGlass = null;
        huntingTarget = null;
    }

    private void OnEnterMovingToTable()
    {
        if (targetTable != null)
        {
            lastNPCPosition = transform.position;
            lastNPCColor = gameColor;
        }
    }

    private void OnEnterMovingToSpotLight()
    {
        targetGlass = null;
    }

    private void OnEnterWaitingForTurn()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnEnterHuntingTarget()
    {
        // 狩獵模式初始化
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Table>(out Table table))
        {
            if (table.HasGlass)
            {
                GameColor color = table.GetGlass();
                gameColor = color;
                UpdateColor(color);
            }
        }
        if (collision.TryGetComponent<MemberBase>(out MemberBase member))
        {
            if (member != this)
            {
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

    // 覆寫 UseSkill 方法來實現 NPC 的衝刺技能
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
}
public enum NPCState
{
    SearchingTable,    // 尋找目標顏色的桌子
    MovingToTable,     // 移動到桌子
    MovingToSpotLight, // 移動到聚光燈
    WaitingForTurn,    // 等待下一輪
    HuntingTarget      // 狩獵模式：尋找有顏色的目標進行衝撞
}
