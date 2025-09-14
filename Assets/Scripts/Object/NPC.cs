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

    [Header("NPC Settings")]
    public float moveSpeed = 2f;     // 移動速度
    public void Init(Action action)
    {
        onDie = action;
        // 自動尋找聚光燈
        targetSpotLight = FindFirstObjectByType<SpotLight>();
        Debug.Log($"[NPC Init] {name}: 初始化完成");
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

    // 各狀態的更新方法
    private void UpdateSearchingTable()
    {
        Debug.Log("[NPC] " + name + ": 搜尋桌子中...");

        if (canMove && !isDashing && !isKnocked)
        {
            // 獲取目標顏色
            GameColor targetColor = GameStateManager.Instance.NextColor;
            Debug.Log($"[NPC] {name}: 目標顏色 - {targetColor}");

            // 尋找匹配顏色的桌子
            Table foundTable = FindTableWithColor(targetColor);

            if (foundTable != null)
            {
                // 找到匹配的桌子，設定為目標並切換狀態
                SetTargetTable(foundTable);
                ChangeState(NPCState.MovingToTable);
                Debug.Log($"[NPC] {name}: 找到目標桌子 {foundTable.name}，切換到移動狀態");
            }
            else
            {
                // 沒找到匹配的桌子，做待機移動
                Debug.Log($"[NPC] {name}: 沒有找到顏色 {targetColor} 的桌子，繼續搜尋");
                DoIdleMovement();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // 尋找特定顏色的桌子
    private Table FindTableWithColor(GameColor targetColor)
    {
        Table[] allTables = FindObjectsByType<Table>(FindObjectsSortMode.None);
        Table closestTable = null;
        float closestDistance = float.MaxValue;

        foreach (Table table in allTables)
        {
            // 檢查桌子是否有玻璃且顏色匹配
            if (table.HasGlass && table.gameColor == targetColor)
            {
                float distance = Vector2.Distance(transform.position, table.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTable = table;
                }

                Debug.Log($"[NPC] {name}: 找到匹配桌子 {table.name}，顏色: {table.gameColor}，距離: {distance}");
            }
        }

        if (closestTable != null)
        {
            Debug.Log($"[NPC] {name}: 選定最近的桌子 {closestTable.name}，距離: {closestDistance}");
        }

        return closestTable;
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

            rb.linearVelocity = randomDirection * (moveSpeed * 0.5f); // 較慢的速度
        }
    }

    private void UpdateMovingToTable()
    {
        if (targetTable != null && canMove && !isDashing && !isKnocked)
        {
            Vector2 direction = (targetTable.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // 檢查是否接近目標
            float distance = Vector2.Distance(transform.position, targetTable.transform.position);
            if (distance < 0.5f)
            {
                rb.linearVelocity = Vector2.zero;
                Debug.Log($"[NPC] {name}: 到達桌子");

                // 檢查是否成功拿到顏色
                if (gameColor != GameColor.white && gameColor == GameStateManager.Instance.NextColor)
                {
                    // 成功拿到目標顏色，移動到聚光燈
                    Debug.Log($"[NPC] {name}: 成功獲得目標顏色 {gameColor}，移動到聚光燈");
                    ChangeState(NPCState.MovingToSpotLight);
                }
                else
                {
                    // 沒有獲得目標顏色，進入狩獵模式
                    Debug.Log($"[NPC] {name}: 沒有獲得目標顏色，進入狩獵模式");
                    StartHuntingMode();
                }
            }
        }
        else if (targetTable == null)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.LogWarning($"[NPC] {name}: 沒有設定目標桌子，進入狩獵模式");
            StartHuntingMode();
        }
    }

    // 開始狩獵模式：尋找有目標顏色的成員進行攻擊
    private void StartHuntingMode()
    {
        GameColor targetColor = GameStateManager.Instance.NextColor;
        MemberBase targetMember = FindMemberWithColor(targetColor);

        if (targetMember != null)
        {
            SetHuntingTarget(targetMember);
            ChangeState(NPCState.HuntingTarget);
            Debug.Log($"[NPC] {name}: 找到狩獵目標 {targetMember.name}，顏色: {targetColor}");
        }
        else
        {
            Debug.Log($"[NPC] {name}: 沒有找到有顏色 {targetColor} 的目標，回到搜尋狀態");
            ChangeState(NPCState.SearchingTable);
        }
    }

    // 尋找有特定顏色的成員
    private MemberBase FindMemberWithColor(GameColor targetColor)
    {
        MemberBase[] allMembers = new MemberBase[0];
        System.Collections.Generic.List<MemberBase> memberList = new System.Collections.Generic.List<MemberBase>();

        // 檢查玩家
        Player player = FindFirstObjectByType<Player>();
        if (player != null && player.gameColor == targetColor && player != this)
        {
            memberList.Add(player);
        }

        // 檢查其他NPC
        NPC[] npcs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        foreach (NPC npc in npcs)
        {
            if (npc != this && npc.gameColor == targetColor)
            {
                memberList.Add(npc);
            }
        }

        // 如果有找到目標，隨機選擇一個
        if (memberList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, memberList.Count);
            MemberBase selectedTarget = memberList[randomIndex];
            Debug.Log($"[NPC] {name}: 從 {memberList.Count} 個目標中選擇 {selectedTarget.name}");
            return selectedTarget;
        }

        return null;
    }

    private void UpdateMovingToSpotLight()
    {
        if (gameColor == GameColor.white)
        {
            Debug.Log($"[NPC] {name}: 顏色為白色，無法進入聚光燈，回到搜尋狀態");
            ChangeState(NPCState.SearchingTable);
            return;
        }

        if (targetSpotLight != null && canMove && !isDashing && !isKnocked)
        {
            Vector2 direction = (targetSpotLight.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // 檢查是否到達聚光燈
            if (isInSpotLight)
            {
                // 在聚光燈內，但仍然跟隨聚光燈移動
                float distanceToCenter = Vector2.Distance(transform.position, targetSpotLight.transform.position);

                // 如果距離聚光燈中心太遠，繼續移動靠近
                if (distanceToCenter > 0.5f)
                {
                    Vector2 followDirection = (targetSpotLight.transform.position - transform.position).normalized;
                    rb.linearVelocity = followDirection * (moveSpeed * 0.8f); // 稍微慢一點跟隨
                    Debug.Log($"[NPC] {name}: 在聚光燈內跟隨移動，距離中心: {distanceToCenter:F2}");
                }
                else
                {
                    // 很接近中心，保持輕微跟隨
                    Vector2 followDirection = (targetSpotLight.transform.position - transform.position).normalized;
                    rb.linearVelocity = followDirection * (moveSpeed * 0.3f); // 更慢的跟隨速度
                    Debug.Log($"[NPC] {name}: 在聚光燈中心附近，輕微跟隨");
                }
            }
            else
            {
                // 還沒到達聚光燈，全速移動
                Debug.Log($"[NPC] {name}: 移動到聚光燈中");
            }
        }
        else if (targetSpotLight == null)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.LogWarning($"[NPC] {name}: 沒有找到聚光燈");
        }
    }

    private void UpdateWaitingForTurn()
    {
        // 等待狀態，但如果在聚光燈內需要跟隨移動
        if (rb != null)
        {
            if (isInSpotLight && targetSpotLight != null)
            {
                // 在聚光燈內時，輕微跟隨聚光燈移動
                Vector2 followDirection = (targetSpotLight.transform.position - transform.position).normalized;
                rb.linearVelocity = followDirection * (moveSpeed * 0.2f); // 很慢的跟隨速度
                Debug.Log($"[NPC] {name}: 等待狀態中跟隨聚光燈移動");
            }
            else
            {
                // 不在聚光燈內，完全停止
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void UpdateHuntingTarget()
    {
        if (huntingTarget != null && canMove && !isDashing && !isKnocked)
        {
            Vector2 direction = (huntingTarget.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // 檢查是否接近目標，可以使用技能
            float distance = Vector2.Distance(transform.position, huntingTarget.transform.position);
            if (distance < 2f && canDash)
            {
                UseSkill(); // 使用衝刺技能
            }
        }
        else if (huntingTarget == null)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.LogWarning($"[NPC] {name}: 沒有設定狩獵目標");
        }
    }

    // 提供設定目標的公開方法
    public void SetTargetTable(Table table)
    {
        targetTable = table;
        Debug.Log($"[NPC] {name}: 設定目標桌子 {(table != null ? table.name : "null")}");
    }

    public void SetHuntingTarget(MemberBase target)
    {
        huntingTarget = target;
        Debug.Log($"[NPC] {name}: 設定狩獵目標 {(target != null ? target.name : "null")}");
    }

    public override void OnCheckPoint(GameColor color)
    {
        if (color != gameColor)
        {
            Debug.Log($"[NPC] {name}: 顏色不匹配，死亡");
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
        ChangeState(NPCState.SearchingTable);
        Debug.Log($"[NPC] {name}: 重置狀態");
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
                Debug.Log("NPC Color Changed to: " + gameColor.ToString());
                return;
            }
        }
    }

    public void ChangeState(NPCState newState)
    {
        Debug.Log($"[NPC State] {name}: 狀態變更 {NPCState} -> {newState}");
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
        Debug.Log($"[NPC] {name}: 進入搜尋桌子狀態");
        // 清除之前的目標
        // 可以在這裡添加搜尋邏輯或等待外部指令
    }

    private void OnEnterMovingToTable()
    {
        Debug.Log($"[NPC] {name}: 進入移動到桌子狀態");
        // 開始移動到指定的桌子
        // 外部需要設定目標桌子
    }

    private void OnEnterMovingToSpotLight()
    {
        Debug.Log($"[NPC] {name}: 進入移動到聚光燈狀態");

        // 確保有聚光燈目標
        if (targetSpotLight == null)
        {
            targetSpotLight = FindFirstObjectByType<SpotLight>();
            Debug.Log($"[NPC] {name}: 重新尋找聚光燈目標");
        }

        // 清除桌子目標，專注於聚光燈
        targetTable = null;

        // 確認當前顏色
        Debug.Log($"[NPC] {name}: 當前顏色 {gameColor}，開始移動到聚光燈");

        // 不管是否已經在聚光燈內，都保持在這個狀態以便跟隨聚光燈移動
        // UpdateMovingToSpotLight 方法會處理跟隨邏輯
        if (isInSpotLight)
        {
            Debug.Log($"[NPC] {name}: 已經在聚光燈內，將跟隨聚光燈移動");
        }
        else
        {
            Debug.Log($"[NPC] {name}: 開始移動到聚光燈");
        }
    }

    private void OnEnterWaitingForTurn()
    {
        Debug.Log($"[NPC] {name}: 進入等待狀態");
        // 停止移動，等待下一輪
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnEnterHuntingTarget()
    {
        Debug.Log($"[NPC] {name}: 進入狩獵狀態");
        // 開始狩獵模式
        // 外部需要設定狩獵目標
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Table>(out Table table))
        {
            if (table.HasGlass)
            {
                GameColor color = table.GetGlass();
                gameColor = color; // 設定NPC的顏色
                UpdateColor(color);
            }
        }
        if (collision.TryGetComponent<MemberBase>(out MemberBase member))
        {
            if (member != this)
            {
                Knock(member.transform); // 修正：應該擊退對方
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
public enum NPCState
{
    SearchingTable,    // 尋找目標顏色的桌子
    MovingToTable,     // 移動到桌子
    MovingToSpotLight, // 移動到聚光燈
    WaitingForTurn,    // 等待下一輪
    HuntingTarget      // 狩獵模式：尋找有顏色的目標進行衝撞
}
