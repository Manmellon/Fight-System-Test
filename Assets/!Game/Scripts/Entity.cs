using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private float movingSpeed;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        World.singleton.AddEntity(this);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
