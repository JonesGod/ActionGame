using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relive
{
    private PlayerControl player =GameManager.Instance.m_Player.GetComponent<PlayerControl>();
    private float statetime;
    private Vector3 livePosition;
   
    public void DeadProcess()
    {
        Debug.Log("DeadProcess");
        //    screenflap black
        //    screenflap black  end
        player.transform.position = livePosition;
        player.playerHp = player.playerMaxHp;
        //screenflap normal
        //screenflap normal end
    }
}
