using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumState//列出怪物所有的狀態
    {
        NONE = -1,
        Idle,
        Chase,
        Strafe,
        NormalAttack,
        Hurt,
        Dead
    }
