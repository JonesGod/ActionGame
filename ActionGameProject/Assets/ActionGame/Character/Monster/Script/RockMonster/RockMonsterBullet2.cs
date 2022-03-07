using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterBullet2 : MonoBehaviour
{
    PlayerControl player;
    RockMonsterBulletPool bulletPool;
    public float speed;
    private float lifeTime;
    public ParticleSystem hitEffect;
    private bool canFly = false;     
    void OnEnable()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
        transform.LookAt(player.transform.position + new Vector3(0, 2.0f, 0));
        StartCoroutine(Wait());
    }

    void Update()
    {
        if(canFly == true)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }        
    }
    private void OnTriggerEnter(Collider other) 
    {      
        Debug.Log(other.name);
        if(other.name == "NewLink")
        {
            Debug.Log("子彈打到玩家");            
            //呼叫玩家腳本的受傷function
            player.PlayerHurt(30);                        
            var collisionPoint = other.ClosestPoint(transform.position);
            PlayParticleSystem(hitEffect, collisionPoint);
            //Destroy(this.gameObject);
        }
        else if(other.tag == "Monster")
        {
        }
        else if(other.transform.name == "CoreHitBox")
        {
        }
        else if(other.transform.name == "HitBox")
        {
        }
        else if(other.transform.name == "Arrow(Clone)")
        {
        }
        else
        {
            Debug.Log(this.name);
            var collisionPoint = other.ClosestPoint(transform.position);
            PlayParticleSystem(hitEffect, collisionPoint);
            Destroy(this.gameObject);
        }
    }
    public void PlayParticleSystem(ParticleSystem particle, Vector3 hitPosition)
    {
        Instantiate(particle, hitPosition, Quaternion.identity);
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2.0f);
        transform.LookAt(player.transform.position + new Vector3(0, 2.0f, 0));
        canFly = true;
    }
}