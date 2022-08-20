using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace DataManager
{
    [CustomEditor(typeof(DataManagerSmartCostume))]
    public class DataManagerSmartCostumeUI : Editor
    {
        public List<SmartCostumeRow> SmartCostumeDataTbl;
        public List<SmartCostumeRow> ReferenceSmartCostumeDataTbl;
        public List<AvatarMeshPropertiesRow> MeshDataTbl;
        public List<AvatarBonePropertiesRow> BoneDataTbl;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DataManagerSmartCostume manager = target as DataManagerSmartCostume;
            if (this.SmartCostumeDataTbl != null && this.ReferenceSmartCostumeDataTbl != null && this.MeshDataTbl != null)
            {
                this.GenMenu(manager);
            }
            if (GUILayout.Button("1.Initialize")) { this.Initialize(manager); }
            if (GUILayout.Button("2.Apply change")) { this.ApplyChange(manager); }
        }
        public void Initialize(DataManagerSmartCostume manager)
        {
            GameObject Avatar = manager.Avatar;
            GameObject Armature = manager.AvatarArmature;
            GameObject SmartCostume = manager.SmartCostume;
            GameObject ReferenceSmartCostume = manager.ReferenceSmartCostume;
            // AvatarBoneTbl BoneData = new AvatarBoneTbl();
            // BoneData.WriteAvatarBoneDataTbl(Avatar,Armature);
            AvatarMeshTbl MeshData = new AvatarMeshTbl();
            MeshData.WriteAvatarMeshDataTbl(Avatar);
            SmartCostumeTbl SmartCostumeData = new SmartCostumeTbl();
            SmartCostumeData.WriteSmartCostumeDataTbl(SmartCostume);
            SmartCostumeTbl ReferenceSmartCostumeData = new SmartCostumeTbl();
            ReferenceSmartCostumeData.WriteSmartCostumeDataTbl(ReferenceSmartCostume);
            this.SmartCostumeDataTbl = SmartCostumeData.OnOffTbl;
            this.ReferenceSmartCostumeDataTbl = ReferenceSmartCostumeData.OnOffTbl;
            this.MeshDataTbl = MeshData.Tbl;
            // this.BoneDataTbl = BoneData.Tbl;
            this._writeMenuInitialState(manager);
        }

        public void GenMenu(DataManagerSmartCostume manager)
        {
            var genreList = manager.CostumeFoldOutUI.Keys.ToList();
            foreach (var genre in genreList)
            {
                var itemQuery = from row in this.ReferenceSmartCostumeDataTbl
                                where row.SmartCostumeGroupObject.transform.parent.gameObject == genre
                                where row.SmartCostumeGroupObject != null
                                select row.SmartCostumeGroupObject;
                if (manager.CostumeFoldOutUI[genre] = EditorGUILayout.Foldout(manager.CostumeFoldOutUI[genre], genre.name))
                {
                    foreach (var item in itemQuery)
                    {
                        manager.CostumeCheckBoxUI[item] = EditorGUILayout.ToggleLeft(item.name, manager.CostumeCheckBoxUI[item]);
                    }
                }
            }
        }
        public void ApplyChange(DataManagerSmartCostume manager)
        {
            string refName = manager.ReferenceSmartCostume.name;
            string tarName = manager.SmartCostume.name;
            var NewRef = Instantiate(manager.ReferenceSmartCostume, manager.ReferenceSmartCostume.transform.parent);
            var OnOffQuery = from disabled in manager.CostumeCheckBoxUI
                             where disabled.Value == false
                             select disabled.Key;
            var PresetQuery = from row in this.ReferenceSmartCostumeDataTbl
                              where OnOffQuery.Contains(row.SmartCostumeGroupObject)
                              select row.SmartCostumePreset;
            var OnOffBack = OnOffQuery.ToList();
            foreach (var item in OnOffBack)
            {
                Object.DestroyImmediate(item, true);
            }
            var PresetBack = PresetQuery.SelectMany(x => x).Distinct().ToList();
            foreach (var preset in PresetBack)
            {
                Object.DestroyImmediate(preset, true);
            }
            Object.DestroyImmediate(manager.SmartCostume, true);
            manager.ReferenceSmartCostume.name = tarName;
            NewRef.name = refName;
            manager.SmartCostume = manager.ReferenceSmartCostume;
            manager.ReferenceSmartCostume = NewRef;
            this._editAvatar(manager);
            this.Initialize(manager);
        }
        private void _editAvatar(DataManagerSmartCostume manager)
        {
            this._resetAvatar();
            var OnOffQuery = from disabled in manager.CostumeCheckBoxUI
                             where disabled.Value == false
                             select disabled.Key;
            var ObjectQuery = from row in this.ReferenceSmartCostumeDataTbl
                              where OnOffQuery.Contains(row.SmartCostumeGroupObject)
                              where row.AvatarAffectedObject != null
                              select row.AvatarAffectedObject;
            var ObjectList = ObjectQuery.SelectMany(x => x).Distinct().ToList();
            var BoneQuery = from row in this.MeshDataTbl
                            where row.ParentObjectList.Union(ObjectList).Count() != 0
                            where row.MeshType == "SkinnedMeshRenderer"
                            where row.RelatedBones != null
                            select row.RelatedBones;
            foreach (var disableObj in ObjectQuery.SelectMany(x => x))
            {
                disableObj.tag = "EditorOnly";
                disableObj.SetActive(false);
            }
            foreach (var disableBone in BoneQuery.SelectMany(x => x))
            {
                disableBone.tag = "EditorOnly";
                disableBone.SetActive(false);
            }
        }
        private void _resetAvatar()
        {
            var AllObjectQuery = from row in this.ReferenceSmartCostumeDataTbl
                                 where row.AvatarAffectedObject != null
                                 select row.AvatarAffectedObject;
            foreach (var allObj in AllObjectQuery.SelectMany(x => x))
            {
                allObj.tag = "Untagged";
            }
            var AllBoneQuery = from row in this.MeshDataTbl
                               where row.RelatedBones != null
                               select row.RelatedBones;
            foreach (var allBone in AllBoneQuery.SelectMany(x => x))
            {
                allBone.tag = "Untagged";
                allBone.SetActive(true);
            }
        }
        private void _writeMenuInitialState(DataManagerSmartCostume manager)
        {
            var GenreQuery = from row in this.ReferenceSmartCostumeDataTbl
                             select row.SmartCostumeGroupObject.transform.parent.gameObject;
            GenreQuery = GenreQuery.Distinct();
            foreach (var genre in GenreQuery)
            {
                manager.CostumeFoldOutUI[genre] = false;
            }
            var ItemQueryRef = from row in this.ReferenceSmartCostumeDataTbl
                               select row.SmartCostumeGroupObject;
            foreach (var item in ItemQueryRef)
            {
                manager.CostumeCheckBoxUI[item] = false;
            }
            var ItemQueryAct = from row in this.ReferenceSmartCostumeDataTbl
                               join arow in this.SmartCostumeDataTbl
                               on new { row.SmartCostumeGroupObject.name, row.Depth } equals new { arow.SmartCostumeGroupObject.name, arow.Depth }
                               select row.SmartCostumeGroupObject;
            foreach (var enable in ItemQueryAct)
            {
                manager.CostumeCheckBoxUI[enable] = true;
            }
        }
    }
}