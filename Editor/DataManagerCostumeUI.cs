using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataManager
{
    [CustomEditor(typeof(DataManagerCostume))]
    public class DataManagerCostumeUI : Editor
    {
        public List<AvatarBonePropertiesRow> AvatarTbl;
        public List<AvatarBonePropertiesRow> TargetTbl;
        public List<HumanoidBoneRow> TargetHumanoidTbl;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DataManagerCostume costume = target as DataManagerCostume;
            if (GUILayout.Button("1.Initialize"))
            {
                this.Initialize(costume.Avatar, costume.CostumeList);
            }
            if (GUILayout.Button("2.AddSufix"))
            {
                this.AddSuffix(costume.Suffix);
            }
            if (GUILayout.Button("3.Costume"))
            {
                this.Costume(costume.Avatar, costume.CostumeList);
            }
            if (GUILayout.Button("Opt.Copy DB to Armature"))
            {
                this.CopyDBToRootBone(costume.Avatar);
            }
        }
        public void Initialize(GameObject Avatar, List<GameObject> Targets)
        {
            var TargetTbl = new List<HumanoidBoneRow>();
            // Get all armature game objects
            var Avatar_Armature = GameObjectQuery.GetGameObjectsByName(Avatar, "Hips", 3)[0].transform.parent.gameObject;
            this._writeAvatarTbl(Avatar, Avatar_Armature);
            foreach (var Target in Targets)
            {
                var Candidates = GameObjectQuery.GetGameObjectsByName(Target, "Hips", 3);
                if(Candidates.Count == 1){
                    var Target_Armature = Candidates[0].transform.parent.gameObject;
                    TargetTbl.AddRange(this._getTargetTbl(Target, Target_Armature));
                }else{
                    Debug.Log("No armature error:"+Target.name);
                    continue;
                }
            }
            this.TargetHumanoidTbl = TargetTbl;
        }

        public void Costume(GameObject Avatar, List<GameObject> Targets)
        {
            // ==========================================================
            // Move all bone in costume into avatar base on data in table
            // ==========================================================
            var Tbl = from arow in this.AvatarTbl
                      join crow in this.TargetHumanoidTbl
                      on arow.HumanoidName equals crow.HumanoidName
                      where arow.HumanoidName != "" && arow.Bone != null
                      where crow.HumanoidName != "" && crow.Bone != null
                      select new { Humanoid = arow.HumanoidName, AvatarBone = arow.Bone, CostumeBone = crow.Bone };
            foreach (var row in Tbl.ToList())
            {
                row.CostumeBone.transform.parent = row.AvatarBone.transform;
            }

            // ==========================================================
            // Move other game objects into avatar
            // ==========================================================
            foreach (var target in Targets){
                target.transform.parent = Avatar.transform;
            }
        }
        public void AddSuffix(string suffix)
        {
            Regex rx = new Regex(@"\(.*\)", RegexOptions.Compiled);
            suffix = "(" + suffix + ")";
            var Query = from row in this.TargetTbl
                        select row.Bone;
            foreach (var bone in Query.ToList())
            {
                var name = bone.name;
                if (rx.IsMatch(name))
                {
                    bone.name = name.Replace(rx.Match(name).Value, suffix);
                }
                else
                {
                    bone.name = name + suffix;
                }
            }
        }
        public void CopyDBToRootBone(GameObject Costume)
        {
            var ScriptQuery = from script in Costume.GetComponentsInChildren<DynamicBone>(includeInactive: true)
                              where !(from row in this.TargetTbl
                                      select row.Bone).Contains(script.gameObject)
                              select script;
            foreach (var DBScript in ScriptQuery)
            {
                var RootBone = DBScript.m_Root.gameObject;
                UnityEditorInternal.ComponentUtility.CopyComponent(DBScript);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(RootBone);
            }
            var len = ScriptQuery.ToList().Count;
            for (var n = 0; n < len; n++)
            {
                Object.DestroyImmediate(ScriptQuery.ToList()[n], true);
            }
        }
        private void _writeAvatarTbl(GameObject Avatar, GameObject Armature)
        {
            AvatarBoneTbl ABTbl = new AvatarBoneTbl();
            ABTbl.WriteAvatarBoneDataTbl(Avatar, Armature);
            this.AvatarTbl = ABTbl.Tbl;
        }

        private List<HumanoidBoneRow> _getTargetTbl(GameObject Target, GameObject TargetArmature)
        {
            AvatarBoneTbl TaTbl = new AvatarBoneTbl();
            TaTbl.WriteAvatarBoneDataTbl(Target, TargetArmature);
            this.TargetTbl = TaTbl.Tbl;
            HumanoidRule TaTblHu = new HumanoidRule();
            TaTblHu.Categorize(TaTbl.Tbl);
            return TaTblHu.Tbl;
        }
    }
}