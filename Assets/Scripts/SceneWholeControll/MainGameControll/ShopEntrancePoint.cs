using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--              ���X������Ǘ�����N���X              --
//--   �X�̓�����ƂȂ�n�_�ɒu����obj�ɃA�^�b�`����    --
//--         �����X����̊Ǘ���GameGontroll���s��       --
//--====================================================--
public class ShopEntrancePoint : MonoBehaviour
{ 
    // �~�Z�ɓ���邩�ǂ����̔���l
    public bool Can_enter_shop { get; private set; } = false;

    //##====================================================##
    //##      collision�ɓ���������X�\�A�o����߂�       ##
    //##====================================================##
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Can_enter_shop = true;
            transform.Find(OptionData.current_options.controller).gameObject.SetActive(true);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Can_enter_shop = false;
            transform.Find(OptionData.current_options.controller).gameObject.SetActive(false);
        }

    }
}
