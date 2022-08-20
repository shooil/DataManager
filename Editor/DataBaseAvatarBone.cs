using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>Define</summary>
public struct AvatarBoneTbl{
    /// <value>Table to store all of bone related data</value>
    public List<AvatarBonePropertiesRow> Tbl;

    public void WriteAvatarBoneDataTbl(GameObject Avatar, GameObject Armature){
        this.Tbl = new List<AvatarBonePropertiesRow>();
        this._addBoneRecords(Avatar,Armature);
    }

    private void _addBoneRecords(GameObject Avatar, GameObject Armature){
        List<AvatarBonePropertiesRow> TempTbl = new List<AvatarBonePropertiesRow>();
        foreach(var Bone in Armature.GetComponentsInChildren<Transform>(includeInactive:true)){
            AvatarBonePropertiesRow row = new AvatarBonePropertiesRow();
            row.Avatar = Avatar;
            row.Bone = Bone.gameObject;
            row.ParentObjects = GameObjectQuery.GetRelativeParentsQuery(Armature,row.Bone);
            row.Parent = row.Bone.transform.parent.gameObject;
            row.RelativeCoordinate = row.Bone.transform.position;
            row.Depth = row.ParentObjects.Count;
            row.HumanoidName = _catHumanoid(Avatar,Armature,row.Bone);
            // Excepted for heavy data processing
            row.RelatedSkinnedMeshs = _listupSkinnedMeshs(Avatar,row.Bone);
            TempTbl.Add(row);
        }
        this.Tbl.AddRange(TempTbl);
    }

    /// <summary>Mainly for pairing avatar original bone to humanoid bone</summary>
    /// <param name="Avatar">Avatar GameObject for getting Humanoid Bone set up</param>
    /// <param name="Armature">Armature GameObejct to calculate Depth of Target</param>
    /// <param name="Target">Bone GameObject for pairing to Humanoid Bone name</param>
    private string _catHumanoid(GameObject Avatar, GameObject Armature, GameObject Target){
        var HumanoidName = "";
        var anim = Avatar.GetComponent<Animator>();
        if(anim == null || anim.avatar == null){return "";}
        var DescTbl = anim.avatar.humanDescription.human.ToList();
        var HumanoidRefTbl = HumanoidBoneRefTbl.Parse("");
        var Query = from Datarow in DescTbl
                    join Refrow in HumanoidRefTbl
                    on Datarow.humanName equals Refrow.HumanoidName
                    where Datarow.boneName == Target.name
                    where Refrow.Depth == GameObjectQuery.GetRelativeParentsQuery(Armature,Target).Count
                    select new{HumanoidName = Datarow.humanName, HumanDepth = Refrow.Depth, BoneName = Datarow.boneName};
        if(Query.ToList().Count != 0){
            HumanoidName = Query.ToList()[0].HumanoidName;
        }else{
            HumanoidName = "";
        }
        return HumanoidName;
    }

    /// <summary>Method to pairing SkinnedMeshRenderers GameObject to bone</summary>
    /// <param name="Avatar">Avatar GameObject for finding SkinnedMeshRenderers</param>
    /// <param name="Target">Bone to find related SkinnedMeshRenderers</param>
    private List<GameObject> _listupSkinnedMeshs(GameObject Avatar, GameObject Target){
        List<GameObject> RelatedSkinnedMeshs = new List<GameObject>();
        foreach(var Skinned in Avatar.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive:true)){
            try{
                foreach(var Bone in Skinned.bones){
                    if(Bone == Target){
                        RelatedSkinnedMeshs.Add(Skinned.gameObject);
                    }
                }
            }catch(NullReferenceException){}
        }
        return RelatedSkinnedMeshs;
    }
}

/// <summary>Define Bone related row data</summary>
public class AvatarBonePropertiesRow:IBoneData{
    /// <value>Property to hold avatar GameObject</value>
    public GameObject Avatar;

    /// <value>Property to hold bone GameObject</value>
    public GameObject Bone;

    /// <value>Property to hold all parent GameObjects of bone</value> 
    public List<GameObject> ParentObjects;

    /// <value>Property to hold parent GameObject</value>
    public GameObject Parent;

    /// <value>Property to hold depth of bone
    public int Depth;

    /// <value>Property to pairing bone GameObject to Humanoid Bone name</value>
    public string HumanoidName;

    /// <value>Property to hold relative coordinate from parent</value>
    public Vector3 RelativeCoordinate;

    /// <value>Property to hold SkinnedMeshRenderers related to Bone</value>
    public List<GameObject> RelatedSkinnedMeshs;

    public GameObject GetBone(){
        return this.Bone;
    }

    public int GetDepth(){
        return this.Depth;
    }

    public Vector3 GetRelativePos(){
        return this.RelativeCoordinate;
    }
}