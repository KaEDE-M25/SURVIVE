using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//##====================================================##
//##                 �G�L�����@�X�P���g��               ##
//##====================================================##
public class Skeleton : Enemy
{
    // ���U���̊Ԋu�i�t���[���P�ʁj
    const int THROWBONE_INTERVAL = 300;

    // ���G�t�F�N�g�̃����_��
    Renderer bone_renderer;
    // �v���C���[�Ƃ̋���
    float dist_for_player;
    // ���U���̃C���^�[�o�����v�Z���邽�߂̃J�E���^
    int frame_count = 300;
    [SerializeField, Tooltip("���U���G�t�F�N�g��obj")]
    GameObject bone;
    [SerializeField, Tooltip("���S���[�V�����̐eobj")]
    GameObject dead_motion_parent;

    protected override CharacterStatus CharacterStatus()
    {
        // �X�e�[�^�X�������l
        return EigenValue.STATUS_001_SKELETON.status;
    }

    //##====================================================##
    //##            Start�ōs���X�e�[�^�X������             ##
    //##====================================================##
    protected override void Initialize() 
    {

        //bone = transform.Find("EF_skeleton_bone").gameObject;
        bone_renderer = bone.GetComponent<Renderer>();
        //dead_motion_parent = transform.Find("EF_Skeleton_dead").gameObject;

        bone.transform.parent = GameObject.FindWithTag("GameController").GetComponent<GameControll>().Active_Effects_Parent().transform;

    }

    //##====================================================##
    //##        �A�C�e���h���b�v����                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
    }

    //##====================================================##
    //##                   �ŗL�̍s������                   ##
    //##====================================================##
    protected override void OriginalAction()
    {

        // ���𓊂��Ă�r���Ȃ�
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
                // �������~
                if (!is_jump)
                    rb2d.velocity = Vector2.zero;
                // �v���C���[�̕�������
                Focus(player);

                // �J�E���g
                frame_count++;

                // ���Ԋu���Ƃɍ��U��
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
                �� player.transform.position.x > transform.position.x ^ dist_for_player >= 130f ? -1f : 1f
                �́A�ȉ����ȗ����������́B

                // �v���C���[���E�A�G�����ɂ���Ȃ�
                if (player.transform.position.x > transform.position.x)
                {
                    // ����������߂Â�

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
    //##                �X�P���g���Ŏg������                ##
    //##====================================================##
    // ���U�� (Anim���J�n����)
    void BoneAttack() 
    {
        Anim.Play("throwbone");
    }
    // ���̍U���G�t�F�N�g���΂��iAnimation����Ăяo���j
    void Throw_bone()
    {
        bone.transform.position = transform.position;
        bone.SetActive(true);
        bone.GetComponent<Rigidbody2D>().velocity = new Vector2((-150f + Random.Range(-50f,50f)) * transform.localScale.x,400f);
    }

    // ���S���[�V����(�̂��΂�΂�ɂȂ�G�t�F�N�g)�𔭐�
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
    //##             ���S����(Animation����ďo)            ##
    //##====================================================##
    public override void Dead()
    {
        // �U���G�t�F�N�g�̐e�����̃L�����N�^�[�ɖ߂��iDestroy�ł܂Ƃ߂ď������߁j
        bone.SetActive(false);
        bone.transform.parent = this.transform;

        base.Dead();
    }

}
