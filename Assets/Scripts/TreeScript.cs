using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour{
    public EnumScript.Tree treeType;
    public TreeScript()
    {

    }
    public void SetTreeType(EnumScript.Tree type)
    {
        treeType = type;
    }
}
