using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EvColliderBossFight : MonoBehaviour
    {
        WorldEvManager worldEvManager;

        private void Awake()
        {
            worldEvManager = FindObjectOfType<WorldEvManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                worldEvManager.ActivateBossFight();
            }
        }
    }
}