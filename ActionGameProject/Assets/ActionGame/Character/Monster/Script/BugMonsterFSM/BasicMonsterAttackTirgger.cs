using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMonsterAttackTirgger : MonoBehaviour
{
    //PlayerControl player;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "NewLink")
        {
            Debug.Log("打中玩家");            
            //呼叫玩家腳本的受傷function
            //player.PlayerHurt(30);
        }
    }
}
