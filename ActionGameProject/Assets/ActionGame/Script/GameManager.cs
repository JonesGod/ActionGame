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
<<<<<<< Updated upstream
    private Observer observer;//玩家死亡時的觀察者
=======
    private Relive relive;//玩家死亡時的觀察者
    public GameObject teleportTransform;
>>>>>>> Stashed changes

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Teleport();
        }
    }
    private void Awake()
    {
        s_Instance = this;
        allMonster = GameObject.FindGameObjectsWithTag("Monster");
        player = GetPlayer().GetComponent<PlayerControl>();
        observer = new Observer();
        player.Subscribe(observer);
    }

    public GameObject GetPlayer()
    {
        return m_Player;
    }
    private void Teleport()
    {
        GetPlayer().transform.position = new Vector3(teleportTransform.transform.position.x, teleportTransform.transform.position.y, teleportTransform.transform.position.z);
    }
    public void ReLife()
    {
        //先把畫面關燈

        //把玩家位置改到重生點並補滿血
        //玩家狀態改成活著

        //畫面燈亮
    }
}
