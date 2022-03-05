using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterAttack : MonoBehaviour
{
    public GameObject bulletSpawnPosition;
    public SeaMonsterBulletPool monsterBulletPool;

    public GameObject screamAudio;
    private AudioSource screamSouce;

    private void Start()
    {
        screamSouce = screamAudio.GetComponent<AudioSource>();
    }

    public void ShootBullet()
    {
        monsterBulletPool.ReUse(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.rotation);       
    }
    void PlayScreamAudio()
    {
        screamSouce.Play();
    }
}
