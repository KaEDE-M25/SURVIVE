using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//##====================================================##
//##                 敵キャラ　スケルトン               ##
//##====================================================##
public class Skeleton : Enemy
{
    // 骨攻撃の間隔（フレーム単位）
    const int THROWBONE_INTERVAL = 300;

    // 骨エフェクトのレンダラ
    Renderer bone_renderer;
    // プレイヤーとの距離
    float dist_for_player;
    // 骨攻撃のインターバルを計算するためのカウンタ
    int frame_count = 300;
    [SerializeField, Tooltip("骨攻撃エフェクトのobj")]
    GameObject bone;
    [SerializeField, Tooltip("死亡モーションの親obj")]
    GameObject dead_motion_parent;

    protected override CharacterStatus CharacterStatus()
    {
        // ステータス初期化値
        return EigenValue.STATUS_001_SKELETON.status;
    }

    //##====================================================##
    //##            Startで行うステータス初期化             ##
    //##====================================================##
    protected override void Initialize() 
    {

        //bone = transform.Find("EF_skeleton_bone").gameObject;
        bone_renderer = bone.GetComponent<Renderer>();
        //dead_motion_parent = transform.Find("EF_Skeleton_dead").gameObject;

        bone.transform.parent = GameObject.FindWithTag("GameController").GetComponent<GameControll>().Active_Effects_Parent().transform;

    }

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

        // 骨を投げてる途中なら
        if (bone.activeSelf)
        {
            if (!bone_renderer.isVisible)
            {
                bone.SetActive(false);
                //bone.transform.parent = this.transform;
            }

        }

        if (!is_damage && player != null)
        {
            dist_for_player = Distance(transform.position.x, player.transform.position.x);
            if (dist_for_player > 100f && dist_for_player < 130f)
            {
                // 動きを停止
                if (!is_jump)
                    rb2d.velocity = Vector2.zero;
                // プレイヤーの方を向く
                Focus(player);

                // カウント
                frame_count++;

                // 一定間隔ごとに骨攻撃
                if (frame_count > THROWBONE_INTERVAL)
                {
                    BoneAttack();
                    frame_count = 0;
                }

            }
            else
            {
                Move(my_status.Speed, player.transform.position.x > transform.position.x ^ dist_for_player >= 130f ? -1f : 1f);

                /*
                ※ player.transform.position.x > transform.position.x ^ dist_for_player >= 130f ? -1f : 1f
                は、以下を簡略化したもの。

                // プレイヤーが右、敵が左にいるなら
                if (player.transform.position.x > transform.position.x)
                {
                    // 遠すぎたら近づく

                    if (dist_for_player >= 130f)
                        Move(Speed, 1f);
                    else
                        Move(Speed, -1f);
                }
                else if (player.transform.position.x < transform.position.x)
                {
                    if (dist_for_player >= 130f)
                        Move(Speed, -1f);
                    else
                        Move(Speed, 1f);
                }
                */
            }
        }
        
    }


    //##====================================================##
    //##                スケルトンで使う処理                ##
    //##====================================================##
    // 骨攻撃 (Animを開始する)
    void BoneAttack() 
    {
        Anim.Play("throwbone");
    }
    // 骨の攻撃エフェクトを飛ばす（Animationから呼び出す）
    void Throw_bone()
    {
        bone.transform.position = transform.position;
        bone.SetActive(true);
        bone.GetComponent<Rigidbody2D>().velocity = new Vector2((-150f + Random.Range(-50f,50f)) * transform.localScale.x,400f);
    }

    // 死亡モーション(体がばらばらになるエフェクト)を発生
    void Dead_motion()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(r:0,g:0,b:0,a:0f);
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
    //##             死亡処理(Animationから呼出)            ##
    //##====================================================##
    public override void Dead()
    {
        // 攻撃エフェクトの親をこのキャラクターに戻す（Destroyでまとめて消すため）
        bone.SetActive(false);
        bone.transform.parent = this.transform;

        base.Dead();
    }

}
