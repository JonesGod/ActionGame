using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EvColliderDragonBossFight : MonoBehaviour
    {
        WorldEvManager worldEvManager;

        private void Awake()
        {
            worldEvManager = FindObjectOfType<WorldEvManager>();
        }


        //��������tag��Ĳ�o
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //worldEvManager.ActivateBossFight();
                worldEvManager.ActivateDragonBossFight();
            }
        }
    }
}