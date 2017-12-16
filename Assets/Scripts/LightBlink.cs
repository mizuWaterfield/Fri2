using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class LightBlink : MonoBehaviour {

    //蛍の制御

    /*
     departure…指定速度まで加速(ランダムに選んだ蛍の方向へ徐々に加速)、制限速度になったらflyingへ遷移 ただしスタート地点の座標と目標物が近すぎる場合、いきなりarrivalに遷移することもある
     Flying…惰性運転。なお、途中で目標の蛍は変更しない　目標まで近づいたらarrivalへ遷移
     arrival…徐行(すぐに止まれる)スピードまで減速する　蛍の近くで速度ベクトルを0にし停止。stopへ遷移
     stop…静止状態。毎フレームdepartureに遷移するか判定する
     */
    
    

    public float blinkAmp = 4.5f; //点滅の強さ

    public float blinkSpeed; //点滅の速さ rad
    private float theta; //点滅の位相 rad

    public float distThreshold = 1.0f; //同期計算に入れる蛍の最高距離(この距離離れている蛍まで考慮する、ということ)
    public bool isSync = true; // 同期するかしないか

    public float transProbability=0.00f; //stopからdepartureへの遷移確率
    public float numNear = 0.0f;

    public List<GameObject> fireFlies; //蛍のリスト

    private Vector3 goal, currentPosition; //目標とする蛍、自分の現在位置
    private Vector3 velocity = Vector3.zero;//自分の移動速度
    private float flyingSpeedLimit = .2f; //制限速度
    private float arrivalThreshold = 10.0f; //減速を始める距離
    private float stopThreshold = 0.1f; //止まる距離


    
    //ステート
    public enum PlayerState
    {
        None = 0,
        Flying,
        Departure,
        Arrival,
        Stop
    }

    //現在のステートと次のステートを用意
    public PlayerState NowState = PlayerState.Stop;
    public PlayerState NextState = PlayerState.None;

	void Start () {

        //you have to use class name to call static member
        fireFlies = FireFlyCreator.fireFlies;

        //弧度法で点滅の速さ・位相を決める
        blinkSpeed = RandNormal(3.0f,5.0f) *Mathf.PI/180.0f;
        theta = Random.Range(0f, 360f) * Mathf.PI / 180.0f;

        //速度の初期化
        velocity = Vector3.zero;

    }
	
	
	// Update is called once per frame
    void Update()
    {

        //蛍の点滅の同期を行う

        //初期化
        float tempSum = 0.0f; //distThreshold以下の距離のホタルとの点滅位相の差の和
        float tempCnt = 0.0f; //distThreshold以下の距離のホタルの数


        for (int i = 0; i < fireFlies.Count; i++) {

            //ある蛍と自分の距離を計算
            float tempDist = Vector3.Distance(fireFlies[i].transform.position, this.transform.position);
            //他人と自分の位相差をもとに加算　加算するかどうかは距離の大きさによる
            if (tempDist < distThreshold) {
                float K = (distThreshold - tempDist) / distThreshold; // 距離ごとに加算する位相差に重み付け 近いほど1,限界距離なほど0
                tempCnt += 1.0f;
                tempSum += K * Mathf.Sin(fireFlies[i].GetComponent<LightBlink>().GetTheta() - this.theta); // 位相差を評価関数 K*sin(dTheta)に入れて和を取る
            }

        }
        numNear = tempCnt;

        //重みと考慮した蛍の数による係数を乗算
        tempSum *= FireFlyCreator.syncCoefficient / tempCnt;


        //点滅速度に反映　Time.deltaTimeをかけることにより点滅速度の次元は[rad/s]となる
        //if (isSync){
        //    blinkSpeed += tempSum * Time.deltaTime;
        //}

        //点滅速度を位相に足す
        //theta += blinkSpeed;
        theta = theta + blinkSpeed + tempSum;

        //位相は0~2piの値を取る
        theta %= (2.0f * Mathf.PI);
        //if(theta > 2.0f*Mathf.PI){
        //    theta = theta - 2.0f*Mathf.PI;
        //}

        //点滅はシェーダのEmissionで表現している
        float tempCol = blinkAmp * (1.0f + Mathf.Sin(theta));
        this.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color( 0.8f*tempCol, tempCol, 0, 1f));

         

        //ここから位置の計算

        currentPosition = this.transform.position;
        currentPosition += velocity;
        this.transform.position = currentPosition;


        //このブロックでステートを遷移するかどうか判定する
        //遷移したくなかったらNoneのままでok
        if (NextState == PlayerState.None)
        {
            switch (NowState)
            {
                case PlayerState.Flying:
                    if ((goal - currentPosition).magnitude < arrivalThreshold){
                        
                        //目標に近づいたら遷移
                        NextState = PlayerState.Arrival;
                    }
                    break;

                case PlayerState.Departure:
                    if((goal - currentPosition).magnitude < arrivalThreshold){
                        
                        //目標に近づいたら遷移
                        NextState = PlayerState.Arrival;

                    }else if (velocity.magnitude > flyingSpeedLimit){
                        //制限速度に達したら遷移、一定速度に
                        NextState = PlayerState.Flying;
                    }
                    break;

                case PlayerState.Arrival:
                    if (velocity.magnitude < stopThreshold) {

                        //目標のすぐ近くまで到達したら停止
                        velocity = Vector3.zero;

                        //遷移する
                        NextState = PlayerState.Stop;
                    }

                    break;


                case PlayerState.Stop:

                    //遷移確率に則り、移動するか判定
                    if (Random.Range(0.0f, 1.0f) <= transProbability) {
                        int i = Random.Range(0, fireFlies.Count);
                        
                        //ある程度離れた蛍を目指す　当然自分自身に移動することはありえない
                        if (Vector3.Distance(fireFlies[i].transform.position, this.transform.position) > arrivalThreshold)
                        {
                            //目標地点を設定
                            goal = fireFlies[i].transform.position;
                            
                            //速度ベクトルは目標地点に対し平行である
                            velocity = (goal - this.transform.position);

                            //大きさの調整
                            velocity /= velocity.magnitude * 10.0f;
                            
                            //遷移
                            NextState = PlayerState.Departure;
                        }

                    }
                    break;
             
            }
        }

        //このブロックはステートが切り替わった時に一度だけ実行される
        if (NextState != PlayerState.None)
        {

            //現在のステートを更新
            NowState = NextState;
            NextState = PlayerState.None;

            switch (NowState)
            {

                case PlayerState.Flying:
                    break;

                case PlayerState.Departure:
                    
                    break;

                case PlayerState.Arrival:
                    break;

                case PlayerState.Stop:
                    
                    break;
              
            }

        }

            //ステートの実行
	    switch(NowState){
		    case PlayerState.Flying:
		    break;

            case PlayerState.Departure:
                //加速
                velocity *= 1.01f;
            break;

            case PlayerState.Arrival:
                //減速
                velocity /= 1.02f;
            break;

		    case PlayerState.Stop:

		    break;
		
	    }
    }

    //位相を返す
    public float GetTheta() {
        return this.theta;
    }

    //正規分布に従う乱数　ボックスミュラー法による
    float RandNormal(float mu, float sigma)
    {
        float z = Mathf.Sqrt(-2.0f * Mathf.Log(Random.Range(0.0f, 1.0f))) * Mathf.Sin(2.0f * Mathf.PI * Random.Range(0.0f, 1.0f));
        return mu + sigma * z;
    }
		
}



    



