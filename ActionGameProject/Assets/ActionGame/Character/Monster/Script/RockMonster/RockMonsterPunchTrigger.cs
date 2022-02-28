using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterPunchTrigger : MonoBehaviour
{
    PlayerControl player;
    public int damageAmount;

    void Start()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
    }
    private void OnTriggerEnter(Collider other) 
    {      
        if(other.transform.name == "NewLink")
        {         
            //呼叫玩家腳本的受傷function
            player.PlayerHurt(damageAmount);
        }
    }
}
