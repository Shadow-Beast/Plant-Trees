using System.Collections.Generic;

[System.Serializable]
public class TreeFile{
    public string fileName;
    public List<TreeData> treeList;
    public int treeId;
    
    public TreeFile()
    {

    }

    public TreeFile(string name,List<TreeData> list,int id)
    {
        fileName = name;
        treeList = list;
        treeId = id;
    }
}
