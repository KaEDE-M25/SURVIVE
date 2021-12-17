using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGauge_Controll : MonoBehaviour
{
    [SerializeField]
    GameObject active_gauge;
    [SerializeField]
    GameObject damage_gauge;
    [SerializeField]
    GameObject empty_gauge;

    [SerializeField]
    GameObject parent;
    Character parent_component;
    public Tween damage_gauge_tween_red;
    public Tween damage_gauge_tween_grey;

    float timer = 0f;

    public void SetParent()
    {
        this.parent = transform.parent.gameObject;
        parent_component = parent.GetComponent<Character>();
        active_gauge = transform.Find("bar_hp_mini").gameObject;
        damage_gauge = transform.Find("bar_hp_damage_mini").gameObject;
        empty_gauge = transform.Find("bar_hp_empty_mini").gameObject;
    }

    private void Update()
    {
        if(timer >= 0f) 
        {
            timer -= Time.deltaTime;

            if (timer < 0f)
                Hide();
        }
    }

    private void LateUpdate()
    {
        transform.localScale = transform.parent.localScale;
    }


    // ゲージobjのゲッター
    public GameObject Active_gauge() { return active_gauge; }
    public GameObject Damage_gauge() { return damage_gauge; }
    public GameObject Empty_gauge() { return empty_gauge; }


    void Hide()
    {
        if (parent_component.HP > 0)
            this.gameObject.SetActive(false);
    }

    public void GaugeReduct(int reduction_value ,float time = 2f)
    {
        var value_from = (float)(parent_component.HP + reduction_value) / (float)parent_component.Max_HP;
        var value_to = (float)(parent_component.HP) / (float)parent_component.Max_HP;


        //アクティブゲージ減少
        active_gauge.transform.localScale = new Vector3(value_to,active_gauge.transform.localScale.y,active_gauge.transform.localScale.z);

        //赤ゲージ減少
        damage_gauge_tween_red = damage_gauge.transform.DOScaleX(value_to, time).SetEase(Ease.OutCirc);
        // 死亡時は空ゲージも徐々に消す、消し終わったらキャラクターを消す
        if (parent_component.HP <= 0)
            damage_gauge_tween_grey = empty_gauge.transform.DOScaleX(0, time).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                if (parent_component.HP <= 0)
                {
                    //damage_gauge_tween_red?.Kill();
                    //damage_gauge_tween_grey?.Kill();
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

