using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer
{
    private PlayerControl player = GameManager.Instance.m_Player.GetComponent<PlayerControl>();

    public void DeadProcess()
    {
        if (player.playerCurrnetState == PlayerControl.PlayerState.dead)
        {
            //    screenflap black
            //        screenflap black  end
            player.StarRelive();
            //screenflap normal
            //screenflap normal end
        }
    }
}
