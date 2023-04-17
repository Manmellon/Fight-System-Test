using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI redAmountText;
    [SerializeField] TextMeshProUGUI blueAmountText;

    public static UI singleton { get; private set; }

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        redAmountText.text = World.singleton.GetAliveUnitsCount(0).ToString();
        blueAmountText.text = World.singleton.GetAliveUnitsCount(1).ToString();
    }
}
