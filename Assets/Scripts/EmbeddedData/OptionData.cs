using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


//###########################################################################################################
//===========================================================================================================
//##                                     オプションの項目を管理するデータ                                  ##
//===========================================================================================================
//###########################################################################################################
[Serializable]
public class OptionData
{

    // 現在のオプションデータ
    public static OptionData current_options = null;

    // ***** コントローラーとして何を利用するか *****
    //  "Keyboard" -> キーボード
    //  "GamePad" -> ゲームパッド(ジョイスティック)
    //  "ScreenPad" -> スクリーンキー（スマホでプレイする場合に画面に操作キーを表示させる）
    public string controller;

    public const string CONTROLLER_NAME_KEYBOARD = "KeyBoard";
    public const string CONTROLLER_NAME_GAMEPAD = "GamePad";
    public const string CONTROLLER_NAME_SCREENPAD = "ScreenPad";


    // ***** エフェクトを軽量化するかどうか *****
    // ※ Particleエフェクトの一部の利用を禁止することで軽量化を図る
    public bool omitted_effect;


    // ***** 倒されたキャラの死亡モーションの透過処理をどうするか *****
    // 0 -> 通常
    // 1 -> 半透明
    public int dead_chara_color;

    // ***** 振動を発生させる処理 *****
    public bool is_play_shake;

    public OptionData(string controller = "Keyboard", bool omitted_effect = false,int dead_chara_color = 0,bool is_play_shake = true) 
    {
        this.controller = controller;
        this.omitted_effect = omitted_effect;
        this.dead_chara_color = dead_chara_color;
        this.is_play_shake = is_play_shake;
    }

}
