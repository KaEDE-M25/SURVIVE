using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                   �L�����N�^�[                     --
//--====================================================--
public abstract class Character : MonoBehaviour
{
    // �Œ�l
    protected const float BASE_JUMP_FORCE = 1000000f;



    // ���g�����R���|�[�l���g (���������ɐݒ�)
    protected Rigidbody2D rb2d;
    public Animator Anim { get; protected set; }
    protected Collider2D coli;
    protected SpriteRenderer spre;

    // GameControll�R���|�[�l���g
    protected GameControll game_controller;

    // �_�ŏ����p�^�C�}�[
    float time = 0f;

    // �����C���[(�m�b�N�o�b�N�p���C���[����߂�̂ɗ��p)
    protected int my_layer;

    // �ėp�G�t�F�N�g�̖��O
    string crush_effect_horizontal;
    string crush_effect_vertical;

    // �L�����N�^�[�ɕt�����Ă���~�jHP�o�[
    public MiniGauge_Controll Mini_hp_bar { get; protected set; }

    // �Z�Z���Ă��邩�̔���ϐ�
    #region if_xxx

    // �W�����v�����ǂ���
    [SerializeField]
    protected bool is_jump;
    // �_���[�W���󂯂Ă��邩�ǂ���
    protected bool is_damage;
    // �m�b�N�o�b�N�����ǂ���
    protected bool is_knockback;
    // �m�b�N�o�b�N���ɕǂɂP�񌃓˂������ǂ���
    protected bool is_knockback_crush;
    // �|�ꂽ��Ԃ��ǂ���
    protected bool is_tumble;
    // ���񂾂��ǂ���
    protected bool is_dead;
    // �_�ŏ������N���Ă邩�ǂ���
    protected bool is_blink;

    #endregion

    // �L�����N�^�[���� (��b����ɒ��ڊ��������\�͂̒�`)(prefab��Őݒ�)
    #region characteristics
    // �U����H��������̖��G���� (�b)
    protected float wakeup_time = 0f;
    // ��s����L�������ǂ���
    protected bool is_fly_chara = false;
    // ������΂Ȃ�
    protected bool cant_knockback = false;
    // ������΂Ȃ��ꍇ�̂̂�����̑傫��
    protected float cant_knockback_weight = 100f;
    #endregion

    // �e��X�e�[�^�X (�ς���^��������̂͒ǉ��Œ�`)
    #region status

    // �X�e�[�^�X���܂Ƃ߂��N���X
    protected CharacterStatus my_status = null;

    // �̓�����З́i�]���r�Ƃ��p�j�i�m�b�N�o�b�N����0�ɂ���j
    public int Touch_damage { get; protected set; }
    // �ő�HP
    public int Max_HP { get; set; }
    // �ő�MP
    public int Max_MP { get; set; }
    // ����HP
    public int HP { get; set; }
    // ����MP
    public int MP { get; set; }

    #endregion




    //##====================================================##
    //##               Awake �X�e�[�^�X�̏�����             ##
    //##====================================================##
    private void Awake()
    {
        // �X�e�[�^�X������
        Status_Initialize(CharacterStatus());
    }


    //##====================================================##
    //##                    Update ��b����                 ##
    //##====================================================##
    protected void Update()
    {
        // �ђʂ����肵�Ė�������������y���W������������(�t���Đn)
        if (transform.position.y <= -10000f)
            transform.position *= Vector2.right;


        // �W�����v���Ȃ�
        if (is_jump)
        {
            // ��s�L�����ł͂Ȃ��Ȃ�
            if (!is_fly_chara)
                rb2d.gravityScale = 100;�@// �d�̓X�P�[����ݒ�
        }
        else
        {
            // �n��ł͏d�͂𒲐�
            rb2d.gravityScale = 0f;
            // velocity��x�̂ݔ��f
            rb2d.velocity *= Vector2.right;
        }
        if (!is_dead)
        {
            // �e�L�����ŗL�A�N�V����
            OriginalAction();

            // �_�ŏ������Ȃ�_��
            if (is_blink)
                Blink();
        }
        // Animator�ɒl��n��
        Set_AnimValue();

    }


    //##====================================================##
    //##             Particle�ɓ����������̏���             ##
    //##====================================================##
    private void OnParticleCollision(GameObject target)
    {
        Damage(target);
    }


    //##====================================================##
    //##            Collision�ɓ����������̏���             ##
    //##====================================================##
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!is_dead)
        {
            // �n�ʂɓ��������ۂ͌��ˏ���
            if (collision.collider.CompareTag("Ground"))
            {
                // �m�b�N�o�b�N���Ȃ�
                if (is_knockback)
                {
                    // ���Ɍ��˂����ꍇ�i�ǂɂP��͌��˂��Ă���j
                    if (is_knockback_crush)
                    {
                        // �|��鏈�������Ă��Ȃ��Ȃ�|��鏈�����n�߂�
                        if (!is_tumble)
                        {
                            if (rb2d.velocity.y == 0)
                            {
                                // ���ʉ����Đ�
                                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.CRUSH, spre);
                                Tumble();
                            }
                        }

                    }
                    else // �ǂɌ��˂����ꍇ�i�͂��߂Ă̌��ˁj
                    {
                        // ���ʉ����Đ�
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.CRUSH, spre);
                        // �n�ʂɓ������Ă��Ȃ��ƍl������(y���W�̈ړ����~�܂��Ă��Ȃ��ꍇ)�Ȃ�ǌ��ˏ���
                        if (rb2d.velocity.y != 0)
                            Crush_Horizontal();
                        else //�n�ʂɓ������Ă���ƍl������(y���W�̈ړ����~�܂��Ă���ꍇ)�Ȃ�|��鏈��
                        {
                            Tumble();
                            is_knockback_crush = true;
                        }
                    }
                }
                else if (rb2d.velocity.x != 0) // �ǂɓ������Ă��Ȃ��ƍl������(x���W�̈ړ����~�܂��Ă���)�Ȃ璅�n����
                {
                    is_jump = false;
                }

            }
            // ����ȊO�̏ꍇ�̓L�����N�^�[�ɓ��������ꍇ�Ȃ̂Ń_���[�W����
            else
            {
                Damage(collision);
            }
        }
    }


    //##====================================================##
    //##              Trigger�ɓ����������̏���             ##
    //##====================================================##
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // �G�������̓v���C���[�̃G�t�F�N�g�������ꍇ�̓_���[�W����
        if (collision.transform.CompareTag("AttackEffect_Player") || collision.transform.CompareTag("AttackEffect_Enemy"))
            Damage(collision);

    }


    //##====================================================##
    //##           Collision�ɓ������Ă��鎞�̏���          ##
    //##====================================================##
    protected void OnCollisionStay2D(Collision2D collision)
    {
        // �n�ʁE�ǂɓ������Ă���Ȃ�
        if (collision.collider.CompareTag("Ground"))
        {
            // �n�ʂɓ������Ă���ƍl������(y���W�̈ړ����~�܂��Ă���)�Ȃ�
            if (rb2d.velocity.y == 0)
            {
                is_jump = false;
                // �̂����蒆(�_���[�W�͎󂯂Ă��邪�m�b�N�o�b�N���Ă��Ȃ�)��������
                if (!is_knockback && is_damage)
                {
                    is_damage = false;
                    //is_knockback = false;
                    // �_�ŏ���
                    Blink(wakeup_time);
                }
                // �|��Ă��鏈�����Ȃ�
                if (is_tumble)
                {
                    // �޻ް
                    rb2d.velocity = new Vector2(rb2d.velocity.x / 1.2f, rb2d.velocity.y);
                }
                else
                {
                    // �P��͕ǂɌ��˂��Ă���Ȃ�
                    if (is_knockback_crush)
                    {
                        // ���ʉ����Đ�
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.CRUSH, spre);
                        // �|��鏈��
                        Tumble();
                    }
                }
            }
            // ����ȊO�̏ꍇ�͕ǂɌ��˂��Ă���ƍl������ꍇ�ɂȂ�
            else
            {
                /*if (rb2d.velocity.y == 0 && Anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_up"))
                {
                    is_jump = false;
                }*/
                // �m�b�N�o�b�N���ł��P��ڂ̕ǌ��˂̏ꍇ
                if (is_knockback)
                {
                    if (/*is_jump && */!is_knockback_crush && Mathf.Abs(rb2d.velocity.x) <= 0.01f)
                    {
                        // ���ʉ����Đ�
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
    //##             Collision���痣�ꂽ���̏���            ##
    //##====================================================##
    protected void OnCollisionExit2D(Collision2D collision)
    {
        // �n�ʂ��痣�ꂽ��W�����v�����ƍl������
        if (collision.collider.CompareTag("Ground"))
        {
            is_jump = true;
        }
    }


    //--====================================================--
    //--      �L�����N�^�[�̃X�e�[�^�X��Ԃ��������֐�      --
    //--           �i�h�����\�b�h�ŕK����`����j           --
    //--====================================================--
    protected abstract CharacterStatus CharacterStatus();


    //--====================================================--
    //--          �l�����������鏈�� (Awake�ŌĂ�)          --
    //--====================================================--
    protected virtual void Status_Initialize(CharacterStatus status)
    {
        // �L�����N�^�[�X�e�[�^�X
        my_status = status;
        this.Touch_damage = status.Touch_damage;
        this.Max_HP = status.Max_HP;
        this.Max_MP = status.Max_MP;
        this.HP = status.Max_HP;
        this.MP = status.Max_MP;

        // �R���|�[�l���g�̎擾
        rb2d = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        coli = GetComponent<Collider2D>();
        spre = GetComponent<SpriteRenderer>();
        //spre = GetComponent<Renderer>();
        game_controller = GameObject.FindWithTag("GameController").GetComponent<GameControll>();


        // �����l�̏�����
        is_jump = true;
        is_damage = false;
        is_knockback = false;
        is_knockback_crush = false;
        is_tumble = false;
        is_dead = false;

        // ���C���[�ԍ��̎擾
        my_layer = transform.gameObject.layer;

        // �ėp�G�t�F�N�g�̖��̎擾

        crush_effect_horizontal = "EF_Crush_horizontal";
        crush_effect_vertical = "EF_Crush_vertical";


        // �~�jHP�o�[��ݒ�
        var res = transform.Find("hp_bar_mini");
        Mini_hp_bar = res != null ? res.GetComponent<MiniGauge_Controll>() : null;
        if (Mini_hp_bar != null) Mini_hp_bar.SetParent();
    }


    //--====================================================--
    //--                �W�����v�����鏈��                  --
    //--====================================================--
    protected void Jump(float jump_power)
    {
        is_jump = true;
        rb2d.AddForce(Vector2.up * jump_power * BASE_JUMP_FORCE);
    }

    //--====================================================--
    //--              Animator�ɒl��n������                --
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
    //--                   �ړ������鏈��                   --
    //--====================================================--

    // ����̑���ւ̒����ړ�
    protected void Move(float speed, GameObject target)
    {
        // �_���[�W���󂯂Ă���r���łȂ��Ȃ�
        if (!is_damage)
        {
            Vector2 vec;

            vec.x = target.transform.position.x - transform.position.x;
            vec.y = target.transform.position.y - transform.position.y;
            vec = vec.normalized * speed;

            // ���W���ړ�
            rb2d.velocity = vec;

            // �Ώۂ֌���
            Focus(target);

            /*
            if (target.transform.position.x - transform.position.x > 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
            */
        }

    }

    // �������w�肵�����������ړ�
    protected void Move(float speed, float direction)
    {
        // �_���[�W���󂯂Ă���r���łȂ��Ȃ�
        if (!is_damage)
        {
            Vector2 vec;
            vec.x = speed * direction;
            vec.y = rb2d.velocity.y;

            // ���W���ړ�
            rb2d.velocity = vec;

            // �ړ�����������
            Focus(direction);

            /*
            //�@����������
            if (direction < 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction > 0)
                transform.localScale = new Vector3(-1, 1, 1);
            */
        }

    }

    //--====================================================--
    //--               �_���[�W���󂯂鏈��                 --
    //--====================================================--
    // collision�i�L�������m�̐ڐG�̏ꍇ�j
    protected void Damage(Collision2D collision)
    {
        if (!is_damage)
        {
            // �^�O���Ⴄ(Player���m Enemy���m�̏ꍇ�̓_���[�W����͂Ȃ��Ƃ���)
            if (!coli.CompareTag(collision.collider.tag))
            {
                if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
                //�L�����N�^�[���m�̐ڐG
                {
                    var enemy_coli = collision.gameObject.GetComponent<Character>();
                    // �_���[�W������Ȃ�
                    if (enemy_coli.Touch_damage > 0)
                    {
                        is_damage = true;
                        // HP�����炷����
                        ReductHP(enemy_coli.Touch_damage);

                        // �G�̕��������@(��������Ɍ��Ƀm�b�N�o�b�N)
                        Focus(collision.gameObject);
                        // �m�b�N�o�b�N�𔭐������鏈��
                        Knockback(150f, 45f, false);

                        // ���ʉ����Đ�
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.DAMAGE, spre);

                        // �_�ŏ���
                        Blink(wakeup_time);

                    }


                }
            }
        }
    }

    // collider (�U���G�t�F�N�g�̏ꍇ)
    protected void Damage(Collider2D collider)
    {
        if (!is_damage)
        {
            // �����Ɠ������͂ł͂Ȃ��U���G�t�F�N�g�������ꍇ
            if (!collider.CompareTag("AttackEffect_" + transform.tag))
            {
                AttackEffectStatus attack_st = collider.transform.GetComponent<AttackEffectStatus>();
                if (attack_st.damage > 0)
                {
                    is_damage = true;
                    ReductHP(attack_st.damage);
                }

                // ���ʉ����Đ�
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.DAMAGE, spre);

                if (!is_dead)
                {
                    // �G�̕��������@(��������Ɍ��Ƀm�b�N�o�b�N)
                    Focus(collider.gameObject);
                    Knockback(attack_st.knockback_weight + Random.Range(-50f, 50f), attack_st.knockback_angle + Random.Range(-0.3f, 0.3f));

                    bool mirror = false;
                    if (transform.localScale.x > 0) mirror = true;

                    Play_Effect(attack_st.effect.name, Vector2.zero, mirror);
                }

            }
        }
    }

    // particle system (�p�[�e�B�N���̏ꍇ�i�v���C���[��MP�U���Ȃǁj)
    protected void Damage(GameObject particle)
    {
        if (!is_damage)
        {
            // �����Ɠ������͂ł͂Ȃ��U���G�t�F�N�g�������ꍇ
            if (!particle.CompareTag("AttackEffect_" + transform.tag))
            {
                AttackEffectStatus attack_st = particle.GetComponent<AttackEffectStatus>();
                if (attack_st.damage > 0)
                {
                    is_damage = true;
                    ReductHP(attack_st.damage);
                }

                // ���ʉ����Đ�
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_CRUSHandDAMAGE, AudioFilePositions.EFFECT.DAMAGE, spre);

                if (!is_dead)
                {
                    // �G�̕��������@(��������Ɍ��Ƀm�b�N�o�b�N)
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
    //--                HP�𑝌������鏈��                  --
    //--====================================================--
    public virtual void ReductHP(int reduct_num)
    {
        // �擾�l�����̒l�̏ꍇ�͉�


        if (HP > 0)
        {
            HP -= reduct_num;
            if (HP < 0)
                HP = 0;
            else if (HP >= Max_HP)
                HP = Max_HP;
        }
        // ���S����
        if (HP <= 0 && is_dead == false)
        {
            is_dead = true;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            coli.enabled = false;
            // �e�I�u�W�F�N�g����O���i�G�L�������̔���̒����p�j
            transform.parent = game_controller.Active_Effects_Parent().transform;
            // ������� (���߃��C���[�ֈړ�)
            gameObject.layer = 1;

            // �u�|���ꂽ�L�����N�^�[�̐F�v�I�v�V�����̓K�p
            if (OptionData.current_options.dead_chara_color == 1)
                GetComponent<SpriteRenderer>().color = EigenValue.TRANSPARENT_COLOR;

            // ���̑��̎��S������
            Moment_of_Dead();

        }
        // HP����������ꍇ�̓~�jHP�o�[�̏��������s
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
    //--                MP�𑝌������鏈��                  --
    //--====================================================--
    public virtual void ReductMP(int reduct_num)
    {
        // �擾�l�����̒l�̏ꍇ�͉�

        MP -= reduct_num;
        if (MP < 0)
            MP = 0;
        else if (MP >= Max_MP)
            MP = Max_MP;
    }

    //--====================================================--
    //--             �m�b�N�o�b�N���󂯂鏈��               --
    //--====================================================--
    // �p�x��44~46���炢���]�܂����H ��fly = true �� �m�b�N�o�b�N����fly = false �� �̂����聄
    void Knockback(float knockback_weight, float vector, bool fly = true)
    {

        if (cant_knockback) // ������΂Ȃ��\�͂�����Ɛ�����т͍ŏ����ɂȂ�
        {
            fly = false;
            knockback_weight = cant_knockback_weight;
        }

        //��s�L�����̓m�b�N�o�b�N���Ȃ�
        if (!is_fly_chara)
        {
            this.gameObject.layer = 10; // �m�b�N�o�b�N�����C���[�ֈړ�
            Touch_damage = 0;
            
            if (fly)
                is_knockback = true;
            is_jump = true;
        }

        rb2d.velocity = new Vector2(transform.localScale.x * knockback_weight * Mathf.Cos(vector), knockback_weight * Mathf.Sin(vector));
        //knockback_horizontal_power = rb2d.velocity.x;
        
    }

    //--====================================================--
    //--        �m�b�N�o�b�N���̐��������̌��ˏ���          --
    //--====================================================--
    void Crush_Horizontal()
    {
        // ���˃G�t�F�N�g���o��
        Play_Effect(crush_effect_horizontal, Vector2.zero, transform.localScale.x < 0);
        // �З͂���������������ё��x�𔽓]
        rb2d.velocity = new Vector2(rb2d.velocity.x * -0.4f * Random.value, rb2d.velocity.y);
 
        is_knockback_crush = true;
    }

    //--====================================================--
    //--                    �|��鏈��                      --
    //--====================================================--
    void Tumble()
    {
        Jump(1.0f);
        is_tumble = true;
        Play_Effect(crush_effect_vertical, Vector2.zero);
    }

    //--====================================================--
    //--�m�b�N�o�b�N�̏I������(�N������Animation����Ăяo��--
    //--====================================================--
    protected virtual void End_of_Tumble()
    {
        if (!is_dead)
        {
            is_damage = false;
            is_knockback = false;
            is_knockback_crush = false;
            is_tumble = false;
            // �_�ŏ���
            Blink(wakeup_time);
        }
    }

    //--====================================================--
    //--�_���[�W�A�j���I������ (Animation����Ăяo��)      --
    //--====================================================--
    void End_of_damage_motion()
    {
        is_damage = false;
    }

    //--====================================================--
    //--             �G�t�F�N�g�𔭐������鏈��             --
    //--====================================================--
    public GameObject Play_Effect(string prefab_name, Vector2 position_gap, bool mirror = false)
    {
        Vector3 vec;
        vec.x = position_gap.x;
        vec.y = position_gap.y;
        vec.z = 0f;

        GameObject effect = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_EFFECTS + prefab_name)), transform.localPosition + vec, transform.rotation, game_controller.Active_Effects_Parent().transform);
        effect.transform.localScale = new Vector3(mirror ? 1f : -1f, effect.transform.localScale.y, effect.transform.localScale.z);

        // �q�̑傫���𒲐�
        for (int i = 0; i < effect.transform.childCount; i++)
        {
            Transform child_transform = effect.transform.GetChild(i);
            // �G�t�F�N�g�y�ʉ��I�v�V�����̓K�p
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
    //$$         �ŗL�̍s������(Update�ɒǉ�����������)     $$
    //$$====================================================$$
    protected abstract void OriginalAction();
    
    //$$====================================================$$
    //$$             ���S����(Animation����ďo)            $$
    //$$====================================================$$
    public abstract void Dead();

    //--====================================================--
    //--             ���S�����u�Ԃ̏���(�K�v�ɉ����ď���)   --
    //--====================================================--
    public virtual void Moment_of_Dead() { }
    
    //--====================================================--
    //--              ����̕��������i�����j                --
    //--====================================================--

    // �^�[�Q�b�g�̂�������֌���
    protected void Focus(GameObject target)
    {
        var scale = Vector3.one;
        scale.x = target.transform.position.x > transform.position.x ? -1f : 1f;
        transform.localScale = scale;

    }

    // ����̕����֌���
    protected void Focus(float direction)
    {
        if (direction == 0f) return;

        var scale = Vector3.one;
        scale.x = direction > 0 ? -1f : 1f;
        transform.localScale = scale;
    }

    //--====================================================--
    //--                �Q���W�Ԃ̋����𓱏o                --
    //--====================================================--
    protected float Distance(float pos1, float pos2)
    {
        return Mathf.Abs(pos2 - pos1);
    }

    //--====================================================--
    //--                     �_�ŏ���                       --
    //--====================================================--
    // �J�n����
    protected void Blink(float time)
    {
        if (time > 0f)
        {
            is_blink = true;
            this.time = time;
        }
        else
        {
            this.gameObject.layer = my_layer; // ���̃��C���[�ɖ߂��ĐڐG����𕜊�
            Touch_damage = my_status.Touch_damage;
        }
    }

    // �_�Œ�����
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

    // �I������
    protected virtual void Blink_end()
    {
        is_blink = false;
        spre.color = new Color(255, 255, 255, 255);
        this.gameObject.layer = my_layer; // ���̃��C���[�ɖ߂��ĐڐG����𕜊�
        Touch_damage = my_status.Touch_damage;
    }
}