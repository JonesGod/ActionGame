using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class FogWell : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void ActivateFogWell()
        {
            gameObject.SetActive(true);
        }

        public void DeactivteFoWell()
        {
            gameObject.SetActive(false);
        }
    }
