using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System.Collections;
using UnityEngine.UI;

public class red_move : Agent
{
    Rigidbody rBody;
    public GameObject enemy;
    public GameObject score;
    public Image image;
    public float lapsetime;
    public float boost;
    
    // 初期化時に呼ばれる
    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
    }

    // 観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,enemy.transform.localPosition));
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition,Vector3.zero));
        sensor.AddObservation(Vector3.Distance(enemy.transform.localPosition,Vector3.zero));
        sensor.AddObservation(lapsetime);
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actions) {
        Vector3 dirToGo = Vector3.zero;
        int action = actions.DiscreteActions[0];

        //ブーストがある
        if (2<lapsetime && lapsetime<6)
        {
            //移動
            if (action == 1) dirToGo = transform.forward;
            if (action == 2) dirToGo = transform.forward * -1.0f;
            if (action == 3) dirToGo = transform.up;
            if (action == 4) dirToGo = transform.up * -1.0f;
            rBody.AddForce(dirToGo * 3.0f, ForceMode.VelocityChange);
            //攻撃
            if (action == 6) 
            {
                this.tag = "attack";
            }
        }
        //着地
        if (3<lapsetime)
        {
            if (action == 5) 
                {
                    lapsetime = -8.0f;
                }
        }
    }

    void Update()
    {
        transform.LookAt(enemy.transform, Vector3.forward);
        transform.Rotate(new Vector3(-180f, -180f, +90f));
        rBody.velocity = Vector3.zero;
        //ブーストの状態
        if (0<=lapsetime && lapsetime<=6)
            boost = 1.0f-lapsetime/6;

        lapsetime += 1.0f;
        image.fillAmount = boost;
    }

    //OnTriggerEnter関数
    //接触したオブジェクトが引数otherとして渡される
    void OnTriggerEnter(Collider other)
    {
            //接触したオブジェクトのタグ
        if (other.CompareTag("attack"))
        {
            score.tag = "win_blue";
            this.AddReward(-1f);
            EndEpisode();
        }
    }

    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow)) actions[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow)) actions[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow)) actions[0] = 3;
        if (Input.GetKey(KeyCode.RightArrow)) actions[0] = 4;
        if (Input.GetKey(KeyCode.Space)) actions[0] = 5;
        if (Input.GetKey(KeyCode.S)) actions[0] = 6;
    }
}