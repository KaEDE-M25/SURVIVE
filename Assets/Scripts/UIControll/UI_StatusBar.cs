using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--        UI上のステータスバーを操作するクラス        --
//--====================================================--
public class UI_StatusBar : MonoBehaviour
{
    // ターゲットとなるステータス
    enum TargetStatus
    {
        HP,
        MP
    }

    [SerializeField,Tooltip("このステータスバーが操作するステータス")]
    TargetStatus targetstatus;

    [SerializeField,Tooltip("アクティブゲージ")]
    Image active_gauge;
    [SerializeField,Tooltip("ダメージを表す所謂赤ゲージ")]
    Image damage_gauge;
    [SerializeField,Tooltip("バーの中央に表示されるテキスト(ステータスの現在値と最大値を数値で示す)")]
    Text center_text;

    // プレイヤーのFighterコンポーネント
    Fighters player_fighter_component;
    Tween damage_gauge_tween;

    //##====================================================##
    //##                  Awake   初期化処理                ##
    //##====================================================##
    private void Awake()
    {
        player_fighter_component = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighters>();
        center_text.text = TargetStatus_current() + "/" + TargetStatus_max();
    }

    //##====================================================##
    //##              ステータスの現在値を取得              ##
    //##====================================================##
    int TargetStatus_current() 
    {
        switch (targetstatus) 
        {
            case TargetStatus.HP: {return player_fighter_component.HP; }
            case TargetStatus.MP: {return player_fighter_component.MP; }
            default: { throw new System.Exception("Invalid Status.");}
        }
    }

    //##====================================================##
    //##              ステータスの最大値を取得              ##
    //##====================================================##
    int TargetStatus_max()
    {
        switch (targetstatus)
        {
            case TargetStatus.HP: { return player_fighter_component.Max_HP; }
            case TargetStatus.MP: { return player_fighter_component.Max_MP; }
            default: { throw new System.Exception("Invalid Status."); }
        }
    }

    //##====================================================##
    //##                ゲージを手動更新する                ##
    //##====================================================##
    public void RenewGaugeAmount()
    {
        center_text.text = TargetStatus_current() + "/" + TargetStatus_max();
        var value = (float)TargetStatus_current() / TargetStatus_max();
        active_gauge.fillAmount = value;
        damage_gauge.fillAmount = value;
    }

    //##====================================================##
    //##               ゲージを減少させる処理               ##
    //##====================================================##
    public void GaugeReduct(int reduction_value, float time = 1f) 
    {
        var value_from = (float)(TargetStatus_current() + reduction_value) / TargetStatus_max();
        var value_to = (float)TargetStatus_current() / TargetStatus_max();

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
