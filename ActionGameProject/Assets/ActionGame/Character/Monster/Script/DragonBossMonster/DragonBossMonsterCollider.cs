using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossMonsterCollider : MonoBehaviour
{
    public BoxCollider characterCollider;
    public BoxCollider characterBlockCollider;
    void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterBlockCollider, true);        
    }
}
