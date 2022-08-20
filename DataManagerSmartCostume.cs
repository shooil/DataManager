using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManagerSmartCostume : MonoBehaviour
{
    public GameObject Avatar;
    public GameObject AvatarArmature;
    public GameObject SmartCostume;
    public GameObject ReferenceSmartCostume;

    public Dictionary<GameObject,bool> CostumeCheckBoxUI = new Dictionary<GameObject, bool>();
    public Dictionary<GameObject,bool> CostumeFoldOutUI = new Dictionary<GameObject, bool>();
}
