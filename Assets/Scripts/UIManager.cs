using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    Personnage[] personnages = new Personnage[2];
    
    // Start is called before the first frame update
    void Start()
    {
        personnages = FindObjectsOfType<Personnage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
