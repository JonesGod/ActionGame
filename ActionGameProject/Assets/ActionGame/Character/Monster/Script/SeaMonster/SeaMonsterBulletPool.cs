using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterBulletPool : MonoBehaviour
{
    public static SeaMonsterBulletPool Instance
    {
        get { return s_Instance; }
    }
    protected static SeaMonsterBulletPool s_Instance;
    public GameObject bullet;
    public int initailSize = 20;
    private Queue<GameObject> m_pool = new Queue<GameObject>();
    void Awake()
    {
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
            reuse.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(bullet) as GameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
        }
    }
    public void Recovery(GameObject recovery)
    {
        m_pool.Enqueue(recovery);
        recovery.SetActive(false);
    }
}
