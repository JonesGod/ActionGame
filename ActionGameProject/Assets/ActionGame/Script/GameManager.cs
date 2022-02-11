using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{

    public static GameManager Instance
    {
        get { return s_Instance; }
    }
    protected static GameManager s_Instance;
    public GameObject m_Player;
    public GameObject[] allMonster;
    private PlayerControl player;
    private Relive relive;//玩家死亡時的觀察者

    private void Awake()
    {
        s_Instance = this;
        allMonster = GameObject.FindGameObjectsWithTag("Monster");
        player = GetPlayer().GetComponent<PlayerControl>();
        relive = new Relive();
        player.Subscribe(relive);
    }

    public GameObject GetPlayer()
    {
        return m_Player;
    }
    public void ReLife()
    {
        //先把畫面關燈

        //把玩家位置改到重生點並補滿血
        //玩家狀態改成活著

        //畫面燈亮
    }
}
