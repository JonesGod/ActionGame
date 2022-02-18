using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterBullet : MonoBehaviour
{
    public SeaMonsterBulletPool monsterBulletPool;
    Vector3 targetDirection;  
    PlayerControl player;
    public float speed = 10.0f;

    void Start()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
        targetDirection = player.transform.position - this.transform.position; 
    }
    void Update()
    {
        transform.Translate(targetDirection * speed * Time.deltaTime); 
    }
    private void OnTriggerEnter(Collider other) 
    {      
        if(other.transform.name == "NewLink")
        {
            Debug.Log("子彈打到玩家");            
            //呼叫玩家腳本的受傷function
            player.PlayerHurt(20);
        }
        if(other.tag == "Monster")
        {
            return;
        }
        GameObject.Find("SeaMonsterBulletPool").GetComponent<SeaMonsterBulletPool>().Recovery(this.gameObject);
    }
}
