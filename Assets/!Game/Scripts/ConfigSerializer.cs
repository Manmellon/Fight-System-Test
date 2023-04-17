using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ConfigSerializer : MonoBehaviour
{
    [SerializeField] Entity[] entities;

    // Start is called before the first frame update
    void Start()
    {
        SaveConfig();
        LoadConfig();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveConfig()
    {
        using (StreamWriter writer = new StreamWriter("config.json"))
        {
            /*string s = "";
            for (int i = 0; i < entities.Length; i++)
            {
                s += (i>0 ? "," : "") + entities[i].Serialize();
            }
            writer.WriteLine("[" + s + "]");*/
            SerializedEntity[] serializedEntities = new SerializedEntity[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                serializedEntities[i] = entities[i].Serialize();
            }
            writer.WriteLine(JsonHelper.ToJson(entities, true)); 
        }
    }

    public void LoadConfig()
    {
        try
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                string json = reader.ReadToEnd();
                SerializedEntity[] serializedEntities = JsonHelper.FromJson<SerializedEntity>(json);
                Debug.Log(serializedEntities.Length);
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
