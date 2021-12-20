using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public delegate void Choose_Yes();
public delegate void Choose_No();

//--====================================================--
//--   はいいいえウィンドウを出す処理を統括するクラス   --
//--====================================================--
public class PopUpWindowControll : MonoBehaviour
{
    [SerializeField,Tooltip("説明文を表示するTextコンポーネント")]
    Text explain_text;

    [SerializeField,Tooltip("ウィンドウの背景")]
    Image background;

    [SerializeField,Tooltip("選択肢 YES と NO を表示するTextコンポーネント")]
    Text[] yes_no = new Text[2];

    // 返り値 (どちらの選択肢を選択したか)
    public bool return_value = false;

    // ウィンドウを閉じたかどうか
    public bool? Is_complete { get; private set; } = null;

    // 選択中の選択肢 (0 or 1)
    int choose = 0;

    Tween active_chooses_tween;


    // はい・いいえウィンドウを呼び出し、選択させる処理

    // 引数
    // 1 -> はいを選んだ時にする処理メソッド
    // 2 -> いいえ 同様
    // 3 -> このをコルーチンを使うComponent
    // 4 -> ウィンドウの大きさ
    // 5 -> ウィンドウに表示する案内テキスト
    // 6 -> ウィンドウの表示位置(画面の中心が(0,0))

    /*  記述例
            StartCoroutine(PopUpWindowControll.CreateSmallWindow(
            delegate ()
            {
                Pausing(false);
            },
            delegate ()
            {
                Pausing(false);
            }, this,new Vector2(200f, 160f), CONFIRM_SAVE_TEXT,Vector2.zero));
    */
    //##====================================================##
    //##              ウィンドウを呼び出す処理              ##
    //##====================================================##
    public static IEnumerator CreateSmallWindow(Choose_Yes yes_func,Choose_No no_func,MonoBehaviour calling_component,Vector2 window_size,string guide_text, Vector2 position) 
    {        
        // ウィンドウを作る
        GameObject window = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_UIS + "PopUpWindow_YorN")), position, Quaternion.identity, GameObject.Find("Canvas").transform);
        PopUpWindowControll popUpWindowControll = window.GetComponent<PopUpWindowControll>();

        // ウィンドウの設定をする
        popUpWindowControll.Setting(window_size, guide_text);

        bool return_value = false;
        calling_component.enabled = false;

        // ウィンドウが消えるまで待つ
        yield return new WaitUntil(() =>
        {
            if (window != null)
            {
                return_value = popUpWindowControll.return_value;
                return false;
            }

            return true;
        });

        if (return_value) // はい の処理
            yes_func();
        else // いいえ の処理
            no_func();

        yield break;
    }

    //##====================================================##
    //##              Update    ウィンドウ操作              ##
    //##====================================================##
    void Update()
    {
        // 選択終了していないなら
        if (Is_complete == false)
        {
            // カーソルを左に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_LEFTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = 1;
                else
                    choose -= 1;
                Renew_Board();
            }
            // カーソルを右に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_RIGHTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board();
            }

            // 決定キーを押したらウィンドウを閉じる
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                switch (choose)
                {
                    case 0: 
                        {
                            return_value = true;
                            break;
                        }
                    case 1: 
                        {
                            return_value = false;
                            break;
                        }
                    default:
                        break;

                }
                WindowClose();
            }
            // キャンセルキーでも閉じれる
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
                return_value = false;
                WindowClose();            
            }
        }
    }

    //##====================================================##
    //##               メニューを更新する処理               ##
    //##====================================================##
    void Renew_Board()
    {
        // 上下運動させるテキストを変更
        active_chooses_tween.timeScale = 0f;
        active_chooses_tween?.Kill();
        yes_no[choose == 1 ? 0 : 1].rectTransform.localPosition = new Vector3(yes_no[choose == 1 ? 0 : 1].rectTransform.localPosition.x, yes_no[choose].rectTransform.localPosition.y, 0f);
        active_chooses_tween = yes_no[choose].rectTransform.DOMoveY(yes_no[choose].rectTransform.localPosition.y + 5f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
    }

    //##====================================================##
    //##                    内容の設定する                  ##
    //##====================================================##
    // 引数　１→ウィンドウの大きさ、２→表示されるテキスト
    public void Setting(Vector2 scale,string text) 
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        explain_text.text = text;

        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.MENUOPENCLOSE);

        rectTransform.DOSizeDelta(new Vector2(scale.x, rectTransform.sizeDelta.y), 0.2f).SetEase(Ease.InOutCirc).OnComplete(() =>
         {
             rectTransform.DOSizeDelta(scale, 0.2f).SetEase(Ease.InOutCirc).OnComplete(() =>
             {
                 // 説明文を表示
                 explain_text.transform.parent.gameObject.SetActive(true);
                 explain_text.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.sizeDelta.x - 20f, rectTransform.sizeDelta.y * 0.75f - 40f);

                 // yesとnoの選択肢の位置を調整
                 yes_no[0].rectTransform.localPosition = new Vector3(rectTransform.sizeDelta.x / -6f, rectTransform.sizeDelta.y * -0.125f, 0f);
                 yes_no[1].rectTransform.localPosition = new Vector3(rectTransform.sizeDelta.x / 6f, rectTransform.sizeDelta.y * -0.125f, 0f);

                 active_chooses_tween = yes_no[choose].rectTransform.DOMoveY(yes_no[choose].rectTransform.localPosition.y + 5f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);

                 Is_complete = false;
             });

         });

    }

    //##====================================================##
    //##               ウィンドウを閉じる処理               ##
    //##====================================================##
    void WindowClose()
    {
        active_chooses_tween?.Kill();
        Is_complete = true;

        RectTransform rectTransform = GetComponent<RectTransform>();

        explain_text.transform.parent.gameObject.SetActive(false);
        rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 18f), 0.2f).SetEase(Ease.InOutCirc).OnComplete(() =>
        {
            rectTransform.DOSizeDelta(new Vector2(18f,18f), 0.2f).SetEase(Ease.InOutCirc).OnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        });
    }
}
