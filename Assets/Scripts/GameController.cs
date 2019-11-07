using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {
    public EnumScript.Tree currentTree;
    public EnumScript.View currentView;
    public EnumScript.State activeState;
    public Text txtChangeType, txtChangeView, txtInformation, txtToolbar;  
    public GameObject tree, ground, popupWindow, blockPanel;
    public InputField inputFieldFileName;
    public Dropdown dropDownFileName;
    public Button btnPlant, btnMove, btnDelete, btnSaveOrLoad;
    public Color activeColor, inactiveColor;
    public Texture normalTreeTexture, appleTreeTexture, orangeTreeTexture;
    public Camera topCamera, sideCamera, frontCamera;
    public string folderPath;
    private bool letTreeMove,isSave;
    private GameObject thisObject;
    private Ray ray;
    private RaycastHit hit;
    //private float x = 0f, z = 0f;

    public List<TreeData> treeList;
    public int treeId, numberOfNormalTree, numberOfAppleTree, numberOfOrangeTree;

    private void Awake()
    {
        treeList = new List<TreeData>();
        currentTree = (EnumScript.Tree)ConstantScript.initial;
        currentView = (EnumScript.View)ConstantScript.initial;
        activeState = (EnumScript.State)ConstantScript.initial;
        treeId = ConstantScript.initial;
        numberOfNormalTree=numberOfAppleTree=numberOfOrangeTree = ConstantScript.initial;
        letTreeMove = false;
        isSave = false;
        SwitchCamera(currentView);
        ShowPopupWindow(false);
        UpdateInformation();
    }

    public void ChangeTypeOfTree()
    {
        currentTree = (EnumScript.Tree)(((int)currentTree) + 1);
        if ((int)currentTree>=Enum.GetNames(typeof(EnumScript.Tree)).Length)
        {
            currentTree = (EnumScript.Tree)ConstantScript.initial;
        }
        txtChangeType.text = ConstantScript.changeTypeString + currentTree.ToString();
    }

    public void ChangeView()
    {
        currentView = (EnumScript.View)(((int)currentView) + 1);
        if ((int)currentView >= Enum.GetNames(typeof(EnumScript.View)).Length)
        {
            currentView = (EnumScript.View)ConstantScript.initial;
        }
        txtChangeView.text = ConstantScript.changeViewString + currentView;
        SwitchCamera(currentView);
    }

    private void SwitchCamera(EnumScript.View cameraView)
    {
        switch (cameraView)
        {
            case EnumScript.View.Top:
                topCamera.enabled = true;
                frontCamera.enabled = false;
                sideCamera.enabled = false;
                break;
            case EnumScript.View.Front:
                topCamera.enabled = false;
                frontCamera.enabled = true;
                sideCamera.enabled = false;
                break;
            case EnumScript.View.Side:
                topCamera.enabled = false;
                frontCamera.enabled = false;
                sideCamera.enabled = true;
                break;
        }
    }

    private void UpdateInformation()
    {
        txtInformation.text = ConstantScript.informationString +
                            ConstantScript.normalTreeString + numberOfNormalTree +
                            ConstantScript.appleTreeString + numberOfAppleTree +
                            ConstantScript.orangeTreeString + numberOfOrangeTree;
    }

    public void SetButtonActive(int btnId)
    {
        activeState = (EnumScript.State) (((int)activeState == btnId) ? ConstantScript.initial : btnId);
        switch (activeState)
        {
            case EnumScript.State.Plant:
                btnPlant.image.color= activeColor;
                btnMove.image.color = inactiveColor;
                btnDelete.image.color = inactiveColor;
                break;
            case EnumScript.State.Move:
                btnPlant.image.color = inactiveColor;
                btnMove.image.color = activeColor;
                btnDelete.image.color = inactiveColor;
                break;
            case EnumScript.State.Delete:
                btnPlant.image.color = inactiveColor;
                btnMove.image.color = inactiveColor;
                btnDelete.image.color = activeColor;
                break;
            default:
                btnPlant.image.color = inactiveColor;
                btnMove.image.color = inactiveColor;
                btnDelete.image.color = inactiveColor;
                break;
        }
    }

    public void ShowPopupWindow(bool toggle)
    {
        popupWindow.SetActive(toggle);
        blockPanel.SetActive(toggle);
        dropDownFileName.gameObject.SetActive(!isSave);
        inputFieldFileName.gameObject.SetActive(isSave);        
        txtToolbar.text = ((isSave) ? ConstantScript.SaveString : ConstantScript.LoadString) + ConstantScript.FileString;
        btnSaveOrLoad.GetComponentInChildren<Text>().text=(isSave) ? ConstantScript.SaveString : ConstantScript.LoadString;
        inputFieldFileName.text = null;
        btnSaveOrLoad.interactable = true;
        folderPath = GetFolderPath();
        if (toggle && !isSave)
        {
            dropDownFileName.ClearOptions();
            List<string> files = GetFiles();
            if (files.Count > 0)
            {
                dropDownFileName.AddOptions(files);               
            }
            else
            {
                btnSaveOrLoad.interactable = false;
            }
        }
    }

    public void IsSaveButtonClicked(bool toggle)
    {
        isSave = toggle;
    }

    public void BtnSaveOrLoadClicked()
    {
        if (isSave)
        {
            if (inputFieldFileName.text != null)
            {
                TreeFile newFile = new TreeFile(inputFieldFileName.text, treeList, treeId);
                SaveTreeData(newFile);
            }
            else
            {
                //for toast
            }
        }
        else
        {
            string fileName = dropDownFileName.options[dropDownFileName.value].text;
            TreeFile treeData = GetTreeData(fileName);
            LoadTreeData(treeData);
        }
        ShowPopupWindow(false);
    }

    List<string> GetFiles()
    {        
        string[] files=Directory.GetFiles(folderPath,ConstantScript.all+ConstantScript.fileExtension);
        List<string> filesList=new List<string>();
        for(int i = 0; i < files.Length; i++)
        {
            filesList.Add(Path.GetFileNameWithoutExtension(files[i]));
        }
        return filesList;
    }

    void SaveTreeData(TreeFile treeData)
    {
        string dataPath = Path.Combine(folderPath, treeData.fileName + ConstantScript.fileExtension);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream,treeData);
        }
        Debug.Log("Data Path:" + dataPath);
    }

    TreeFile GetTreeData(string name)
    {
        string dataPath = Path.Combine(folderPath, name + ConstantScript.fileExtension);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
        {
            return (TreeFile)binaryFormatter.Deserialize(fileStream);
        }
    }

    void LoadTreeData(TreeFile treeData)
    {
        GameObject[] allTrees = GameObject.FindGameObjectsWithTag("Tree");
        for (int i = 0; i < allTrees.Length; i++)
            DeleteTree(allTrees[i]);
        for(int j = 0; j < treeData.treeList.Count; j++)
        {
            PlantTree(treeData.treeList[j].treeName,treeData.treeList[j].treeType,treeData.treeList[j].treePositionX,treeData.treeList[j].treePositionZ);
        }
        treeId = treeData.treeId;
    }

    string GetFolderPath()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, ConstantScript.folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        return folderPath;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!IsPointerOverGameObject())
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    letTreeMove = false;
                    thisObject = hit.collider.gameObject;
                    Debug.Log(thisObject.name);
                    switch (activeState)
                    {
                        case EnumScript.State.Plant:
                            if (thisObject.tag == "Ground")
                            {
                                treeId++;                                
                                string treeName = currentTree.ToString() + treeId;
                                PlantTree(treeName, currentTree, hit.point.x, hit.point.z);                                
                            }
                            break;

                        case EnumScript.State.Move:
                            if (thisObject.tag == "Tree")
                            {
                                letTreeMove = true;
                            }
                            break;

                        case EnumScript.State.Delete:
                            if (thisObject.tag == "Tree")
                            {
                                DeleteTree(thisObject);
                            }
                            break;

                        default:
                            //do nothing
                            break;
                    }

                }
                else if (Input.GetMouseButton(0))
                {
                    if (letTreeMove)
                    {
                        MoveObject(thisObject);
                    }
                }
            }
            
        }  
    }

    bool IsPointerOverGameObject()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        else
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    void PlantTree(string treeName,EnumScript.Tree treeType,float treePositionX,float treePositionZ)
    {
        Vector3 treePosition = new Vector3(treePositionX, ConstantScript.root, treePositionZ);
        GameObject newTree = Instantiate(tree, treePosition, Quaternion.identity);
        newTree.name = treeName;
        newTree.GetComponentInParent<TreeScript>().SetTreeType(treeType);    
        switch (treeType)
        {
            case EnumScript.Tree.Normal:
                newTree.GetComponent<Renderer>().material.mainTexture = normalTreeTexture;
                numberOfNormalTree++;
                break;
            case EnumScript.Tree.Apple:
                newTree.GetComponent<Renderer>().material.mainTexture = appleTreeTexture;
                numberOfAppleTree++;
                break;
            case EnumScript.Tree.Orange:
                newTree.GetComponent<Renderer>().material.mainTexture = orangeTreeTexture;
                numberOfOrangeTree++;
                break;
        }
        treeList.Add(new TreeData(treeName,treeType,treePosition.x,treePosition.z));
        UpdateInformation();
    }

    void MoveObject(GameObject obj)
    {
        Vector3 objPositionScreenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);
        Vector3 objPositionWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objPositionScreenPoint.z));
        Vector3 rawObjPosition = new Vector3(objPositionWorldPoint.x, ConstantScript.root, objPositionWorldPoint.z);
        if (hit.collider.gameObject.tag == "Ground")
        {
            obj.transform.position = rawObjPosition;
            GetTreeDataByName(obj.name).ChangePosition(rawObjPosition.x, rawObjPosition.z);
            //x = rawObjPosition.x;
            //z = rawObjPosition.z;
        }
        //else
        //{
        //    if (x < hit.point.x && z<hit.point.z)
        //    {
        //        rawObjPosition = new Vector3(x, ConstantScript.root,z);
        //    }
        //    else if (x < hit.point.x)
        //    {
        //        rawObjPosition = new Vector3(x, ConstantScript.root, objPositionWorldPoint.z);
        //    }
        //    else if (z < hit.point.z)
        //    {
        //        rawObjPosition = new Vector3(objPositionWorldPoint.x, ConstantScript.root, z);
        //    }            
        //}        
    }

    void DeleteTree(GameObject obj)
    { 
        switch (obj.GetComponentInParent<TreeScript>().treeType)
        {
            case EnumScript.Tree.Normal:
                numberOfNormalTree--;
                break;
            case EnumScript.Tree.Apple:
                numberOfAppleTree--;
                break;
            case EnumScript.Tree.Orange:
                numberOfOrangeTree--;
                break;
        }
        Destroy(obj);
        treeList.Remove(GetTreeDataByName(obj.name));
        UpdateInformation();
    }

    TreeData GetTreeDataByName(string name)
    {
        for(int i = 0; i < treeList.Count; i++)
        {
            if (treeList[i].treeName == name)
                return treeList[i];
        }
        return null;
    }
}