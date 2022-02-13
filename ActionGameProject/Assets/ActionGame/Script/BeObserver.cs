using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BeObserver
{
    void NotifyDead();
    void NotifyLife();
}
