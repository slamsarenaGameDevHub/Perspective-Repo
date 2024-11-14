using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrafficLight : MonoBehaviour {

    public GameObject Green;
    public GameObject Yellow;
    public GameObject Red;
    public GameObject Pedestrians;
    public GameObject StopCollider;

    public void SetStatus(string status)
    {

        Red.SetActive(status == "1");
        Yellow.SetActive(status == "2");
        Green.SetActive(status == "3");
        Pedestrians.SetActive(status == "4");
        StopCollider.SetActive(status == "1" || status == "4");

    }


}
