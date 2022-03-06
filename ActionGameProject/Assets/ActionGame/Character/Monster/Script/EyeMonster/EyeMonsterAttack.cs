using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMonsterAttack : MonoBehaviour
{
    public GameObject bulletSpawnPosition;
    public EyeMonsterBulletPool monsterBulletPool;

    ///Audio
    public GameObject shootAudio;
    private AudioSource shootSource;

    private void Start()
    {
        shootSource = shootAudio.GetComponent<AudioSource>();
    }
    public void ShootBullet()
    {
        shootSource.Play();
        monsterBulletPool.ReUse(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.rotation);       
    }
}
