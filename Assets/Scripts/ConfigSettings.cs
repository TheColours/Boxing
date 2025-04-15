using UnityEngine;


public class ConfigSettings : MonoBehaviour
{
    void Awake()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Mathf.Max(60, (int)Screen.currentResolution.refreshRateRatio.value); 
#endif
    }
}
