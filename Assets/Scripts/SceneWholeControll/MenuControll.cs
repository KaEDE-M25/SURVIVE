using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--            シーンMenuの管理を統括する              --
//--====================================================--
public class MenuControll : MonoBehaviour
{
    [SerializeField,Tooltip("選択肢の説明文のobj")]
    Text explain_text;
    [SerializeField,Tooltip("メニューのカーソルのobj")]
    GameObject cursor;
    
    [SerializeField,Tooltip("選択肢の説明文")]
    string[] explains = new string[5];

    [SerializeField,Tooltip("選択肢のイメージ画像")]
    Image[] explain_images = new Image[5];

    [SerializeField,Tooltip("メニューの選択肢のobj")]
    GameObject[] menu_objs = new GameObject[5];

    [SerializeField,Tooltip("シーン遷移アニメーションのコンポーネント")]
    MoveSceneAnimation mcanim;

    Tween tween;
    Tween cursor_tween;
    int choose = 0;

    //##====================================================##
    //##                     Start 初期化                   ##
    //##====================================================##
    void Start()
    {
        cursor_tween = cursor.transform.DOLocalMoveX(cursor.transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);

        explain_text.text = explains[0];
    }

    //##====================================================##
    //##                 Update メニュー選択                ##
    //##====================================================##
    void Update()
    {
        // シーン遷移アニメーション中でなければ操作できる
        if (mcanim.fading <= 0)
        {
            // カーソルを上に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = menu_objs.Length - 1;
                else
                    choose -= 1;
                Renew_Board();
            }
            // カーソルを下に移動
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= menu_objs.Length - 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board();
            }

            // 決定キーを押したら遷移
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                switch (choose)
                {
                    case 0: //はじめから
                        {
                            mcanim.MoveScene("NewGameSetting");
                            break;
                        }
                    case 1: //つづきから
                        {
                            mcanim.MoveScene("SaveLoadGame");
                            break;
                        }
                    case 2: //モンスターずかん (未実装)
                        {
                            StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "まだ実装されていません！", Vector2.zero));
                            return;
                        }
                    case 3: // クレジット (未実装)
                        {
                            StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "まだ実装されていません！", Vector2.zero));
                            return;
                        }
                    case 4: //オプション
                        {
                            mcanim.MoveScene("OptionChange");
                            break;
                        }

                    default:
                        throw new System.Exception("Invalid menu content.");

                }

                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                tween?.Kill();
                cursor_tween?.Kill();
            }
        }
    }

    //##====================================================##
    //##                メニューを更新する処理              ##
    //##====================================================##
    void Renew_Board()
    {
        explain_text.text = "";

        cursor.transform.SetParent(menu_objs[choose].transform);
        Vector3 vec = cursor.transform.localPosition;
        vec.y = 0f;
        cursor.transform.localPosition = vec;
        
        for(int i=0;i < explain_images.Length;i++)
            explain_images[i].gameObject.SetActive(i == choose);
        
        tween?.Kill();
        tween = explain_text.DOText(explains[choose], 0.01f * explains[choose].Length).SetEase(Ease.Linear);
    }
}
