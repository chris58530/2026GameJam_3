using System;
using System.Diagnostics.Contracts;
using UnityEngine;

public class MemberView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private int npcCount;

    [Header("NPC")]
    [SerializeField] private NPC npcPrefab;
    [SerializeField] private Vector2 spawnRangeX;
    [SerializeField] private Vector2 spawnRangeY;
    [SerializeField] private MemberBase memberBase;


    public void StartNPCSpawn(Action onMemberDie)
    {
        //spawnNPC
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(spawnRangeX.x, spawnRangeX.y), UnityEngine.Random.Range(spawnRangeY.x, spawnRangeY.y), 0);
            NPC npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
            npc.transform.SetParent(transform);
            npc.Init(onMemberDie);
        }
    }



}
