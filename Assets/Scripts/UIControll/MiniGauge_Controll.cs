using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--キャラクターについている小HPゲージの操作をするクラス--
//--====================================================--
public class MiniGauge_Controll : MonoBehaviour
{
    // アクティブなゲージ
    public GameObject Active_gauge { get; private set; }
    // ダメージ分のゲージ
    public GameObject Damage_gauge { get; private set; }
    //空ゲージ
    public GameObject Empty_gauge { get; private set; }
    //ゲージを持っているキャラクターのCharacterコンポーネント
    Character parent_component;

    float timer = 0f;

    //##====================================================##
    //##       初期化処理（キャラクター側から呼び出す）     ##
    //##====================================================##
    public void SetParent()
    {
        parent_component = transform.parent.GetComponent<Character>();
        Active_gauge = transform.Find("bar_hp_mini").gameObject;
        Damage_gauge = transform.Find("bar_hp_damage_mini").gameObject;
        Empty_gauge = transform.Find("bar_hp_empty_mini").gameObject;
    }

    //##====================================================##
    //##                Update    表示終了処理              ##
    //##====================================================##
    private void Update()
    {
        if(timer >= 0f) 
        {
            timer -= Time.deltaTime;

            if (timer < 0f)
                Hide();
        }
    }

    //##====================================================##
    //##              LateUpdate      向きの固定            ##
    //##====================================================##
    private void LateUpdate()
    {
        // 向きを固定する
        transform.localScale = transform.parent.localScale;
    }

    //##====================================================##
    //##                   ゲージを隠す処理                 ##
    //##====================================================##
    void Hide()
    {
        if (parent_component.HP > 0)
            this.gameObject.SetActive(false);
    }

    //##====================================================##
    //##                ゲージを減少させる処理              ##
    //##====================================================##
    public void GaugeReduct(int reduction_value ,float time = 2f)
    {
        var value_from = (float)(parent_component.HP + reduction_value) / (float)parent_component.Max_HP;
        var value_to = (float)(parent_component.HP) / (float)parent_component.Max_HP;

        //アクティブゲージ減少
        Active_gauge.transform.localScale = new Vector3(value_to,Active_gauge.transform.localScale.y,Active_gauge.transform.localScale.z);

        //赤ゲージ減少
        Damage_gauge.transform.DOScaleX(value_to, time).SetEase(Ease.OutCirc);
        // 死亡時は空ゲージも徐々に消す、消し終わったらキャラクターを消す
        if (parent_component.HP <= 0)
            Empty_gauge.transform.DOScaleX(0, time).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                if (parent_component.HP <= 0)
                {
                    parent_component.Dead();
                }
            });
        //4秒後にゲージを隠す
        if (parent_component.HP > 0)
        {
            timer = 4f;
        }
    }

}