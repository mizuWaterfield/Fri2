using UnityEngine;
using System.Collections;

// https://qiita.com/junya/items/ce1d0f4d3da61ba38df7
public class CameraChanger : MonoBehaviour
{

    private GameObject MainCam;
    private GameObject SubCam;

    void Start()
    {
        MainCam = GameObject.Find("MainCamera");
        SubCam = GameObject.Find("SubCamera");

        SubCam.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (MainCam.activeSelf)
            {
                MainCam.SetActive(false);
                SubCam.SetActive(true);
            }
            else
            {
                MainCam.SetActive(true);
                SubCam.SetActive(false);
            }
        }
    }

}