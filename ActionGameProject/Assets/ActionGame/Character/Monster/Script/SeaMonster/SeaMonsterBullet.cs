using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterBullet : MonoBehaviour
{
    PlayerControl player;
    public float speed;
    void OnEnable()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other) 
    {      
        Debug.Log(other.name);
        if(other.transform.name == "NewLink")
        {
            Debug.Log("子彈打到玩家");            
            //呼叫玩家腳本的受傷function
            player.PlayerHurt(20);            
            GameObject.Find("SeaMonsterBulletPool").GetComponent<SeaMonsterBulletPool>().Recovery(this.gameObject);
        }
        else if(other.tag == "Monster")
        {
        }
        else if(other.transform.name == "Arrow(Clone)")
        {
        }
        else
        {
            GameObject.Find("SeaMonsterBulletPool").GetComponent<SeaMonsterBulletPool>().Recovery(this.gameObject);
        }
    }
}
