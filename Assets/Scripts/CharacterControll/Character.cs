using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                   キャラクター                     --
//--====================================================--
public abstract class Character : MonoBehaviour
{
    // 固定値
    protected const float BASE_JUMP_FORCE = 1000000f;



    // 自身が持つコンポーネント (初期化時に設定)
    protected Rigidbody2D rb2d;
    public Animator Anim { get; protected set; }
    protected Collider2D coli;
    protected SpriteRenderer spre;

    // GameControllコンポーネント
    protected GameControll game_controller;

    // 点滅処理用タイマー
    float time = 0f;

    // 元レイヤー(ノックバック用レイヤーから戻るのに利用)
    protected int my_layer;

    // 汎用エフェクトの名前
    string crush_effect_horizontal;
    string crush_effect_vertical;

    // キャラクターに付属しているミニHPバー
    public MiniGauge_Controll Mini_hp_bar { get; protected set; }

    // 〇〇しているかの判定変数
    #region if_xxx

    // ジャンプ中かどうか
    [SerializeField]
    protected bool is_jump;
    // ダメージを受けているかどうか
    protected bool is_damage;
    // ノックバック中かどうか
    protected bool is_knockback;
    // ノックバック中に壁に１回激突したかどうか
    protected bool is_knockback_crush;
    // 倒れた状態かどうか
    protected bool is_tumble;
    // 死んだかどうか
    protected bool is_dead;
    // 点滅処理が起きてるかどうか
    protected bool is_blink;

    #endregion

    // キャラクター特性 (基礎動作に直接干渉する特殊能力の定義)(prefab上で設定)
    #region characteristics
    // 攻撃を食らった時の無敵時間 (秒)
    protected float wakeup_time = 0f;
    // 飛行するキャラかどうか
    protected bool is_fly_chara = false;
    // 吹っ飛ばない
    protected bool cant_knockback = false;
    // 吹っ飛ばない場合ののけぞりの大きさ
    protected float cant_knockback_weight = 100f;
    #endregion

    // 各種ステータス (可変する／させるものは追加で定義)
    #region status

    // ステータスをまとめたクラス
    protected CharacterStatus my_status = null;

    // 体当たり威力（ゾンビとか用）（ノックバック時に0にする）
    public int Touch_damage { get; protected set; }
    // 最大HP
    public int Max_HP { get; set; }
    // 最大MP
    public int Max_MP { get; set; }
    // 現在HP
    public int HP { get; set; }
    // 現在MP
    public int MP { get; set; }

    #endregion




    //##====================================================##
    //##               Awake ステータスの初期化             ##
    //##====================================================##
    private void Awake()
    {
        // ステータス初期化
        Status_Initialize(CharacterStatus());
    }


    //##====================================================##
    //##                    Update 基礎動作                 ##
    //##====================================================##
    protected void Update()
    {
        // 貫通したりして無限落下したらy座標を初期化する(付け焼刃)
        if (transform.position.y <= -10000f)
            transform.position *= Vector2.right;


        // ジャンプ中なら
        if (is_jump)
        {
            // 飛行キャラではないなら
            if (!is_fly_chara)
                rb2d.gravityScale = 100;　// 重力スケールを設定
        }
        else
        {
            // 地上では重力を調整
            rb2d.gravityScale = 0f;
            // velocityはxのみ反映
            rb2d.velocity *= Vector2.right;
        }
        if (!is_dead)
        {
            // 各キャラ固有アクション
            OriginalAction();

            // 点滅処理中なら点滅
            if (is_blink)
                Blink();
        }
        // Animatorに値を渡す
        Set_AnimValue();

    }


    //##====================================================##
    //##             Particleに当たった時の処理             ##
    //##====================================================##
    private void OnParticleCollision(GameObject target)
    {
        Damage(target);
    }


    //##====================================================##
    //##            Collisionに当たった時の処理             ##
    //##====================================================##
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!is_dead)
        {
            // 地面に当たった際は激突処理
            if (collision.collider.CompareTag("Ground"))
            {
                // ノックバック中なら
                if (is_knockback)
                {
                    // 床に激突した場合（壁に１回は激突している）
                    if (is_knockback_crush)
                    {
                        // 倒れる処理をしていないなら倒れる処理を始める
                        if (!is_tumble)
                        {
                            if (rb2d.velocity.y == 0)
                            {
                                // 効果音を再生
                                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.CRUSH, spre);
                                Tumble();
                            }
                        }

                    }
                    else // 壁に激突した場合（はじめての激突）
                    {
                        // 効果音を再生
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.CRUSH, spre);
                        // 地面に当たっていないと考えられる(y座標の移動が止まっていない場合)なら壁激突処理
                        if (rb2d.velocity.y != 0)
                            Crush_Horizontal();
                        else //地面に当たっていると考えられる(y座標の移動が止まっている場合)なら倒れる処理
                        {
                            Tumble();
                            is_knockback_crush = true;
                        }
                    }
                }
                else if (rb2d.velocity.x != 0) // 壁に当たっていないと考えられる(x座標の移動が止まっている)なら着地処理
                {
                    is_jump = false;
                }

            }
            // それ以外の場合はキャラクターに当たった場合なのでダメージ処理
            else
            {
                Damage(collision);
            }
        }
    }


    //##====================================================##
    //##              Triggerに当たった時の処理             ##
    //##====================================================##
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵もしくはプレイヤーのエフェクトだった場合はダメージ処理
        if (collision.transform.CompareTag("AttackEffect_Player") || collision.transform.CompareTag("AttackEffect_Enemy"))
            Damage(collision);

    }


    //##====================================================##
    //##           Collisionに当たっている時の処理          ##
    //##====================================================##
    protected void OnCollisionStay2D(Collision2D collision)
    {
        // 地面・壁に当たっているなら
        if (collision.collider.CompareTag("Ground"))
        {
            // 地面に当たっていると考えられる(y座標の移動が止まっている)なら
            if (rb2d.velocity.y == 0)
            {
                is_jump = false;
                // のけぞり中(ダメージは受けているがノックバックしていない)だったら
                if (!is_knockback && is_damage)
                {
                    is_damage = false;
                    //is_knockback = false;
                    // 点滅処理
                    Blink(wakeup_time);
                }
                // 倒れている処理中なら
                if (is_tumble)
                {
                    // ｽﾞｻﾞｰ
                    rb2d.velocity = new Vector2(rb2d.velocity.x / 1.2f, rb2d.velocity.y);
                }
                else
                {
                    // １回は壁に激突しているなら
                    if (is_knockback_crush)
                    {
                        // 効果音を再生
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.CRUSH, spre);
                        // 倒れる処理
                        Tumble();
                    }
                }
            }
            // それ以外の場合は壁に激突していると考えられる場合になる
            else
            {
                /*if (rb2d.velocity.y == 0 && Anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_up"))
                {
                    is_jump = false;
                }*/
                // ノックバック中でかつ１回目の壁激突の場合
                if (is_knockback)
                {
                    if (/*is_jump && */!is_knockback_crush && Mathf.Abs(rb2d.velocity.x) <= 0.01f)
                    {
                        // 効果音を再生
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.CRUSH, spre);
                        Crush_Horizontal();
                    }
                }
                /*else if (!is_jump)
                {
                    rb2d.velocity *= Vector2.right;
                }*/

            }
        }
    }

    //##====================================================##
    //##             Collisionから離れた時の処理            ##
    //##====================================================##
    protected void OnCollisionExit2D(Collision2D collision)
    {
        // 地面から離れたらジャンプしたと考えられる
        if (collision.collider.CompareTag("Ground"))
        {
            is_jump = true;
        }
    }


    //--====================================================--
    //--      キャラクターのステータスを返す初期化関数      --
    //--           （派生メソッドで必ず定義する）           --
    //--====================================================--
    protected abstract CharacterStatus CharacterStatus();


    //--====================================================--
    //--          値を初期化する処理 (Awakeで呼ぶ)          --
    //--====================================================--
    protected virtual void Status_Initialize(CharacterStatus status)
    {
        // キャラクターステータス
        my_status = status;
        this.Touch_damage = status.Touch_damage;
        this.Max_HP = status.Max_HP;
        this.Max_MP = status.Max_MP;
        this.HP = status.Max_HP;
        this.MP = status.Max_MP;

        // コンポーネントの取得
        rb2d = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        coli = GetComponent<Collider2D>();
        spre = GetComponent<SpriteRenderer>();
        //spre = GetComponent<Renderer>();
        game_controller = GameObject.FindWithTag("GameController").GetComponent<GameControll>();


        // 内部値の初期化
        is_jump = true;
        is_damage = false;
        is_knockback = false;
        is_knockback_crush = false;
        is_tumble = false;
        is_dead = false;

        // レイヤー番号の取得
        my_layer = transform.gameObject.layer;

        // 汎用エフェクトの名称取得

        crush_effect_horizontal = "EF_Crush_horizontal";
        crush_effect_vertical = "EF_Crush_vertical";


        // ミニHPバーを設定
        var res = transform.Find("hp_bar_mini");
        Mini_hp_bar = res != null ? res.GetComponent<MiniGauge_Controll>() : null;
        if (Mini_hp_bar != null) Mini_hp_bar.SetParent();
    }


    //--====================================================--
    //--                ジャンプをする処理                  --
    //--====================================================--
    protected void Jump(float jump_power)
    {
        is_jump = true;
        rb2d.AddForce(Vector2.up * jump_power * BASE_JUMP_FORCE);
    }

    //--====================================================--
    //--              Animatorに値を渡す処理                --
    //--====================================================--
    protected virtual void Set_AnimValue()
    {
        Anim.SetFloat("velocity.x", Mathf.Abs(rb2d.velocity.x));
        Anim.SetFloat("velocity.y", rb2d.velocity.y);
        Anim.SetBool("jump", is_jump);
        Anim.SetBool("damage", is_damage);
        Anim.SetBool("knockback", is_knockback);
        Anim.SetBool("knockback_crush", is_knockback_crush);
        Anim.SetBool("tumble", is_tumble);
        Anim.SetBool("dead", is_dead);

    }

    //--====================================================--
    //--                   移動をする処理                   --
    //--====================================================--

    // 特定の相手への直線移動
    protected void Move(float speed, GameObject target)
    {
        // ダメージを受けている途中でないなら
        if (!is_damage)
        {
            Vector2 vec;

            vec.x = target.transform.position.x - transform.position.x;
            vec.y = target.transform.position.y - transform.position.y;
            vec = vec.normalized * speed;

            // 座標を移動
            rb2d.velocity = vec;

            // 対象へ向く
            Focus(target);

            /*
            if (target.transform.position.x - transform.position.x > 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
            */
        }

    }

    // 向きを指定した水平直線移動
    protected void Move(float speed, float direction)
    {
        // ダメージを受けている途中でないなら
        if (!is_damage)
        {
            Vector2 vec;
            vec.x = speed * direction;
            vec.y = rb2d.velocity.y;

            // 座標を移動
            rb2d.velocity = vec;

            // 移動方向を向く
            Focus(direction);

            /*
            //　向きを決定
            if (direction < 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction > 0)
                transform.localScale = new Vector3(-1, 1, 1);
            */
        }

    }

    //--====================================================--
    //--               ダメージを受ける処理                 --
    //--====================================================--
    // collision（キャラ同士の接触の場合）
    protected void Damage(Collision2D collision)
    {
        if (!is_damage)
        {
            // タグが違う(Player同士 Enemy同士の場合はダメージ判定はなしとする)
            if (!coli.CompareTag(collision.collider.tag))
            {
                if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
                //キャラクター同士の接触
                {
                    var enemy_coli = collision.gameObject.GetComponent<Character>();
                    // ダメージがあるなら
                    if (enemy_coli.Touch_damage > 0)
                    {
                        is_damage = true;
                        // HPを減らす処理
                        ReductHP(enemy_coli.Touch_damage);

                        // 敵の方を向く　(向いた後に後ろにノックバック)
                        Focus(collision.gameObject);
                        // ノックバックを発生させる処理
                        Knockback(150f, 45f, false);

                        // 効果音を再生
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.DAMAGE, spre);

                        // 点滅処理
                        Blink(wakeup_time);

                    }


                }
            }
        }
    }

    // collider (攻撃エフェクトの場合)
    protected void Damage(Collider2D collider)
    {
        if (!is_damage)
        {
            // 自分と同じ勢力ではない攻撃エフェクトだった場合
            if (!collider.CompareTag("AttackEffect_" + transform.tag))
            {
                AttackEffectStatus attack_st = collider.transform.GetComponent<AttackEffectStatus>();
                if (attack_st.damage > 0)
                {
                    is_damage = true;
                    ReductHP(attack_st.damage);
                }

                // 効果音を再生
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.DAMAGE, spre);

                if (!is_dead)
                {
                    // 敵の方を向く　(向いた後に後ろにノックバック)
                    Focus(collider.gameObject);
                    Knockback(attack_st.knockback_weight + Random.Range(-50f, 50f), attack_st.knockback_angle + Random.Range(-0.3f, 0.3f));

                    bool mirror = false;
                    if (transform.localScale.x > 0) mirror = true;

                    Play_Effect(attack_st.effect.name, Vector2.zero, mirror);
                }

            }
        }
    }

    // particle system (パーティクルの場合（プレイヤーのMP攻撃など）)
    protected void Damage(GameObject particle)
    {
        if (!is_damage)
        {
            // 自分と同じ勢力ではない攻撃エフェクトだった場合
            if (!particle.CompareTag("AttackEffect_" + transform.tag))
            {
                AttackEffectStatus attack_st = particle.GetComponent<AttackEffectStatus>();
                if (attack_st.damage > 0)
                {
                    is_damage = true;
                    ReductHP(attack_st.damage);
                }

                // 効果音を再生
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.DAMAGE, spre);

                if (!is_dead)
                {
                    // 敵の方を向く　(向いた後に後ろにノックバック)
                    Focus(particle);
                    Knockback(attack_st.knockback_weight + Random.Range(-50f, 50f), attack_st.knockback_angle + Random.Range(-0.3f, 0.3f));

                    bool mirror = false;
                    if (transform.localScale.x > 0) mirror = true;

                    Play_Effect(attack_st.effect.name, Vector2.zero, mirror);
                }

            }
        }
    }


    //--====================================================--
    //--                HPを増減させる処理                  --
    //--====================================================--
    public virtual void ReductHP(int reduct_num)
    {
        // 取得値が負の値の場合は回復


        if (HP > 0)
        {
            HP -= reduct_num;
            if (HP < 0)
                HP = 0;
            else if (HP >= Max_HP)
                HP = Max_HP;
        }
        // 死亡処理
        if (HP <= 0 && is_dead == false)
        {
            is_dead = true;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            coli.enabled = false;
            // 親オブジェクトから外れる（敵キャラ数の判定の調整用）
            transform.parent = game_controller.Active_Effects_Parent().transform;
            // 判定消失 (透過レイヤーへ移動)
            gameObject.layer = 1;

            // 「倒されたキャラクターの色」オプションの適用
            if (OptionData.current_options.dead_chara_color == 1)
                GetComponent<SpriteRenderer>().color = EigenValue.TRANSPARENT_COLOR;

            // その他の死亡時処理
            Moment_of_Dead();

        }
        // HPが減少する場合はミニHPバーの処理を実行
        if (reduct_num > 0)
        {
            if (Mini_hp_bar != null)
            {
                Mini_hp_bar.gameObject.SetActive(true);
                Mini_hp_bar.GaugeReduct(reduct_num);
            }
        }
    }

    //--====================================================--
    //--                MPを増減させる処理                  --
    //--====================================================--
    public virtual void ReductMP(int reduct_num)
    {
        // 取得値が負の値の場合は回復

        MP -= reduct_num;
        if (MP < 0)
            MP = 0;
        else if (MP >= Max_MP)
            MP = Max_MP;
    }

    //--====================================================--
    //--             ノックバックを受ける処理               --
    //--====================================================--
    // 角度は44~46くらいが望ましい？ ＜fly = true → ノックバック＞＜fly = false → のけぞり＞
    void Knockback(float knockback_weight, float vector, bool fly = true)
    {

        if (cant_knockback) // 吹っ飛ばない能力があると吹っ飛びは最小限になる
        {
            fly = false;
            knockback_weight = cant_knockback_weight;
        }

        //飛行キャラはノックバックしない
        if (!is_fly_chara)
        {
            this.gameObject.layer = 10; // ノックバック中レイヤーへ移動
            Touch_damage = 0;
            
            if (fly)
                is_knockback = true;
            is_jump = true;
        }

        rb2d.velocity = new Vector2(transform.localScale.x * knockback_weight * Mathf.Cos(vector), knockback_weight * Mathf.Sin(vector));
        //knockback_horizontal_power = rb2d.velocity.x;
        
    }

    //--====================================================--
    //--        ノックバック中の水平方向の激突処理          --
    //--====================================================--
    void Crush_Horizontal()
    {
        // 激突エフェクトを出す
        Play_Effect(crush_effect_horizontal, Vector2.zero, transform.localScale.x < 0);
        // 威力を減少させつつ吹っ飛び速度を反転
        rb2d.velocity = new Vector2(rb2d.velocity.x * -0.4f * Random.value, rb2d.velocity.y);
 
        is_knockback_crush = true;
    }

    //--====================================================--
    //--                    倒れる処理                      --
    //--====================================================--
    void Tumble()
    {
        Jump(1.0f);
        is_tumble = true;
        Play_Effect(crush_effect_vertical, Vector2.zero);
    }

    //--====================================================--
    //--ノックバックの終了処理(起床時にAnimationから呼び出し--
    //--====================================================--
    protected virtual void End_of_Tumble()
    {
        if (!is_dead)
        {
            is_damage = false;
            is_knockback = false;
            is_knockback_crush = false;
            is_tumble = false;
            // 点滅処理
            Blink(wakeup_time);
        }
    }

    //--====================================================--
    //--ダメージアニメ終了処理 (Animationから呼び出し)      --
    //--====================================================--
    void End_of_damage_motion()
    {
        is_damage = false;
    }

    //--====================================================--
    //--             エフェクトを発生させる処理             --
    //--====================================================--
    public GameObject Play_Effect(string prefab_name, Vector2 position_gap, bool mirror = false)
    {
        Vector3 vec;
        vec.x = position_gap.x;
        vec.y = position_gap.y;
        vec.z = 0f;

        GameObject effect = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_EFFECTS + prefab_name)), transform.localPosition + vec, transform.rotation, game_controller.Active_Effects_Parent().transform);
        effect.transform.localScale = new Vector3(mirror ? 1f : -1f, effect.transform.localScale.y, effect.transform.localScale.z);

        // 子の大きさを調整
        for (int i = 0; i < effect.transform.childCount; i++)
        {
            Transform child_transform = effect.transform.GetChild(i);
            // エフェクト軽量化オプションの適用
            if (OptionData.current_options.omitted_effect)
            {
                if (child_transform.name.Equals("detail_particle"))
                    Destroy(child_transform.gameObject);
            }
            else
                child_transform.localScale = effect.transform.localScale;
        }
        return effect;

    }

    //$$====================================================$$
    //$$         固有の行動処理(Updateに追加したい処理)     $$
    //$$====================================================$$
    protected abstract void OriginalAction();
    
    //$$====================================================$$
    //$$             死亡処理(Animationから呼出)            $$
    //$$====================================================$$
    public abstract void Dead();

    //--====================================================--
    //--             死亡した瞬間の処理(必要に応じて書換)   --
    //--====================================================--
    public virtual void Moment_of_Dead() { }
    
    //--====================================================--
    //--              特定の方を向く（だけ）                --
    //--====================================================--

    // ターゲットのいる方向へ向く
    protected void Focus(GameObject target)
    {
        var scale = Vector3.one;
        scale.x = target.transform.position.x > transform.position.x ? -1f : 1f;
        transform.localScale = scale;

    }

    // 特定の方向へ向く
    protected void Focus(float direction)
    {
        if (direction == 0f) return;

        var scale = Vector3.one;
        scale.x = direction > 0 ? -1f : 1f;
        transform.localScale = scale;
    }

    //--====================================================--
    //--                ２座標間の距離を導出                --
    //--====================================================--
    protected float Distance(float pos1, float pos2)
    {
        return Mathf.Abs(pos2 - pos1);
    }

    //--====================================================--
    //--                     点滅処理                       --
    //--====================================================--
    // 開始処理
    protected void Blink(float time)
    {
        if (time > 0f)
        {
            is_blink = true;
            this.time = time;
        }
        else
        {
            this.gameObject.layer = my_layer; // 元のレイヤーに戻して接触判定を復活
            Touch_damage = my_status.Touch_damage;
        }
    }

    // 点滅中処理
    protected void Blink()
    {
        Color color = spre.color;
        color.a = Mathf.Sin(Time.time * 100) / 2 + 0.5f;
        spre.color = color;

        time -= Time.deltaTime;
        if(time < 0f) 
        {
            Blink_end();
        }
    }

    // 終了処理
    protected virtual void Blink_end()
    {
        is_blink = false;
        spre.color = new Color(255, 255, 255, 255);
        this.gameObject.layer = my_layer; // 元のレイヤーに戻して接触判定を復活
        Touch_damage = my_status.Touch_damage;
    }
}