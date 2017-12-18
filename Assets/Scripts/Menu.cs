using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    public GUIStyleState styleState;

    private GameObject ffc;
    private FireFlyCreator ffcScript;
    private GUIStyle style;

    private bool showInfo = false;

    // Use this for initialization
    void Start () {
        ffc = GameObject.Find("FireFlyCreator");
        ffcScript = ffc.GetComponent<FireFlyCreator>();

        style = new GUIStyle();
        style.fontSize = 30;

        styleState.textColor = Color.white;
    }
	
	// Update is called once per frame
	void Update () {
        // ESCキーで終了
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // @キーで位相差の表示切り替え
        if (Input.GetKeyDown(KeyCode.F1) )
        {
            showInfo = !showInfo; 
        }
    }

    void OnGUI()
    {
        if(showInfo)
        {
            //デバッグ情報表示
            string sigma = ffcScript.orderSigma.ToString();
            style.normal = styleState;
            GUI.Label(new Rect(10, 10, 400, 300), "theta σ = " + sigma, style);
        }
    }
}
