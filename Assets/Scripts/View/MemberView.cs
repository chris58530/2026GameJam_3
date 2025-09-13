using System;
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

    public void CheckAllMembersPoint(GameColor color, bool isInSpotLight, MemberBase member)
    {
        memberBase = GetComponent<MemberBase>();
        if (memberBase != null)
        {
            memberBase.checkPointAction += (c, isInLight, m) =>
            {
                if (c == color && isInLight)
                {
                    Debug.Log("CheckPoint Success");
                }
                else
                {
                    Debug.Log("CheckPoint Failed" + m.name);
                    m.Die();
                }
            };
        }


    }
    public void Init(Action onGameOver)
    {
        player.Init(onGameOver);
    }



}
