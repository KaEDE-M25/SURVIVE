using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

//--====================================================--
//--         シーンOptionChangeの管理を統括する         --
//--====================================================--
public class OptionChangeControll : MonoBehaviour
{
    public const int IMPLEMENTED_OPTION = 4;

    [SerializeField,Tooltip("カーソルのobj(Tweenを適用するリスト)")]
    GameObject[] cursors;

    [SerializeField,Tooltip("メニューの選択肢のobj")]
    GameObject[] menu_objs = new GameObject[5];

    [SerializeField,Tooltip("オプションの現在値を表示するテキストobj")]
    Text[] change_item_objs = new Text[4];

    [SerializeField,Tooltip("シーン遷移アニメーションのコンポーネント")]
    MoveSceneAnimation mcanim;

    // 各オプションの選択肢を表示するメッセージ
    string[][] chooseMessages;
    readonly Tween[] cursor_tweens = new Tween[3];
    int choose = 0;
    // 各選択肢における選択状態
    int[] sub_choose;

    //##====================================================##
    //##                     Awake 初期化                   ##
    //##====================================================##
    private void Awake()
    {
        // メッセージをここでつくる
        chooseMessages = new string[IMPLEMENTED_OPTION][];


        // WINDOWS環境ではコントローラーが一つ以上接続されていたら選択肢を２つにする
        #if UNITY_STANDALONE_WIN
        //Debug.Log("WINDOWSで起動しています");
        var connected_controllers = CountController(Input.GetJoystickNames());
        //Debug.Log(connected_controllers);
        if(connected_controllers <= 0)
            chooseMessages[0] = new string[1] { "KEYBOARD"};
        else
            chooseMessages[0] = new string[2] { "KEYBOARD", "GAMEPAD"};
        //ANDROIDもしくはIOS環境
        #elif UNITY_ANDROID || UNITY_IOS
        //Debug.Log("スマホで起動しています");
        chooseMessages[0] = new string[1] { "SCREENPAD"};
        #else
        //Debug.Log("その他のなにかで起動しています");
        chooseMessages[0] = new string[3] { "KEYBOARD", "GAMEPAD","SCREENPAD"};
        #endif

        chooseMessages[1] = new string[2] { "ＯＮ", "ＯＦＦ" };
        chooseMessages[2] = new string[2] { "ふつう", "半透明"};
        chooseMessages[3] = new string[2] { "ふつう", "省略" };

        sub_choose = new int[change_item_objs.Length];
    }

    //##====================================================##
    //##             コントローラの数を計測する             ##
    //##====================================================##
    int CountController(string[] controller_names) 
    {
        int count = 0;
        foreach (string controller_name in controller_names)
            if (!controller_name.Equals("")) count++;

        return count;
    }

    //##====================================================##
    //##                  OnEnable 初期化                   ##
    //##====================================================##
    void OnEnable()
    {
        cursor_tweens[0] = cursors[0].transform.DOLocalMoveX(cursors[0].transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        cursor_tweens[1] = cursors[1].transform.DOLocalMoveX(cursors[1].transform.localPosition.x - 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        cursor_tweens[2] = cursors[2].transform.DOLocalMoveX(cursors[2].transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        sub_choose[0] = OptionData.current_options.controller switch
        {
            OptionData.CONTROLLER_NAME_KEYBOARD => Array.IndexOf(chooseMessages[0], "KEYBOARD"),
            OptionData.CONTROLLER_NAME_GAMEPAD => Array.IndexOf(chooseMessages[0], "KEYBOARD"),
            OptionData.CONTROLLER_NAME_SCREENPAD => Array.IndexOf(chooseMessages[0], "KEYBOARD"),
            _ => throw new Exception("無効なオプションです"),
        };
        sub_choose[1] = OptionData.current_options.is_play_shake ? 0 : 1;
        sub_choose[2] = OptionData.current_options.dead_chara_color;
        sub_choose[3] = OptionData.current_options.omitted_effect ? 1 : 0;

        Renew_Board();
    }

    //##====================================================##
    //##                Update オプション選択               ##
    //##====================================================##
    void Update()
    {
        // シーン遷移アニメーション中でなければ操作できる
        if (mcanim.fading <= 0)
        {
            // カーソルを上に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = menu_objs.Length - 1;
                else
                    choose -= 1;
                Renew_Board();
            }
            // カーソルを下に移動
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= menu_objs.Length - 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board();
            }

            // 戻るを選択していなければ
            if (choose != menu_objs.Length - 1) 
            {
                if (InputControll.GetInputDown(InputControll.INPUT_ID_LEFTARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                    if (sub_choose[choose] <= 0)
                        sub_choose[choose] = chooseMessages[choose].Length - 1;
                    else
                        sub_choose[choose] -= 1;
                    Renew_Board();
                }
                // カーソルを下に移動
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_RIGHTARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                    if (sub_choose[choose] >= chooseMessages[choose].Length - 1)
                        sub_choose[choose] = 0;
                    else
                        sub_choose[choose] += 1;
                    Renew_Board();
                }
            }

            // メニューに戻るを選択したら戻る
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                if (choose == menu_objs.Length - 1)
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.DECISION);
                    SaveOption_and_Return_Menu();
                }
            }
            // Bボタンで戻る
            if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CANCEL);
                SaveOption_and_Return_Menu();
            }
        }
    }


    //##====================================================##
    //##          オプションを保存し、メニューに戻る        ##
    //##====================================================##
    void SaveOption_and_Return_Menu() 
    {
        // Tweenを止める
        for (int i = 0; i < cursor_tweens.Length; i++)
            cursor_tweens[i]?.Kill();


        // 選択したオプションを反映
        OptionData.current_options.controller = (chooseMessages[0][sub_choose[0]]) switch
        {
            "KEYBOARD" => OptionData.CONTROLLER_NAME_KEYBOARD,
            "GAMEPAD" => OptionData.CONTROLLER_NAME_GAMEPAD,
            "SCREENPAD" => OptionData.CONTROLLER_NAME_SCREENPAD,
            _ => throw new Exception("無効なオプションです"),
        };
        OptionData.current_options.is_play_shake = sub_choose[1] == 0;
        OptionData.current_options.dead_chara_color = sub_choose[2];
        OptionData.current_options.omitted_effect = sub_choose[3] == 1;

        // ファイルに保存
        SaveLoadControll.Seriarize<OptionData>(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME, OptionData.current_options);

        // ゲームパッドが使える環境ではゲームパッド設定も保存
        #if !(UNITY_ANDROID || UNITY_IOS)
        SaveLoadControll.Seriarize<Joystick_Setting>(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME, InputControll.joystick_setting);
        #endif
        
        // メニューへ遷移
        mcanim.MoveScene("Menu");
    }

    //##====================================================##
    //##                メニューを更新する処理              ##
    //##====================================================##
    void Renew_Board()
    {
        cursors[0].transform.SetParent(menu_objs[choose].transform);
        Vector3 vec = cursors[0].transform.localPosition;
        vec.y = 0f;
        cursors[0].transform.localPosition = vec;

        for (int i = 1; i < cursors.Length; i++)
        {
            // メニューに戻る　を選択しているだけ右側の２つのカーソルは消す
            if (choose == menu_objs.Length - 1)
            {
                cursors[i].SetActive(false);
            }
            else
            {
                cursors[i].SetActive(true);
                cursors[i].transform.SetParent(change_item_objs[choose].transform);
                vec = cursors[i].transform.localPosition;
                vec.y = 0f;
                cursors[i].transform.localPosition = vec;
            }
        }

        // 現在の設定表示を更新
        for(int i = 0; i < change_item_objs.Length; i++) 
        {
            change_item_objs[i].text = chooseMessages[i][sub_choose[i]];
        }
    }
}
