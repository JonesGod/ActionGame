using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("MyPostProcessing/ZoomBlurVolume")]
    public class ZoomBlurVolume : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter enableEffect = new BoolParameter(false);
        public Vector2Parameter centerPosition = new Vector2Parameter(new Vector2(0.5f, 0.5f));
        public ClampedFloatParameter blurIntensity = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
        public ClampedIntParameter iteration = new ClampedIntParameter(5, 1, 16);
        public ClampedFloatParameter blurDistance = new ClampedFloatParameter(3.0f, 0.0f, 16.0f);
        public ClampedIntParameter downSample = new ClampedIntParameter(1, 1, 8);

        public bool IsActive() => enableEffect.value;
        public bool IsTileCompatible() => false;
    }
}