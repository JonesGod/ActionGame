using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMonsterAttack : MonoBehaviour
{
    public GameObject bulletSpawnPosition;
    public EyeMonsterBulletPool monsterBulletPool;
    public void ShootBullet()
    {
        Debug.Log("shoot");
        monsterBulletPool.ReUse(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.rotation);       
    }
}
