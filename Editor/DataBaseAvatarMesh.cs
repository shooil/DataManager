using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace DataManager
{
    /// <summary>Define table contains SkinnedMeshRenderer related data and MeshRenderer related data</summary>
    public struct AvatarMeshTbl
    {
        /// <value>Table to store all of mesh related data</value>
        public List<AvatarMeshPropertiesRow> Tbl;

        /// <summary>Write mesh related data into table</summary>
        /// <param name="Avatar">Avatar GameObject to make data table</param>
        public void WriteAvatarMeshDataTbl(GameObject Avatar)
        {
            this.Tbl = new List<AvatarMeshPropertiesRow>();
            this._addSkinnedMeshRecords(Avatar);
            this._addMeshRecords(Avatar);
        }

        /// <summary>Write the data about SkinnedMeshRenderer into table</summary>
        /// <param name="Avatar">Avatar GameObject to analyze</param>
        private void _addSkinnedMeshRecords(GameObject Avatar)
        {
            List<AvatarMeshPropertiesRow> TempTbl = new List<AvatarMeshPropertiesRow>();
            foreach (var Skinned in Avatar.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true))
            {
                AvatarMeshPropertiesRow row = new AvatarMeshPropertiesRow();
                row.Avatar = Avatar;
                row.MeshGameObject = Skinned.gameObject;
                row.MeshType = "SkinnedMeshRenderer";
                row.ParentObjectList = GameObjectQuery.GetRelativeParentsQuery(Avatar, Skinned.gameObject);
                row.Parent = Skinned.gameObject.GetComponentInParent<Transform>().gameObject;
                row.Depth = row.ParentObjectList.Count;
                row.Materials = Skinned.sharedMaterials.ToList();

                // Deal with skinned mesh renderer with cloth
                try
                {
                    row.RootBone = Skinned.rootBone.gameObject;
                }
                catch (NullReferenceException) { }

                // Deal with deleted bones in unity
                row.RelatedBones = new List<GameObject>();
                try
                {
                    foreach (var bone in Skinned.bones)
                    {
                        row.RelatedBones.Add(bone.gameObject);
                    }
                }
                catch (NullReferenceException) { }

                Tbl.Add(row);
            }
            this.Tbl.AddRange(TempTbl);
        }

        /// <summary>Write the data about MeshRenderer into table</summary>
        /// <param name="Avatar">Avatar GameObject to analyze</param>
        private void _addMeshRecords(GameObject Avatar)
        {
            List<AvatarMeshPropertiesRow> TempTbl = new List<AvatarMeshPropertiesRow>();
            foreach (var Mesh in Avatar.GetComponentsInChildren<MeshRenderer>(includeInactive: true))
            {
                AvatarMeshPropertiesRow row = new AvatarMeshPropertiesRow();
                row.Avatar = Avatar;
                row.MeshGameObject = Mesh.gameObject;
                row.MeshType = "MeshRenderer";
                row.ParentObjectList = GameObjectQuery.GetRelativeParentsQuery(Avatar, Mesh.gameObject);
                row.Parent = Mesh.gameObject.GetComponentInParent<Transform>().gameObject;
                row.Depth = row.ParentObjectList.Count;
                row.Materials = Mesh.sharedMaterials.ToList();
                Tbl.Add(row);
            }
            this.Tbl.AddRange(TempTbl);
        }
    }

    /// <summary>Define SkinnedMeshRenderer and MeshRenderer related row data</summary>
    public struct AvatarMeshPropertiesRow
    {
        ///<value>Property to hold avatar GameObject</value>
        public GameObject Avatar;

        ///<value>Property to hold GameObject include SkinnedMeshRenderer or MeshRenderer script in avatar GameObject</value>
        public GameObject MeshGameObject;

        ///<value>Property to hold data about the GameObject include which type of renderer</value>
        public string MeshType;

        ///<value>Property to hold root bone of SkinnedMeshRenderer</value>
        public GameObject RootBone;

        ///<value>Property to hold all bone GameObject used by SkinnedMeshRenderer</value>
        public List<GameObject> RelatedBones;

        ///<value>Property to hold all parent GameObject of SkinnedMeshRenderer or MeshRenderer GameObject</value>
        public List<GameObject> ParentObjectList;

        ///<value>Property to hold parent GameObject</value>
        public GameObject Parent;

        ///<value>Property to hold depth of SkinnedMeshRenderer or MeshRenderer GameObject</value>
        public int Depth;

        ///<value>Property to hold Materials for SkinnedMeshRenderer and MeshRenderer</value>
        public List<Material> Materials;
    }
}