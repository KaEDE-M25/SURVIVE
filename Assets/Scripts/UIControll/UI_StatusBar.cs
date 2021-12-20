using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--        UI��̃X�e�[�^�X�o�[�𑀍삷��N���X        --
//--====================================================--
public class UI_StatusBar : MonoBehaviour
{
    // �^�[�Q�b�g�ƂȂ�X�e�[�^�X
    enum TargetStatus
    {
        HP,
        MP
    }

    [SerializeField,Tooltip("���̃X�e�[�^�X�o�[�����삷��X�e�[�^�X")]
    TargetStatus targetstatus;

    [SerializeField,Tooltip("�A�N�e�B�u�Q�[�W")]
    Image active_gauge;
    [SerializeField,Tooltip("�_���[�W��\�������ԃQ�[�W")]
    Image damage_gauge;
    [SerializeField,Tooltip("�o�[�̒����ɕ\�������e�L�X�g(�X�e�[�^�X�̌��ݒl�ƍő�l�𐔒l�Ŏ���)")]
    Text center_text;

    // �v���C���[��Fighter�R���|�[�l���g
    Fighters player_fighter_component;
    Tween damage_gauge_tween;

    //##====================================================##
    //##                  Awake   ����������                ##
    //##====================================================##
    private void Awake()
    {
        player_fighter_component = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighters>();
        center_text.text = TargetStatus_current() + "/" + TargetStatus_max();
    }

    //##====================================================##
    //##              �X�e�[�^�X�̌��ݒl���擾              ##
    //##====================================================##
    int TargetStatus_current() 
    {
        switch (targetstatus) 
        {
            case TargetStatus.HP: {return player_fighter_component.HP; }
            case TargetStatus.MP: {return player_fighter_component.MP; }
            default: { throw new System.Exception("Invalid Status.");}
        }
    }

    //##====================================================##
    //##              �X�e�[�^�X�̍ő�l���擾              ##
    //##====================================================##
    int TargetStatus_max()
    {
        switch (targetstatus)
        {
            case TargetStatus.HP: { return player_fighter_component.Max_HP; }
            case TargetStatus.MP: { return player_fighter_component.Max_MP; }
            default: { throw new System.Exception("Invalid Status."); }
        }
    }

    //##====================================================##
    //##                �Q�[�W���蓮�X�V����                ##
    //##====================================================##
    public void RenewGaugeAmount()
    {
        center_text.text = TargetStatus_current() + "/" + TargetStatus_max();
        var value = (float)TargetStatus_current() / TargetStatus_max();
        active_gauge.fillAmount = value;
        damage_gauge.fillAmount = value;
    }

    //##====================================================##
    //##               �Q�[�W�����������鏈��               ##
    //##====================================================##
    public void GaugeReduct(int reduction_value, float time = 1f) 
    {
        var value_from = (float)(TargetStatus_current() + reduction_value) / TargetStatus_max();
        var value_to = (float)TargetStatus_current() / TargetStatus_max();

        center_text.text = TargetStatus_current() + "/" + TargetStatus_max();

        //�A�N�e�B�u�Q�[�W����
        active_gauge.fillAmount = value_to;

        // �ԃQ�[�W����������������~�߂�
        if (damage_gauge_tween != null)
            damage_gauge_tween.Kill();

        //�ԃQ�[�W����
        damage_gauge_tween = DOTween.To(
            () => value_from,
            x =>
            {
                damage_gauge.fillAmount = x;
            },
            value_to,
            time
            ).SetEase(Ease.OutCirc);
    }
}
