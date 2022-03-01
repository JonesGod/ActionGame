using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterBullet : MonoBehaviour
{
    PlayerControl player;
    RockMonsterBulletPool bulletPool;
    public float speed;
    private float lifeTime;
    public ParticleSystem hitEffect;     
    void OnEnable()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
        transform.LookAt(player.transform.position);
        StartCoroutine(Wait());
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
            player.PlayerHurt(30);                        
            var collisionPoint = other.ClosestPoint(transform.position);
            PlayParticleSystem(hitEffect, collisionPoint);

        }
        else if(other.tag == "Monster")
        {
        }
        else if(other.transform.name == "CoreHitBox")
        {
        }
        else if(other.transform.name == "ShieldHitBox")
        {
        }
        else if(other.transform.name == "Arrow(Clone)")
        {
        }
        else
        {
            var collisionPoint = other.ClosestPoint(transform.position);
            PlayParticleSystem(hitEffect, collisionPoint);
        }
    }
    public void PlayParticleSystem(ParticleSystem particle, Vector3 hitPosition)
    {
        Instantiate(particle, hitPosition, Quaternion.identity);
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3.0f);
    }
}
