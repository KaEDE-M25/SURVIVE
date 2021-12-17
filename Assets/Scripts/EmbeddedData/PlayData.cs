using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


//###########################################################################################################
//===========================================================================================================
//##                                   アイテムストックを単体を表すクラス                                  ##
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
//##                                    アイテムストックを表すクラス                                       ##
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
//##                    ゲームプレイ時のデータをまとめるクラス（保存、読み込みの時に使用）                 ##
//===========================================================================================================
//###########################################################################################################
[Serializable]
public class PlayData
{
    // あとから変えられないもの

    // 選択したキャラクタのID
    public readonly int choose_chara_id;
    // 選択したステージのID
    public readonly int choose_stage_id;
    // 選択したモード
    // 0 ノーマル
    // 1 タイムレス
    // 2 エンドレス

    public readonly int choose_mode_id;

    // あとから変えることができるもの

    // HP
    public int hp;

    // MP
    public int mp;
    
    // スコア
    public int score;

    // お金
    public int money;

    // キルカウント
    public int kill_count;

    // ウェーブ到達数
    public int clear_waves;

    // アイテムストック
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
