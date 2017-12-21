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
    public static float syncCoefficient = 0.7f; //同期する重み係数
    [SerializeField]
    private float randWidth = 30f; //蛍の生成範囲(X軸)
    [SerializeField]
    private float randDepth = 10f; //蛍の生成範囲(Y軸)
    [SerializeField]
    private float randHeight = 30f; //蛍の生成範囲(Z軸)

    public int theNumber = 10;

    //同期ぐあいを示す
    public float orderSigma;

    // Use this for initialization
    void Start () {

        

        fireFlies.Add(fireFly); // コピー元もリストに加える

        for (int i = 0; i < theNumber; i++){

            while (true)
            {
                //障害物にあたるまで、ランダムな方向へrayを発射し続ける
                Ray ray = new Ray(this.transform.position+new Vector3(Random.Range(-randWidth,randWidth),Random.Range(-randDepth,randDepth),Random.Range(-randHeight,randHeight)), new Vector3(Random.Range(-1,1),Random.Range(-1,1),Random.Range(-1,1)));
                RaycastHit hit;
                //障害物に当たったらそこに生成
                if (Physics.Raycast(ray, out hit, 500f)) {
                    ffTemp = Instantiate(fireFly, hit.point, Quaternion.identity);
                    Debug.Log(hit.point);
                    break;
                }


            }

            //マテリアルを複製し、複製した蛍に適用
            ffTemp.GetComponent<Renderer>().material = Instantiate(ffMat);
            
            //蛍のリストに加える
            fireFlies.Add(ffTemp);
        }

	}
	
	// Update is called once per frame

	void Update () {

        // orderSigmaはtheta値の分散度
        float tempAve = 0f;
        float tempDiffSum = 0f;

        for (int i = 0; i < fireFlies.Count; i++)
        {
            tempAve += fireFlies[i].GetComponent<LightBlink>().GetTheta();
        }

        tempAve = tempAve / fireFlies.Count;
        for (int i = 0; i < fireFlies.Count; i++)
        {
            tempDiffSum += Mathf.Pow( (fireFlies[i].GetComponent<LightBlink>().GetTheta() - tempAve), 2);
        }
        orderSigma = tempDiffSum / fireFlies.Count;
		
	}

   


}
