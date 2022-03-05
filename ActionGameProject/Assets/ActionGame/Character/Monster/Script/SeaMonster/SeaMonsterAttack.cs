using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterAttack : MonoBehaviour
{
    public GameObject bulletSpawnPosition;
    public SeaMonsterBulletPool monsterBulletPool;

    public GameObject screamAudio;
    private AudioSource screamSouce;
    public GameObject appearAudio;
    private AudioSource appearSouce;

    private void Start()
    {
        screamSouce = screamAudio.GetComponent<AudioSource>();
        appearSouce = appearAudio.GetComponent<AudioSource>();
    }

    public void ShootBullet()
    {
        monsterBulletPool.ReUse(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.rotation);       
    }
    void PlayScreamAudio()
    {
        screamSouce.Play();
        //appearSouce.Stop();
    }
    void PlayAppearAudio()
    {
        appearSouce.Play();
    }
}
