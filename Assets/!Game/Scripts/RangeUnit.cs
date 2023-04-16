using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUnit : FightingUnit
{
    [SerializeField] private Projectile _projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.Euler(transform.forward));
        projectile.Init(this);
    }
}
