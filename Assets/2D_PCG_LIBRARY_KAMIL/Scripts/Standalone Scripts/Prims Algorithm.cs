//Created by: Kamil Woloszyn
//In the Years 2024-2025

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrimsAlgorithm : MonoBehaviour
{
    
}


#if UNITY_EDITOR
[CustomEditor(typeof(PrimsAlgorithm))]
public class PrimsAlgorithmCustomInspector : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        PrimsAlgorithm pathfind = (PrimsAlgorithm)target;
        if (DrawDefaultInspector())
        {
            //Include Function call here to auto update values in this inspector

        }

    }
}
#endif
