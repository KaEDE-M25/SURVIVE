using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//##====================================================##
//##                 敵キャラ　ゴースト                 ##
//##====================================================##
public class Ghost : Enemy
{
    [SerializeField,Tooltip("子オブジェクトについている死亡パーティクル")]
    GameObject ghost_dead_particle;


    protected override CharacterStatus CharacterStatus()
    {
        // 飛行キャラ
        is_fly_chara = true;

        return EigenValue.STATUS_002_GHOST.status;
    }


    //##====================================================##
    //##                   固有の行動処理                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        // ゴースト特有の行動処理
        if (!is_damage && player != null)
        {
            if (Distance(transform.position.x, player.transform.position.x) > 3f)
                Move(my_status.Speed, player);
        }
    }

    //##====================================================##
    //##            Startで行うステータス初期化             ##
    //##====================================================##
    protected override void Initialize() { }


    //##====================================================##
    //##        アイテムドロップ処理                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
        // 確率で回復ポーションをドロップ
        if (Random.Range(0, 100) == 0)
        {
            Drop("MP_Potion", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        }


    }

    //##====================================================##
    //##             死亡処理(Animationから呼出)            ##
    //##====================================================##
    public override void Dead()
    {
        ghost_dead_particle.SetActive(true);
        ghost_dead_particle.transform.parent = GameObject.FindWithTag("GameController").GetComponent<GameControll>().Active_Effects_Parent().transform;

        base.Dead();
    }


}
