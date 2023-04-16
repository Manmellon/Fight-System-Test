using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingUnit : Entity
{
    [SerializeField] private int _team;
    public int Team => _team;

    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int team)
    {
        _team = team;
    }
}
