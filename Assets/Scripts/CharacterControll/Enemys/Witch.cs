using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 �G�L�����@�E�B�b�`                   --
//--====================================================--
public class Witch: Enemy
{
    static readonly Vector3 INITIAL_SHOT_POS = new Vector3(-13f, -1f, 0f);

    [SerializeField, Tooltip("�V���b�g��rigidbody2D�B���ˏ����Ŏg�p")]
    Rigidbody2D shot_rb2d;
    [SerializeField, Tooltip("���˃G�t�F�N�g��obj")]
    GameObject fire_effect;

    // �V���b�g�G�t�F�N�g�̃����_��
    Renderer shot_renderer;
    // �v���C���[�Ƃ̋���
    float dist_for_player;
    // ���@�U���̃C���^�[�o�����v�Z���邽�߂̃J�E���^
    int frame_count = 200;
    // �ړ������ǂ���
    bool is_move = false;
    // �V���b�g�������Ă���r�����ǂ���
    bool is_shot = false;




    protected override CharacterStatus CharacterStatus()
    {
        // �X�e�[�^�X�������l
        return EigenValue.STATUS_004_WITCH.status;
    }

    //$$====================================================$$
    //$$            Start�ōs���X�e�[�^�X������             $$
    //$$====================================================$$
    protected override void Initialize() 
    {

        // �����_���[���擾
        shot_renderer = shot_rb2d.GetComponent<Renderer>();
        shot_rb2d.transform.parent = GameObject.FindWithTag("GameController").GetComponent<GameControll>().Active_Effects_Parent().transform;

        // �G�t�F�N�g�y�ʉ��I�v�V������ݒ肵�Ă�����
        if (OptionData.current_options.omitted_effect)
        {
            Destroy(shot_rb2d.transform.Find("detail_particle").gameObject);
        }

    }


    //##====================================================##
    //##                   �ŗL�̍s������                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        // �E�B�b�`���L�̍s��
        
        // �V���b�g���A�N�e�B�u�Ȃ�A��ʊO�ɔ�񂾂����
        if (shot_rb2d.gameObject.activeSelf)
        {
            if (!shot_renderer.isVisible)
            {
                shot_rb2d.gameObject.SetActive(false);
            }

        }

        // �V���b�g��łƂ��Ƃ��Ă���Ƃ��Ƀ_���[�W���󂯂��璆�f
        if(is_shot) 
        {
            if (is_damage || is_knockback)
            {
                shot_rb2d.gameObject.SetActive(false);
                is_shot = false;
            }
        }
        else
        {
            // ��苗����ۂi�X�P���g���Q�Ɓj
            if (!is_damage && player != null)
            {
                dist_for_player = Distance(transform.position.x, player.transform.position.x);
                if (dist_for_player > 80f && dist_for_player < 100f)
                {
                    rb2d.velocity *= Vector2.up;
                    is_move = false;
                    // �������~
                    if (!is_jump)
                    {
                        // �v���C���[�̕�������
                        Focus(player);

                        // �J�E���g
                        frame_count++;

                        // ���Ԋu���Ƃɖ��@�U��
                        if (frame_count > 100 && !shot_rb2d.gameObject.activeSelf)
                        {
                            is_shot = true;
                            MagicAttack();
                            frame_count = 0;
                        }
                    }
                }
                else // ��苗����ۂ�
                {

                    if (rb2d.velocity.x == 0f && !is_jump && is_move)
                        Jump(my_status.Jump_power);

                    Move(my_status.Speed, player.transform.position.x > transform.position.x ^ dist_for_player >= 100f ? -1f : 1f);


                    /*
                    �� player.transform.position.x > transform.position.x ^ dist_for_player >= 100f ? -1f : 1f
                    �́A�ȉ����ȗ����������́B

                    // �v���C���[���E�A�G�����ɂ���Ȃ�
                    if (player.transform.position.x > transform.position.x)
                    {
                        // ����������߂Â�
                        if (dist_for_player >= 100f)
                            Move(my_status.Speed, 1f);
                        else
                            Move(my_status.Speed, -1f);
                    }
                    else if (player.transform.position.x < transform.position.x)
                    {
                        if (dist_for_player >= 100f)
                            Move(my_status.Speed, -1f);
                        else
                            Move(my_status.Speed, 1f);
                    }
                    */
                    is_move = true;
                }
            }
        }
    }



    //##====================================================##
    //##                 �E�B�b�`�Ŏg������                 ##
    //##====================================================##
    // ���@�U��������(Anim���J�n����)
    void MagicAttack()
    {
        Anim.Play("magic");
    }

    // ���@�e�����(�悤�Ɍ��������Ďq�̃V���b�gobj���A�N�e�B�u�ɂ���)
    void CreateShot()
    {
        if (!is_damage && !is_knockback)
        {
            shot_rb2d.transform.position = transform.position + INITIAL_SHOT_POS * transform.localScale.x;
            shot_rb2d.gameObject.SetActive(true);
        }
    }

    // �V���b�g�����
    void FireShot()
    {
        if (!is_damage && !is_knockback) 
        {
            fire_effect.SetActive(true);
            shot_rb2d.velocity = new Vector2(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y).normalized * -200f;            
        }
    }

    // �V���b�g������I���ianimation����ďo�j
    void End_of_shot_anim()
    {
        is_shot = false;
    }

    //##====================================================##
    //##             ���S����(Animation����ďo)            ##
    //##====================================================##
    public override void Dead()
    {
        // �U���G�t�F�N�g�̐e�����̃L�����N�^�[�ɖ߂��iDestroy�ł܂Ƃ߂ď������߁j
        shot_rb2d.gameObject.SetActive(false);
        shot_rb2d.transform.parent = this.transform;

        base.Dead();
    }


    //##====================================================##
    //##                 ���S�����u�Ԃ̏���                 ##
    //##====================================================##
    public override void Moment_of_Dead()
    {
        shot_rb2d.gameObject.SetActive(false);
        is_shot = false;
    }


    //##====================================================##
    //##        �A�C�e���h���b�v����                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
        // �h���b�v���邨��������
        int drop_money = Random.Range(60, 80);
        // �h���b�v���邨��(�e��̐�)������
        int drop_gold = drop_money / 10;
        int drop_silver = drop_money % 10 / 5;
        int drop_bronze = drop_money % 10 % 5;

        // �����̃h���b�v
        for (int i = 0; i < drop_bronze; i++)
            Drop("Money_bronze", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_silver; i++)
            Drop("Money_silver", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_gold; i++)
            Drop("Money_gold", Random.Range(1.125f, 2f), Random.Range(200f, 300f));

        // �m����MP�|�[�V�������h���b�v
        if (Random.Range(0, 10) == 0)
        {
            Drop("MP_Potion", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        }
    }



}
