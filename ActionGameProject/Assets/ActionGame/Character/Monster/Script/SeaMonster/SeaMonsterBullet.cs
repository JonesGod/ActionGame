using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterBullet : MonoBehaviour
{
    PlayerControl player;
    SeaMonsterBulletPool bulletPool;
    public float speed;
    private float lifeTime;
    void OnEnable()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
        bulletPool = GameObject.Find("SeaMonsterBulletPool").GetComponent<SeaMonsterBulletPool>();
        lifeTime = 0.0f;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTime += Time.deltaTime;
        if(lifeTime >= 5.0f)
        {
            bulletPool.Recovery(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other) 
    {      
        Debug.Log(other.name);
        if(other.transform.name == "NewLink")
        {
            Debug.Log("子彈打到玩家");            
            //呼叫玩家腳本的受傷function
            player.PlayerHurt(20);            
            bulletPool.Recovery(this.gameObject);
        }
        else if(other.tag == "Monster")
        {
        }
        else if(other.transform.name == "Arrow(Clone)")
        {
        }
        else
        {
            bulletPool.Recovery(this.gameObject);
        }
    }
}
