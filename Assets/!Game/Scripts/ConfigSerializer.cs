using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ConfigSerializer : MonoBehaviour
{
    [SerializeField] List<Entity> serializedEntities;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(JsonHelper);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveConfig()
    {
        using (StreamWriter writer = new StreamWriter("config.json"))
        {
            
        }
    }

    public void LoadConfig()
    {
        try
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                string json = reader.ReadToEnd();
                //JsonUtility.FromJsonOverwrite(json, serializedEntities[]);
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
