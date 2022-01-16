using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace UnityEngine.Rendering.Universal
{    
    public class VolumeController : MonoBehaviour
    {
        public Volume volume;
        ZoomBlurVolume zoomBlur;

        MyScanVolume myScan;
        public Camera scanCamera;
        public Transform scannerOrigin;
        bool isScanning;

        void Start()
        {
            if(volume.profile.TryGet<ZoomBlurVolume>(out zoomBlur))
            {
                zoomBlur.blurIntensity.value = 0;
            } 

            if(volume.profile.TryGet<MyScanVolume>(out myScan))
            {
                myScan.scannerPos.value = scannerOrigin.position;
                Debug.Log(myScan.scannerPos.value);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(volume.profile.TryGet<ZoomBlurVolume>(out zoomBlur))
                {
                    zoomBlur.blurIntensity.value = 1;
                }
            }
            if(Input.GetKeyDown(KeyCode.B))
            {
                if(volume.profile.TryGet<ZoomBlurVolume>(out zoomBlur))
                {
                    zoomBlur.blurIntensity.value = 0;
                }
            }

            if (isScanning)
            {
                myScan.scanDistance.value += Time.deltaTime * 25;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                isScanning = true;
                myScan.scanDistance.value = 0;
                myScan.scannerPos.value = scannerOrigin.position;
                Debug.Log(myScan.scannerPos.value);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = scanCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    isScanning = true;
                    myScan.scanDistance.value = 0;
                    myScan.scannerPos.value = hit.point;
                    Debug.Log(myScan.scannerPos.value);
                }
            }
        }
    }
}    
