using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMonsterBulletPool : MonoBehaviour
{
    public GameObject bullet;
    public int initailSize = 10;
    private Queue<GameObject> m_pool = new Queue<GameObject>();
    PlayerControl player;    
    //public GameObject monsterShootPoint;
    void Awake()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
        for(int i = 0; i < initailSize; i++)
        {
            GameObject go = Instantiate(bullet) as GameObject;
            m_pool.Enqueue(go);
            go.SetActive(false);
        }        
    }
    void Update()
    {
    }
    public void ReUse(Vector3 position, Quaternion rotation)
    {
        if(m_pool.Count > 0)
        {
            GameObject reuse = m_pool.Dequeue();
            reuse.transform.position = position;
            reuse.transform.rotation = rotation;
            reuse.transform.forward = (player.transform.position + new Vector3(0, 1.0f, 0)) - position; 
            reuse.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(bullet) as GameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.forward = player.transform.position - position; 
        }
    }
    public void Recovery(GameObject recovery)
    {
        m_pool.Enqueue(recovery);
        recovery.SetActive(false);
    }
}
