using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace DataManager
{
    /// <summary>Util Querys to analyze structure of GameObject</summary>
    public class GameObjectQuery
    {
        /// <summary> Query function to get all parent GameObjects of Target GameObject</summary>
        /// <param name="Target">Target GameObject to find Parent GameObjects</param>
        public static List<GameObject> GetParentsObjectsQuery(GameObject Target)
        {
            List<GameObject> Parents = new List<GameObject>();
            var Query = from Object in Target.GetComponentsInParent<Transform>(includeInactive: true)
                        select Object.gameObject;
            Parents = Query.ToList();
            return Parents;
        }

        /// <summary>Query function to get parent GameObjects of Target GameObject relative to Root GameObject, Depth of Root Object is 1 </summary>
        /// <param name="Root">Root GameObject to stop</param>
        /// <param name="Target">Target GameObject to find Parent GameObjects</param>
        public static List<GameObject> GetRelativeParentsQuery(GameObject Root, GameObject Target)
        {
            List<GameObject> RelativeParents = new List<GameObject>();
            var RootDepth = GameObjectQuery.GetParentsObjectsQuery(Root).Count;
            var Query = from Object in Target.GetComponentsInParent<Transform>(includeInactive: true)
                        where GameObjectQuery.GetParentsObjectsQuery(Object.gameObject).Count >= RootDepth
                        select Object.gameObject;
            RelativeParents = Query.ToList();
            return RelativeParents;
        }

        /// <summary>Query function to get GameObject by name and depth</summary>
        /// <param name="Root">Root GameObject for searching</param>
        /// <param name="ObjectName">Name of GameObject for searching</param>
        /// <param name="Depth">Relative depth of GameObject to Root for searching</param>
        public static List<GameObject> GetGameObjectsByName(GameObject Root,string ObjectName,int Depth){
            List<GameObject> GameObjectList = new List<GameObject>();
            var Query = from Object in Root.GetComponentsInChildren<Transform>(includeInactive: true)
                        where Object.name == ObjectName
                        where GetRelativeParentsQuery(Root,Object.gameObject).Count == Depth
                        select Object.gameObject;
            GameObjectList = Query.ToList();
            return GameObjectList;
        }
    }

    /// <summary>Util Querys to pair Avatar Bone to Humanoid Bone</summary>
    public class HumanoidQueryChain
    {
        /// <summary> Basic Query to pair Avatar Bone to Hips, Spine, Chest, Neck, Head </summary>
        /// <param name="Depth"> Depth relative to Armature bone condition to filtering GameObject </param>
        /// <param name="Expression"> WIP: Keyword to filtering GameObejct </param>
        /// <param name="Tbl"> Table inputting Bone related data </param>
        public static List<GameObject> HumanoidBasicQuery(int Depth, string Expression, IEnumerable<IBoneData> Tbl)
        {
            Regex rx = new Regex(Expression, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var Query = from row in Tbl
                        where row.GetDepth() == Depth
                        where rx.IsMatch(row.GetBone().name)
                        where !new Regex(@"(costume|sub|twist|bell)", RegexOptions.IgnoreCase).IsMatch(row.GetBone().name)
                        select row.GetBone();
            return Query.ToList();
        }

        /// <summary> Query to pair Avatar Bone to Humanoid Bone that divided by direction </summary>
        /// <param name="Depth"> Depth relative to Armature bone condition to filtering GameObject </param>
        /// <param name="Expression"> WIP: Keyword to filtering GameObejct </param>
        /// <param name="Direction"> Direction condition to filtering GameObject, only Left or Right is allowed to input </param>
        /// <param name="Tbl"> Table inputting Bone related data </param>
        public static List<GameObject> HumanoidLimbQuery(int Depth, string Expression, string Direction, IEnumerable<IBoneData> Tbl)
        {
            Regex rx = new Regex(Expression, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (Direction == "Left")
            {
                var Query = from row in Tbl
                            where row.GetDepth() == Depth
                            where rx.IsMatch(row.GetBone().name)
                            where !new Regex(@"(costume|sub|twist|bell)", RegexOptions.IgnoreCase).IsMatch(row.GetBone().name)
                            where row.GetRelativePos().x < 0
                            select row.GetBone();
                if (Query.ToList().Count == 0)
                {
                    Debug.Log("Depth:" + Depth + ",RegexExpression:" + Expression + ",Direction:" + Direction);
                }
                return Query.ToList();
            }
            else
            {
                var Query = from row in Tbl
                            where row.GetDepth() == Depth
                            where rx.IsMatch(row.GetBone().name)
                            where !new Regex(@"(costume|sub|twist|bell)", RegexOptions.IgnoreCase).IsMatch(row.GetBone().name)
                            where row.GetRelativePos().x > 0
                            select row.GetBone();
                if (Query.ToList().Count == 0)
                {
                    Debug.Log("Depth:" + Depth + ",RegexExpression:" + Expression + ",Direction:" + Direction);
                }
                return Query.ToList();
            }
        }
    }
}