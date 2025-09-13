using System;
using UnityEngine;

public class NPC : MemberBase
{
    public Action onDie;
    public void Init(Action action)
    {
        onDie = action;
    }
}
