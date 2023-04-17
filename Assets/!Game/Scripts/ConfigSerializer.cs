using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class ConfigSerializer : MonoBehaviour
{
    [SerializeField] Entity[] entities;

    private JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

    // Start is called before the first frame update
    void Start()
    {
        //SaveConfig();
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
            //writer.WriteLine(JsonHelper.ToJson(serializedEntities, true));
            writer.WriteLine(JsonConvert.SerializeObject(serializedEntities, Formatting.Indented, settings));
        }
    }

    public void LoadConfig()
    {
        try
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                string json = reader.ReadToEnd();
                SerializedEntity[] serializedEntities = JsonConvert.DeserializeObject<SerializedEntity[]>(json, settings);
                foreach (var se in serializedEntities)
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        if (entities[i].name.Equals(se.name))
                        {
                            entities[i].Deserialize(se);
                        }
                    }
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
