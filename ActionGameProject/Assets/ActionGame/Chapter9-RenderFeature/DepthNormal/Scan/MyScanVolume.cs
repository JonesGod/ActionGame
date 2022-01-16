using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("MyPostProcessing/MyScanVolume")]
    public class MyScanVolume : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter enableEffect = new BoolParameter(false);
        [HideInInspector]public MinFloatParameter scanDistance = new MinFloatParameter(10.0f, 0.0f, true);        
        [HideInInspector]public Vector3Parameter scannerPos = new Vector3Parameter(new Vector3(0, 0, 0), true);
        public MinFloatParameter scanWidth = new MinFloatParameter(0.0f, 0.0f);
        public bool IsActive() => enableEffect.value;
        public bool IsTileCompatible() => false;
    }
}
