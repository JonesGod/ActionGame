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

    private void Awake()
    {
        s_Instance = this;
    }
    // Use this for initialization
    void Start () 
    {
        
    }    
    void Update () 
    {
		
	}
    public GameObject GetPlayer()
    {
        return m_Player;
    }
}
