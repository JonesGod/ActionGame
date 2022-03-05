using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

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
    public FSMBase monsterObserver;
    public GameObject testNewPosition;
    public GameObject testNewPosition2;
    //播放喝水particle
    public ParticleSystem healHpEffect;
    public ParticleSystem healMpEffect;        
    public GameObject effectPosition;

    public PlayableDirector endingTimeline;
    public AtmosphericFogRenderFeature test;

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.P))
        {
            GetPlayer().transform.position = new Vector3(testNewPosition.transform.position.x, testNewPosition.transform.position.y, testNewPosition.transform.position.z);
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            GetPlayer().transform.position = new Vector3(testNewPosition2.transform.position.x, testNewPosition2.transform.position.y, testNewPosition2.transform.position.z);
        }
        
    }
    private void Awake()
    {
        s_Instance = this;
        allMonster = GameObject.FindGameObjectsWithTag("Monster");
        player = GetPlayer().GetComponent<PlayerControl>();
        //observer = new Observer();
        for(int i = 0; i < allMonster.Length; i++)
        {            
            player.Subscribe(allMonster[i].GetComponent<FSMBase>()); 
        }
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
    public void PlayParticleSystem(ParticleSystem particle)
    {
        Instantiate(particle, effectPosition.transform.position, Quaternion.identity);
    }
    public void PlayEnding()
    {
        StartCoroutine(PlayEndingTimeLine());
    }
    IEnumerator PlayEndingTimeLine()
    {
        yield return new WaitForSeconds(6.0f);
        endingTimeline.Play();
        PlayerInput.Instance.isPlayingTimeline = true;
        yield return new WaitForSeconds((float) endingTimeline.duration);
        PlayerInput.Instance.isPlayingTimeline = false;        
        //回到主選單場景        
        test.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
