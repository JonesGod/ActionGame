using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterCollider : MonoBehaviour
{
    public BoxCollider characterCollider;
    public BoxCollider characterBlockCollider;
    public BoxCollider characterBlockCollider2;

    void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterBlockCollider, true); 
        Physics.IgnoreCollision(characterCollider, characterBlockCollider2, true);        
    }       
}
