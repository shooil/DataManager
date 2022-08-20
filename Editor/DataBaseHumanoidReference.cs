using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text;
using System;

public class HumanoidBoneRefTbl{
    public static List<HumanoidBoneRow> Parse(string path){
        path = "./Assets/shooil/DataManager/Editor/HumanoidRule.csv";
        var lines = File.ReadAllLines(path,Encoding.Default);
        var Query = from line in lines.Skip(1)
                    let column = line.Split(',')
                    select new HumanoidBoneRow{HumanoidName=column[0],Expressions = column[2].Split('.').ToList(), Depth = Convert.ToInt32(column[3])};
        return Query.ToList();
    }
}