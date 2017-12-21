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

        for (int i = 0; i < theNumber; i++)
        {

            // 自分の位置から見て指定された範囲内に蛍を複製
            //ffTemp = Instantiate(fireFly, this.transform.position + new Vector3(Random.Range(-randWidth, randWidth), Random.Range(-randDepth,randDepth), Random.Range(-randHeight, randHeight)), Quaternion.identity);

            // 自分の位置から適当にrayを飛ばしてぶつかったところにホタルを作る
            Vector3 way = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Ray ray = new Ray(this.transform.position, way); //+new Vector3(Random.Range(-50,50),Random.Range(-5,5),Random.Range(-50,50))
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500f))
            {   // ヒットしたらそこにコピーを作成
                ffTemp = Instantiate(fireFly, hit.point + new Vector3( 0f, Random.Range(-randDepth, randDepth), 0f) , Quaternion.identity); // 高さをランダムに加算
            }
            else
            {   // ヒットしなかったらなにもしない
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
