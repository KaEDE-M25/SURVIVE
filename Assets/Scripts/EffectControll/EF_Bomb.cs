using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--            エフェクト：ばくだんの爆発              --
//--====================================================--
public class EF_Bomb : MonoBehaviour
{
    // 子にある爆発オブジェクト
    [SerializeField]
    GameObject explosion_effect;

    // 投擲から爆発までにかかる時間
    public static readonly float EXPLODE_TIME = 3f;

    // 時間計測カウンター
    float time_count = 0f;

    void Update()
    {
        time_count += Time.deltaTime;

        // 爆発時間になったら
        if (time_count >= EXPLODE_TIME) 
        {
            // 爆発エフェクトを自身から外して表示開始
            explosion_effect.transform.parent = transform.parent;
            explosion_effect.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
