using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    Camera cam;
    Personnage[] personnages = new Personnage[2];
    float vitesseCam;
    
    // Start is called before the first frame update
    void Start()
    {
        vitesseCam = 5f;
        cam = GetComponent<Camera>();
        personnages = FindObjectsOfType<Personnage>();
    }

    // Update is called once per frame
    void Update()
    {
        BougerCamera();
    }

    void BougerCamera()
    {
        Vector2 differencePersonnages = new(personnages[0].transform.position.x - personnages[1].transform.position.x, personnages[0].transform.position.y - personnages[1].transform.position.y);
        differencePersonnages /= 2;
        Vector3 destination = new(differencePersonnages.x + personnages[1].transform.position.x,
            differencePersonnages.y + personnages[1].transform.position.y);
        Vector3 mouvementCamera = new(destination.x - transform.position.x, destination.y - transform.position.y);
        
        transform.position += mouvementCamera * vitesseCam * Time.deltaTime;
    }
}
