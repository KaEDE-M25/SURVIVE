using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

//--====================================================--
//--            シーンTitleの管理を統括する             --
//--====================================================--
public class TitleSceneControll : MonoBehaviour
{
    [SerializeField,Tooltip("タイトルロゴのIMAGEコンポーネント")]
    Image title_logo;
    [SerializeField,Tooltip("PRESS ANY BUTTONのTEXTコンポーネント")]
    Text press_any_button;
    [SerializeField, Tooltip("シーン遷移アニメーションのコンポーネント")]
    MoveSceneAnimation mcanim;

    bool start_move = false;
    float time = 0f;

    [Header("テスト用")]
    [SerializeField,Tooltip("スクリーンサイズを表示するtextコンポーネント（テスト用）")]
    Text screensize;

    //##====================================================##
    //##               Update メニューへの遷移              ##
    //##====================================================##
    void Update()
    {
        screensize.text = Screen.currentResolution.width.ToString() + " / " + Screen.currentResolution.height.ToString();

        if (!start_move)
        {

            // PRESS ANY BUTTON の点滅処理
            time += Time.deltaTime;
            if (time > 0.5f)
            {
                switch ((int)press_any_button.color.a)
                {
                    case 0:
                        press_any_button.color = Color.white;
                        break;

                    case 1:
                        press_any_button.color = Color.clear;
                        break;

                    default:
                        break;
                }
                time = 0f;
            }

            // 何かボタンを押したりタップしたら遷移開始
            if (Input.anyKeyDown)
            {
                start_move = true;
                press_any_button.color = Color.clear;

                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.TITLE_PUSH);

                // 光沢演出
                title_logo.transform.Find("Light").DOLocalMoveX(135f, 1.5f).SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    StartCoroutine(GameStart());
                });
            }
        }
    }

    //##====================================================##
    //##      ゲーム開始処理 (設定のロードなどをする)       ##
    //##====================================================##
    IEnumerator GameStart() 
    {
        if (!Directory.Exists(EigenValue.GetSaveDataPath()))
            Directory.CreateDirectory(EigenValue.GetSaveDataPath());
        if(File.Exists(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME))
        OptionData.current_options = SaveLoadControll.Deserialize<OptionData>(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME);
        if (File.Exists(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME))
            InputControll.joystick_setting = SaveLoadControll.Deserialize<Joystick_Setting>(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME);
        
        // 無かったらオプション初期設定をする
        if (OptionData.current_options == default(OptionData))
        {
            string initial_controller;

            // OSに従って初期コントローラーを決定
            #if UNITY_ANDROID || UNITY_IOS
            initial_controller = OptionData.CONTROLLER_NAME_SCREENPAD;
            #else
            initial_controller = OptionData.CONTROLLER_NAME_KEYBOARD;
            InputControll.joystick_setting = new Joystick_Setting();
            SaveLoadControll.Seriarize<Joystick_Setting>(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME, InputControll.joystick_setting);
            #endif

            OptionData.current_options = new OptionData(controller: initial_controller);
            // 初期化したオプションを保存しておく
            SaveLoadControll.Seriarize<OptionData>(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME, OptionData.current_options);
        }

        mcanim.MoveScene("Menu");
        yield break;
    }
}
