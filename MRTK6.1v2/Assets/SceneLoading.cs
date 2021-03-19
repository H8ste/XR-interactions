﻿using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using UnityEngine;

public class SceneLoading : MonoBehaviour
{
    [SerializeField]
    private SceneTracker sceneTracker;

    public async void LoadScene(bool loadingNext)
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        if (loadingNext)
        {
            Debug.Log("Loading next scene");
            await sceneSystem.LoadNextContent(wrap: true);
        }

        if (!loadingNext)
        {
            Debug.Log("Loading prev scene");
            await sceneSystem.LoadPrevContent(wrap: true);
        }

        if (sceneTracker != null)
        {
            sceneTracker.UpdateSceneTrackingText();
        }
    }
}
