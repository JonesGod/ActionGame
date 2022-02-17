using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterAttack : MonoBehaviour
{    
    GameObject player;
    void start()
    {
        player = GameManager.Instance.GetPlayer();
    }
    public void ShootBullet()
    {
        Vector3 targetDirection = player.transform.position - this.transform.position;        
    }
}
