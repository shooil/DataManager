using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DataManager
{
    [CustomEditor(typeof(DataManagerMesh))]
    public class DataManagerMeshUI : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DataManagerMesh mesh = target as DataManagerMesh;
            if (GUILayout.Button("Initialize")) { this.Initialize(mesh.Avatar); }
        }
        public void Initialize(GameObject Avatar)
        {
            AvatarMeshTbl Tbl = new AvatarMeshTbl();
            Tbl.WriteAvatarMeshDataTbl(Avatar);
        }
    }
}