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
        player.removeMemberCallback = RemoveMember;
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
            npc.removeMemberCallback = RemoveMember;
        }
    }

    public int GetMemberCount()
    {
        return memberBases.Count;
    }

    public void CheckAllMembersPoint(GameColor color)
    {
        // 創建一個副本來避免集合修改異常
        var membersToCheck = new List<MemberBase>(memberBases);

        foreach (var member in membersToCheck)
        {
            if (member != null)
            {
                member.OnCheckPoint(color);
            }
        }
    }

    public void AllMembersWhite()
    {
        // 創建一個副本來避免集合修改異常
        var membersToUpdate = new List<MemberBase>(memberBases);

        foreach (var member in membersToUpdate)
        {
            if (member != null)
            {
                member.ChangeColor();
            }
        }
    }

    public void RemoveMember(MemberBase member)
    {
        if (memberBases.Contains(member))
        {
            memberBases.Remove(member);
        }
    }

    public void ResetView()
    {
        // 創建一個副本來避免集合修改異常
        var membersToReset = new List<MemberBase>(memberBases);

        foreach (var member in membersToReset)
        {
            if (member != null)
            {
                member.ResetView();
            }
        }
        memberBases.Clear();
    }

}
