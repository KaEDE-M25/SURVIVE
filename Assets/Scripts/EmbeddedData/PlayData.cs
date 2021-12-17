using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


//###########################################################################################################
//===========================================================================================================
//##                                   �A�C�e���X�g�b�N��P�̂�\���N���X                                  ##
//===========================================================================================================
//###########################################################################################################
[Serializable]
public struct ItemStock
{

    public int item_id;
    public int num;

    public ItemStock(int item_id = 0, int num = 0)
    {
        this.item_id = item_id;
        this.num = num;
    }

}

//###########################################################################################################
//===========================================================================================================
//##                                    �A�C�e���X�g�b�N��\���N���X                                       ##
//===========================================================================================================
//###########################################################################################################
[Serializable]
public class ItemStocks
{
    public ItemStock[] item_stocks;

    public ItemStocks()
    {
        item_stocks = new ItemStock[8];
    }


}

//###########################################################################################################
//===========================================================================================================
//##                    �Q�[���v���C���̃f�[�^���܂Ƃ߂�N���X�i�ۑ��A�ǂݍ��݂̎��Ɏg�p�j                 ##
//===========================================================================================================
//###########################################################################################################
[Serializable]
public class PlayData
{
    // ���Ƃ���ς����Ȃ�����

    // �I�������L�����N�^��ID
    public readonly int choose_chara_id;
    // �I�������X�e�[�W��ID
    public readonly int choose_stage_id;
    // �I���������[�h
    // 0 �m�[�}��
    // 1 �^�C�����X
    // 2 �G���h���X

    public readonly int choose_mode_id;

    // ���Ƃ���ς��邱�Ƃ��ł������

    // HP
    public int hp;

    // MP
    public int mp;
    
    // �X�R�A
    public int score;

    // ����
    public int money;

    // �L���J�E���g
    public int kill_count;

    // �E�F�[�u���B��
    public int clear_waves;

    // �A�C�e���X�g�b�N
    public ItemStocks item_stocks;


    public PlayData(
        int choose_chara_id,
        int choose_stage_id,
        int choose_mode_id,
        int hp,
        int mp,
        int score,
        int money,
        int kill_count,
        int clear_waves,
        ItemStocks item_stocks)
    {
        this.choose_chara_id = choose_chara_id;
        this.choose_stage_id = choose_stage_id;
        this.choose_mode_id = choose_mode_id;
        this.hp = hp;
        this.mp = mp;
        this.score = score;
        this.money = money;
        this.kill_count = kill_count;
        this.clear_waves = clear_waves;
        this.item_stocks = item_stocks;
    
    }

}
