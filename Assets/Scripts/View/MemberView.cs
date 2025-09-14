using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class MemberView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private int npcCount;

    [Header("Player")]
    [SerializeField] private Player player;

    [Header("NPC")]
    [SerializeField] private NPC npcPrefab;
    [SerializeField] private Vector2 spawnRangeX;
    [SerializeField] private Vector2 spawnRangeY;
    [SerializeField] private MemberBase memberBase;

    private List<MemberBase> memberBases = new List<MemberBase>();

    public void Init(Action onGameOver)
    {
        player.Init(onGameOver);
        memberBases.Add(player);

    }

    public void StartNPCSpawn(Action onMemberDie)
    {
        //spawnNPC
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(spawnRangeX.x, spawnRangeX.y), UnityEngine.Random.Range(spawnRangeY.x, spawnRangeY.y), 0);
            NPC npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
            npc.transform.SetParent(transform);
            npc.Init(onMemberDie);
            memberBases.Add(npc);
        }
    }

    public void CheckAllMembersPoint(GameColor color)
    {
        foreach (var member in memberBases)
        {
            member.OnCheckPoint(color);
        }

    }

    public void ResetView()
    {

    }

}
