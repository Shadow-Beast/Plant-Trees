using UnityEngine;
[System.Serializable]
public class TreeData{
    public string treeName;
    public EnumScript.Tree treeType;
    public float treePositionX;
    public float treePositionZ;

    public TreeData()
    {

    }

    public TreeData(string name,EnumScript.Tree type,float x,float z)
    {
        treeName = name;
        treeType = type;
        treePositionX = x;
        treePositionZ = z;
    }

    public void ChangePosition(float x,float z)
    {
        treePositionX = x;
        treePositionZ = z;
    }
}
