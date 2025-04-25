using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    Camera cam;
    Personnage[] personnages = new Personnage[2];
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        personnages = FindObjectsOfType<Personnage>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 differencePersonnages = new(personnages[0].transform.position.x - personnages[1].transform.position.x, personnages[0].transform.position.y - personnages[1].transform.position.y);
        differencePersonnages /= 2;
        transform.position = new(differencePersonnages.x + personnages[1].transform.position.x, differencePersonnages.y + personnages[1].transform.position.y, transform.position.z);
    }
}
