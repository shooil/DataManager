using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(MeshManager))]
public class AvatarDataManagerUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MeshManager Analyzer = target as MeshManager;
        if (GUILayout.Button("TestMeshDataTable")){TestMeshDataTable(Analyzer.Avatar);}
        if (GUILayout.Button("TestBoneDataTable")){TestBoneDataTable(Analyzer.Avatar,Analyzer.Armature);}
        if (GUILayout.Button("ReadCSV")){TestReadCSV();}
        if (GUILayout.Button("TestSmartCostumeTbl")){TestSmartCostumeDataTable(Analyzer.SmartCostume);}
    }

    public void TestMeshDataTable(GameObject Avatar){
        AvatarMeshTbl Tbl = new AvatarMeshTbl();
        Tbl.WriteAvatarMeshDataTbl(Avatar);
        foreach(var row in Tbl.Tbl){
            Debug.Log("Avatar:"+row.Avatar.name+",MeshObjectName:"+row.MeshGameObject.name+",MeshType:"+row.MeshType+",Depth"+row.Depth);
        }
    }

    public void TestBoneDataTable(GameObject Avatar, GameObject Armature){
        Debug.Log("Start");
        AvatarBoneTbl BoneTbl = new AvatarBoneTbl();
        BoneTbl.WriteAvatarBoneDataTbl(Avatar,Armature);
        Debug.Log("End");
    }

    public void TestReadCSV(){
        string path = "./Assets/0ajisaku/Editor/HumanoidRule.csv";
        var tbl = HumanoidBoneRefTbl.Parse(path);
        foreach(var row in tbl){
            Debug.Log("HumanoidName:"+row.HumanoidName+",ExpressionCount:"+row.Expressions.Count);
        }
    }

    public void TestSmartCostumeDataTable(GameObject SmartCostume){
        SmartCostumeTbl Tbl = new SmartCostumeTbl();
        Tbl.WriteSmartCostumeDataTbl(SmartCostume);
    }
}