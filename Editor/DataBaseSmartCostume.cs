using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace DataManager
{
    public class SmartCostumeTbl
    {
        public List<SmartCostumeRow> OnOffTbl;
        public List<SmartCostumePresetRow> PresetTbl;

        public void WriteSmartCostumeDataTbl(GameObject SmartCostume)
        {
            this.OnOffTbl = new List<SmartCostumeRow>();
            this.PresetTbl = new List<SmartCostumePresetRow>();
            _addSmartCostumePresetRecord(SmartCostume);
            _addSmartCostumeGroupRecord(SmartCostume);
        }

        private void _addSmartCostumeGroupRecord(GameObject SmartCostume)
        {
            List<SmartCostumeRow> Tbl = new List<SmartCostumeRow>();
            foreach (var SmartCostumeGroupScript in SmartCostume.GetComponentsInChildren<HoshinoLabs.SmartCostume.SmartCostume_CostumeGroup>())
            {
                SmartCostumeRow Row = new SmartCostumeRow();
                Row.SmartCostume = SmartCostume;
                Row.SmartCostumeGroupObject = SmartCostumeGroupScript.gameObject;
                Row.AvatarAffectedObject = this._getOnOffObjects(Row.SmartCostumeGroupObject);
                var PresetQuery = from row in this.PresetTbl
                                  where row.SmartCostumeGroupList.Contains(Row.SmartCostumeGroupObject)
                                  select row.SmartCostumePreset;
                Row.SmartCostumePreset = PresetQuery.ToList();
                Row.Depth = GameObjectQuery.GetRelativeParentsQuery(SmartCostume, Row.SmartCostumeGroupObject).Count;
                Tbl.Add(Row);
            }
            this.OnOffTbl = Tbl;
        }

        private void _addSmartCostumePresetRecord(GameObject SmartCostume)
        {
            List<SmartCostumePresetRow> Tbl = new List<SmartCostumePresetRow>();
            foreach (var SmartCostumePresetScript in SmartCostume.GetComponentsInChildren<HoshinoLabs.SmartCostume.SmartCostume_CostumeLink>())
            {
                SmartCostumePresetRow Row = new SmartCostumePresetRow();
                Row.SmartCostume = SmartCostume;
                Row.SmartCostumePreset = SmartCostumePresetScript.gameObject;
                Row.SmartCostumeGroupList = this._getSmartCostumeGroupList(SmartCostumePresetScript);
                Tbl.Add(Row);
            }
            this.PresetTbl = Tbl;
        }

        private List<GameObject> _getOnOffObjects(GameObject SmartCostumeGroup)
        {
            List<GameObject> OnOffObjs = new List<GameObject>();
            var CostumeScript = SmartCostumeGroup.GetComponent<HoshinoLabs.SmartCostume.SmartCostume_Costume>();
            var SerializedObject = new UnityEditor.SerializedObject(CostumeScript);
            var Container = SerializedObject.FindProperty("container");
            for (int i = 0; i < Container.arraySize; i++)
            {
                var Element = Container.GetArrayElementAtIndex(i);
                var Obj = Element.FindPropertyRelative("obj").objectReferenceValue as GameObject;
                OnOffObjs.Add(Obj);
            }
            return OnOffObjs;
        }

        private List<GameObject> _getSmartCostumeGroupList(HoshinoLabs.SmartCostume.SmartCostume_CostumeLink SmartCostumePresetScript)
        {
            List<GameObject> SmartCostumeGroupList = new List<GameObject>();
            var SerializedObject = new UnityEditor.SerializedObject(SmartCostumePresetScript);
            var Container = SerializedObject.FindProperty("container");
            for (int i = 0; i < Container.arraySize; i++)
            {
                var Element = Container.GetArrayElementAtIndex(i);
                var Obj = Element.FindPropertyRelative("group").objectReferenceValue as HoshinoLabs.SmartCostume.SmartCostume_CostumeGroup;
                SmartCostumeGroupList.Add(Obj.gameObject);
            }
            return SmartCostumeGroupList;
        }
    }

    public class SmartCostumeRow
    {
        public GameObject SmartCostume;
        public GameObject SmartCostumeGroupObject;
        public int Depth;
        public List<GameObject> SmartCostumePreset;
        public List<GameObject> AvatarAffectedObject;
    }

    public class SmartCostumePresetRow
    {
        public GameObject SmartCostume;
        public GameObject SmartCostumePreset;
        public List<GameObject> SmartCostumeGroupList;
    }
}