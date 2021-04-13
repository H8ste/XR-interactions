using UnityEngine;

public static class Helper
{
    /// <summary>
    /// Returns the first occuring child of provided parent with provided tag
    /// </summary>
    /// <param name="tag">tag to search for on children</param>
    /// <param name="parent">parent in which to search for child objects from</param>
    public static GameObject FindChildWithTag(GameObject root, string tag)
    {
        if (root == null) return null;

        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            if (t.tag == tag) return t.gameObject;
        }

        Debug.LogError("Couldn't find child with tag");
        return null;
    }

    /// <summary>
    /// Maps the provided value to map from its old min and max to its new min and max 
    /// </summary>
    /// <param name="oldMin">old minimum of provided value</param>
    /// <param name="oldMax">old maximum of provided value</param>
    /// <param name="newMin">new minimum of provided value</param>
    /// <param name="newMax">new maximum of provided value</param>
    /// <param name="valueToMap">value to map</param>
    public static float Map(float oldMin, float oldMax, float newMin, float newMax, float valueToMap)
    {
        float OldRange = (oldMax - oldMin);
        float NewRange = (newMax - newMin);
        float mappedValue = (((valueToMap - oldMin) * NewRange) / OldRange) + newMin;

        return (mappedValue);
    }
}