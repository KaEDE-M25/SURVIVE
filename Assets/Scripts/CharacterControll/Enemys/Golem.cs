using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 敵キャラ　ゴーレム                 --
//--====================================================--
public class Golem : Enemy
{
    // プレイヤーとの距離
    float dist_for_player;

    [SerializeField, Tooltip("死亡モーションの親obj")]
    GameObject dead_motion_parent;

    protected override CharacterStatus CharacterStatus()
    {
        // ノックバックしない
        cant_knockback = true;
        cant_knockback_weight = 10f;

        return EigenValue.STATUS_005_GOLEM.status;
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
    }
    //##====================================================##
    //##                   固有の行動処理                   ##
    //##====================================================##
    protected override void OriginalAction()
    {

        // 一定距離で止まる
        if (Distance(transform.position.x, player.transform.position.x) < 50f)
        {
            rb2d.velocity *= Vector2.up;
        }
        else // 距離外なら近づく
        {
            Move(my_status.Speed, player.transform.position.x > transform.position.x ? 1f : -1f);
            /*
            if (player.transform.position.x > transform.position.x)
            {
                // 遠すぎたら近づく
                if (dist_for_player >= 50f)
                    Move(Speed, 1f);
            }
            else if (player.transform.position.x < transform.position.x)
            {
                if (dist_for_player >= 50f)
                    Move(Speed, -1f);
            }
            */
        }
    }
    //##====================================================##
    //##      死亡モーション発生処理(Animationから呼出)     ##
    //##====================================================##
    void Dead_motion()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 0, b: 0, a: 0f);
        //transform.GetComponent<SpriteRenderer>().sprite = null;
        if (OptionData.current_options.dead_chara_color == 1)
        {

            for (int i = 0; i < dead_motion_parent.transform.childCount; i++)
            {
                ParticleSystem.MainModule par = dead_motion_parent.transform.GetChild(i).GetComponent<ParticleSystem>().main;
                par.startColor = EigenValue.TRANSPARENT_COLOR;
            }
        }

        dead_motion_parent.SetActive(true);
        transform.parent = GameObject.FindWithTag("GameController").GetComponent<GameControll>().Active_Effects_Parent().transform;
    }



    //##====================================================##
    //##        ゴーレムの特殊能力（衝撃波を止める）        ##
    //##====================================================##
    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (is_damage)
            if (collision.CompareTag("AttackEffect_Player"))
            {
                var rb2d = collision.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                    rb2d.velocity = Vector2.zero;

            }
    }




}
