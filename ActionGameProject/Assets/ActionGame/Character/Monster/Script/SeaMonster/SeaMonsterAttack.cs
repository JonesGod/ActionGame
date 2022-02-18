using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterAttack : MonoBehaviour
{
    public GameObject bulletSpawnPosition;
    public SeaMonsterBulletPool monsterBulletPool;
    public void ShootBullet()
    {
        Debug.Log("shoot");
        monsterBulletPool.ReUse(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.rotation);       
    }
}
