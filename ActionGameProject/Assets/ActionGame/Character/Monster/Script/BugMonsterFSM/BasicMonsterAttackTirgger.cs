using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMonsterAttackTirgger : MonoBehaviour
{
    PlayerControl player;
    public ParticleSystem hitEffect;     

    void Start()
    {
        player = GameManager.Instance.GetPlayer().GetComponent<PlayerControl>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "NewLink")
        {
            Debug.Log("打中玩家");            
            //呼叫玩家腳本的受傷function
            player.PlayerHurt(30);
            var collisionPoint = other.ClosestPoint(transform.position);
            PlayParticleSystem(hitEffect, collisionPoint);
        }
    }
    public void PlayParticleSystem(ParticleSystem particle, Vector3 hitPosition)
    {
        Instantiate(particle, hitPosition, Quaternion.identity);
    }
}
