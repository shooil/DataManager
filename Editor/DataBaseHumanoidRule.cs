using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Microsoft.VisualBasic;

namespace DataManager
{
    public class HumanoidRule
    {
        public List<HumanoidBoneRow> Tbl;
        public void Categorize(IEnumerable<IBoneData> Tbl)
        {
            this.Tbl = new List<HumanoidBoneRow>();

            // ============================================================================================================================
            // Read humanoid bone assignment from csv file, and add regex expression to table
            var RefTbl = HumanoidBoneRefTbl.Parse("");
            var Query = from row in RefTbl
                        select new { HumanoidName = row.HumanoidName, Expression = "(" + String.Join("|", row.Expressions) + ")", Depth = row.Depth };

            // ============================================================================================================================
            // Query humanoid bone name and depth not devided by left and right from csv file
            var MainQuery = from MainRow in Query
                            where !MainRow.HumanoidName.Contains("Left") && !MainRow.HumanoidName.Contains("Right")
                            select MainRow;
            foreach (var Mrow in MainQuery)
            {
                this._writeHumanoidBasic(Tbl, Mrow.HumanoidName, Mrow.Depth, Mrow.Expression);
            }

            // ============================================================================================================================
            // Query humanoid bone name and depth devided by left and right from csv file
            var LimbQuery = from LimbRow in Query
                            where LimbRow.HumanoidName.Contains("Left") || LimbRow.HumanoidName.Contains("Right")
                            select LimbRow;
            foreach (var Lrow in LimbQuery)
            {
                if (Lrow.HumanoidName.Contains("Left"))
                {
                    this._writeHumanoidLimb(Tbl, Lrow.HumanoidName, Lrow.Depth, Lrow.Expression, "Left");
                }
                else if (Lrow.HumanoidName.Contains("Right"))
                {
                    this._writeHumanoidLimb(Tbl, Lrow.HumanoidName, Lrow.Depth, Lrow.Expression, "Right");
                }
            }
        }

        private void _writeHumanoidBasic(IEnumerable<IBoneData> Tbl, string HumanoidName, int Depth, string Expression)
        {
            var TempRow = new HumanoidBoneRow();
            try
            {
                GameObject BoneGameObejct = HumanoidQueryChain.HumanoidBasicQuery(Depth, Expression, Tbl)[0];
                TempRow = new HumanoidBoneRow { HumanoidName = HumanoidName, Bone = BoneGameObejct };
            }
            catch (Exception)
            {
                TempRow = new HumanoidBoneRow { HumanoidName = HumanoidName };
            }
            this.Tbl.Add(TempRow);
        }

        private void _writeHumanoidLimb(IEnumerable<IBoneData> Tbl, string HumanoidName, int Depth, string Expression, string Direction)
        {
            var TempRow = new HumanoidBoneRow();
            try
            {
                GameObject BoneGameObejct = HumanoidQueryChain.HumanoidLimbQuery(Depth, Expression, Direction, Tbl)[0];
                TempRow = new HumanoidBoneRow { HumanoidName = HumanoidName, Bone = BoneGameObejct };
            }
            catch (Exception)
            {
                Debug.Log("Query Error Count:" + HumanoidQueryChain.HumanoidLimbQuery(Depth, Expression, Direction, Tbl).Count);
                TempRow = new HumanoidBoneRow { HumanoidName = HumanoidName };
            }
            this.Tbl.Add(TempRow);
        }
    }

    public struct HumanoidBoneRow
    {
        public string HumanoidName;
        public string ParentHumanoidName;
        public List<string> Expressions;
        public GameObject Bone;
        public int Depth;
    }

    public interface IBoneData
    {
        GameObject GetBone();
        int GetDepth();
        Vector3 GetRelativePos();
    }
}