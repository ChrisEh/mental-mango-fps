using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util 
{
    public static void SetLayerRecursively(GameObject obj, int newLayerIndex)
    {
        if (obj == null)
            return;

        obj.layer = newLayerIndex;

        foreach  (Transform child in obj.transform)
        {
            if (child == null)
                continue;

            SetLayerRecursively(child.gameObject, newLayerIndex);
        }
    }
}
