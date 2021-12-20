using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--�L�����N�^�[�ɂ��Ă��鏬HP�Q�[�W�̑��������N���X--
//--====================================================--
public class MiniGauge_Controll : MonoBehaviour
{
    // �A�N�e�B�u�ȃQ�[�W
    public GameObject Active_gauge { get; private set; }
    // �_���[�W���̃Q�[�W
    public GameObject Damage_gauge { get; private set; }
    //��Q�[�W
    public GameObject Empty_gauge { get; private set; }
    //�Q�[�W�������Ă���L�����N�^�[��Character�R���|�[�l���g
    Character parent_component;

    float timer = 0f;

    //##====================================================##
    //##       �����������i�L�����N�^�[������Ăяo���j     ##
    //##====================================================##
    public void SetParent()
    {
        parent_component = transform.parent.GetComponent<Character>();
        Active_gauge = transform.Find("bar_hp_mini").gameObject;
        Damage_gauge = transform.Find("bar_hp_damage_mini").gameObject;
        Empty_gauge = transform.Find("bar_hp_empty_mini").gameObject;
    }

    //##====================================================##
    //##                Update    �\���I������              ##
    //##====================================================##
    private void Update()
    {
        if(timer >= 0f) 
        {
            timer -= Time.deltaTime;

            if (timer < 0f)
                Hide();
        }
    }

    //##====================================================##
    //##              LateUpdate      �����̌Œ�            ##
    //##====================================================##
    private void LateUpdate()
    {
        // �������Œ肷��
        transform.localScale = transform.parent.localScale;
    }

    //##====================================================##
    //##                   �Q�[�W���B������                 ##
    //##====================================================##
    void Hide()
    {
        if (parent_component.HP > 0)
            this.gameObject.SetActive(false);
    }

    //##====================================================##
    //##                �Q�[�W�����������鏈��              ##
    //##====================================================##
    public void GaugeReduct(int reduction_value ,float time = 2f)
    {
        var value_from = (float)(parent_component.HP + reduction_value) / (float)parent_component.Max_HP;
        var value_to = (float)(parent_component.HP) / (float)parent_component.Max_HP;

        //�A�N�e�B�u�Q�[�W����
        Active_gauge.transform.localScale = new Vector3(value_to,Active_gauge.transform.localScale.y,Active_gauge.transform.localScale.z);

        //�ԃQ�[�W����
        Damage_gauge.transform.DOScaleX(value_to, time).SetEase(Ease.OutCirc);
        // ���S���͋�Q�[�W�����X�ɏ����A�����I�������L�����N�^�[������
        if (parent_component.HP <= 0)
            Empty_gauge.transform.DOScaleX(0, time).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                if (parent_component.HP <= 0)
                {
                    parent_component.Dead();
                }
            });
        //4�b��ɃQ�[�W���B��
        if (parent_component.HP > 0)
        {
            timer = 4f;
        }
    }

}