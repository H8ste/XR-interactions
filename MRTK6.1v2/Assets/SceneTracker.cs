using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using TMPro;
using UnityEngine;

public class SceneTracker : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private TextMeshPro indexTracker;

    [SerializeField]
    private TextMeshPro sceneName;
    void Start()
    {
        //indexTracker = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void UpdateSceneTrackingText()
    {
        if (indexTracker == null || sceneName == null)
        {
            Debug.LogError("Not all properties have been set on this script");
            return;
        }
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        if (GetActiveSceneIndex(sceneSystem.ContentSceneNames, sceneSystem, out int index))
        {
            indexTracker.SetText((index + 1) + "/" + sceneSystem.ContentSceneNames.Length);
            sceneName.SetText(sceneSystem.ContentSceneNames[index]);
        }
        else
        {
            indexTracker.SetText("");
            sceneName.SetText("Scene Selector");
        }
    }

    private bool GetActiveSceneIndex(string[] allScenes, IMixedRealitySceneSystem sceneSystem, out int output)
    {
        for (int idx = 0; idx < allScenes.Length; idx++)
        {
            if (sceneSystem.IsContentLoaded(allScenes[idx]))
            {
                output = idx;
                return true;
            }
        }

        output = -1;
        return false;
    }
}
