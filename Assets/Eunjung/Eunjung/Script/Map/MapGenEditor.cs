using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Eunjung
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MapGenerator myGenerator = (MapGenerator)target;
            if (GUILayout.Button("¸Ê »ý¼º"))
            {
                myGenerator.BuildGenerator();
            }
        }
    }
}
