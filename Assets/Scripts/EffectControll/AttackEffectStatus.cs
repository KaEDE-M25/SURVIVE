using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//--====================================================--
//--           攻撃エフェクトが持つステータス           --
//--====================================================--
public class AttackEffectStatus : MonoBehaviour
{
    // 攻撃のノックバック威力
    public float knockback_weight;
    // 攻撃のノックバック角度
    public float knockback_angle;
    // ダメージ
    public int damage;
    // ダメージエフェクト (流血表現など)
    public GameObject effect;
}
