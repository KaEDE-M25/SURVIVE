using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 �G�L�����@�S�[����                 --
//--====================================================--
public class Golem : Enemy
{
    // �v���C���[�Ƃ̋���
    float dist_for_player;

    [SerializeField, Tooltip("���S���[�V�����̐eobj")]
    GameObject dead_motion_parent;

    protected override CharacterStatus CharacterStatus()
    {
        // �m�b�N�o�b�N���Ȃ�
        cant_knockback = true;
        cant_knockback_weight = 10f;

        return EigenValue.STATUS_005_GOLEM.status;
    }

    //##====================================================##
    //##            Start�ōs���X�e�[�^�X������             ##
    //##====================================================##
    protected override void Initialize() { }

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

        // ��苗���Ŏ~�܂�
        if (Distance(transform.position.x, player.transform.position.x) < 50f)
        {
            rb2d.velocity *= Vector2.up;
        }
        else // �����O�Ȃ�߂Â�
        {
            Move(my_status.Speed, player.transform.position.x > transform.position.x ? 1f : -1f);
            /*
            if (player.transform.position.x > transform.position.x)
            {
                // ����������߂Â�
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
    //##      ���S���[�V������������(Animation����ďo)     ##
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
    //##        �S�[�����̓���\�́i�Ռ��g���~�߂�j        ##
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
