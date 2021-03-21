using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wqefqdw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        uiText.text = "0 m/s";
    }

    //Drag these objects in from the inspector
    public Rigidbody2D player;
    public Text uiText;
    public float conversionFactor;

    void Update()
    {
        uiText.text = (player.velocity.magnitude * conversionFactor).ToString("F2") + " m/s";
    }

}
