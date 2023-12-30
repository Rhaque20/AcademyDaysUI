using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField]float maxLength = 520f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float returnPosition(float ratio)
    {
        Debug.Log("Position is "+(ratio*maxLength));
        return ratio * maxLength;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
