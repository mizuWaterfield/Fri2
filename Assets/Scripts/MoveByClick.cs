using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


// 中クリックで移動(Z,X軸方向)
public class MoveByClick : MonoBehaviour {
    [SerializeField, Range(0.01f, 1.0f)]
    public float moveAmpl = 0.1f;

    private Vector2 origMousePos;
    private Vector3 origCamPos;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // 中ボタンが押されたとき、そこをカメラとマウス原点とする
        if (Input.GetMouseButtonDown(2))
        {
            origMousePos = Input.mousePosition;
            origCamPos = transform.position;
        }

        // 中ボタンが押されている間、マウスの原点との差をカメラの原点に足す
        if(Input.GetMouseButton(2))
        {
            Vector2 nowMousePos = Input.mousePosition;
            Vector2 dMousePos = (origMousePos - nowMousePos);
            transform.position = origCamPos + moveAmpl * new Vector3(dMousePos.x, 0.0f, dMousePos.y);
        }
    }
}
