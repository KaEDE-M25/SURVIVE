using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_StatusBar : MonoBehaviour
{
    enum TargetStatus
    {
        HP,
        MP
    }

    [SerializeField]
    TargetStatus targetstatus;

    [SerializeField]
    Image active_gauge;
    [SerializeField]
    Image damage_gauge;
    // バーの中央に表示されるテキスト
    [SerializeField]
    Text center_text;

    Fighters player_fighter_component;
    Tween damage_gauge_tween;

    // ステータスの現在値を取得
    int TargetStatus_current() 
    {
        switch (targetstatus) 
        {
            case TargetStatus.HP: {return player_fighter_component.HP; }
            case TargetStatus.MP: {return player_fighter_component.MP; }
            default: { throw new System.Exception("Invalid Status.");}
        }
    
    }

    // ステータスの最大値を取得
    int TargetStatus_max()
    {
        switch (targetstatus)
        {
            case TargetStatus.HP: { return player_fighter_component.Max_HP; }
            case TargetStatus.MP: { return player_fighter_component.Max_MP; }
            default: { throw new System.Exception("Invalid Status."); }
        }

    }


    private void Awake()
    {
        var player = GameObject.Find("Knight");
        player_fighter_component = player.GetComponent<Fighters>();
        center_text.text = TargetStatus_current() +"/"+ TargetStatus_max();
        

    }

    // ゲージを手動更新する
    public void RenewGaugeAmount()
    {
        center_text.text = TargetStatus_current() + "/" + TargetStatus_max();
        var value = (float)(TargetStatus_current()) / (float)TargetStatus_max();
        active_gauge.fillAmount = value;
        damage_gauge.fillAmount = value;
    }


    // ゲージを減少させる処理
    public void GaugeReduct(int reduction_value, float time = 1f) 
    {
        var value_from = (float)(TargetStatus_current() + reduction_value) / (float)TargetStatus_max();
        var value_to = (float)(TargetStatus_current()) / (float)TargetStatus_max();

        center_text.text = TargetStatus_current() + "/" + TargetStatus_max();

        //アクティブゲージ減少
        active_gauge.fillAmount = value_to;

        // 赤ゲージが減少中だったら止める
        if (damage_gauge_tween != null)
            damage_gauge_tween.Kill();

        //赤ゲージ減少
        damage_gauge_tween = DOTween.To(
            () => value_from,
            x =>
            {
                damage_gauge.fillAmount = x;
            },
            value_to,
            time
            ).SetEase(Ease.OutCirc);
        
    }
}
