using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyCreator : MonoBehaviour {

    //蛍の生成を行う
    //元となる蛍を複製し量産する
    
    [SerializeField]
    private GameObject fireFly; //コピー元の蛍
    private GameObject ffTemp; //複製した蛍の参照
    [SerializeField]
    private Material ffMat; //コピー元のマテリアル
    public static List<GameObject> fireFlies = new List<GameObject>(); //蛍のリスト
    public static float syncCoefficient = 3f; //同期する重み係数
    [SerializeField]
    private float randWidth = 30f; //蛍の生成範囲
    [SerializeField]
    private float randDepth = 10f; //蛍の生成範囲
    [SerializeField]
    private float randHeight = 30f; //蛍の生成範囲

    public int theNumber = 10;

    //秩序変数のための変数
    private float orderRe,orderIm;
    public float orderAbs;

    /*このようにマテリアルを複製できる
        ff_c=Instantiate(ff);
        */

	// Use this for initialization
	void Start () {
        for (int i = 0; i < theNumber; i++){

            //蛍複製
            ffTemp = Instantiate(fireFly, new Vector3(Random.Range(-randWidth, randWidth), Random.Range(-randDepth,randDepth), Random.Range(-randHeight, randHeight)), Quaternion.identity);
            
            //マテリアルを複製し、複製した蛍に適用
            ffTemp.GetComponent<Renderer>().material = Instantiate(ffMat);
            
            //蛍のリストに加える
            fireFlies.Add(ffTemp);
        }

	}
	
	// Update is called once per frame

	void Update () {
        //秩序変数　orderAbsが大きければ大きいほど同期していることを示す　ほぼデバッグ用
        orderRe = 0f;
        orderIm = 0f;
        orderAbs = 0f;

        for (int i = 0; i < fireFlies.Count; i++)
        {
            orderRe += Mathf.Cos(fireFlies[i].GetComponent<LightBlink>().GetTheta());
            orderIm += Mathf.Sin(fireFlies[i].GetComponent<LightBlink>().GetTheta());
        }

        orderAbs = Mathf.Sqrt(orderRe * orderRe + orderIm * orderIm)/fireFlies.Count;
		
	}

   


}
