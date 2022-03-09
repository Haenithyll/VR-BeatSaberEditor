using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public GameObject CubesParent;

    private Save _newSave = new Save();
    private Save _loadSave = new Save();

    [SerializeField] private GameObject _RedCubePrefab;
    [SerializeField] private GameObject _BlueCubePrefab;

    private string _fileName = "cubeSave.txt";

    private void Awake()
    {
        if (Main.CurrentLevelType == Main.LevelType.Playing)
            LoadJson();
    }

    private void RetrieveCubeData()
    {
        GameObject cube;
        CubeData cubeData;

        bool Color;
        Vector3 position;
        Vector3 rotation;

        for (int index = 0; index < CubesParent.transform.childCount; index++)
        {
            cube = CubesParent.transform.GetChild(index).gameObject;

            position = cube.transform.localPosition;
            rotation = cube.transform.rotation.eulerAngles;

            Color = cube.GetComponent<Cube>().color == Saber.Color.Red;
            cubeData = new CubeData(position.x, position.y, position.z, rotation.z, Color);

            _newSave.Cubes.Add(cubeData);
        }
    }

    public void SaveIntoJson()
    {
        RetrieveCubeData();
        string json = string.Empty;

        json = JsonUtility.ToJson(_newSave);
        WriteToFile(json);
    }

    public void LoadJson()
    {
        string json = ReadFromFile();
        JsonUtility.FromJsonOverwrite(json, _loadSave);

        SpawnCubesFromLoad();
    }

    private void SpawnCubesFromLoad()
    {

        foreach (CubeData cubeData in _loadSave.Cubes)
        {
            GameObject cubeToSpawn;
            GameObject newCube;

            cubeToSpawn = cubeData.color ? _RedCubePrefab : _BlueCubePrefab;
            newCube = Instantiate(cubeToSpawn, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, cubeData.rotation));
            newCube.transform.SetParent(CubesParent.transform);
            newCube.transform.localPosition = new Vector3(cubeData.x, cubeData.y, cubeData.z);

        }
    }

    private void WriteToFile(string json)
    {
        string path = GetFilePath();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    private string ReadFromFile()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                string json = streamReader.ReadToEnd();
                return json;
            }
        }
        else
        {
            throw new System.Exception("File not found");
        }
    }

    private string GetFilePath()
    {
        return Application.persistentDataPath + _fileName;
    }
}

