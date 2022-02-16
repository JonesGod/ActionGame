using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EamyStats : MonoBehaviour
    {
        public int hpLevel = 10;
        public int maxHp;
        public int currentHp;

        public bool isBoss;

        //Animation animation;

        private void Awake()
        {
            //animation = GetComponentInChildren<Animation>();
            maxHp = SetMaxHpFromHpLevel();
            currentHp = maxHp;

        }

        private void Start()
        {
            if (!isBoss)
            {
                //enemyhp.Setmaxhp(maxhp);
            }
        }

        private int SetMaxHpFromHpLevel()
        {
            maxHp = hpLevel * 10;
            return maxHp;
        }

        public void TakeDamge(int damge)
        {
            currentHp = currentHp - damge;

            //animation.Play("dam01");

            //if (currentHp <= 0)
            //{
            //    currentHp = 0;
            //    animation.Play("dam01");
            }
        }

    }
