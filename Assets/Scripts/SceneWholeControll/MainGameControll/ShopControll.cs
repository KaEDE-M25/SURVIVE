using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


//--====================================================--
//--            ショップ画面を管理するクラス            --
//--====================================================--
public class ShopControll : MonoBehaviour
{
    // 各画面のID
    const int MenuContent_MAINMENU = -1;
    const int MenuContent_BUY = 0;
    const int MenuContent_ORGANIZE = 1;
    //const int MenuContent_EXIT = 2;

    // アイテムが購入不可であることを示すための暗化カラー
    static readonly Color DIMMER_COLOR = new Color(0.3f, 0.3f, 0.3f, 1f);
    static readonly Vector3 INITIAL_ITEMSTOCK_POS = new Vector3(75f, 81f, 0f);
    static readonly Vector3 ITEMSTOCK_POS_WHEN_ORGANIZE = new Vector3(-75f, 20f, 0f);

    // ミセ画面全体のウィンドウ背景
    [SerializeField]
    Image background;
    // 操作説明メッセージに付随しているキーアイコンの親obj
    [SerializeField]
    GameObject[] explainer_icon_parents;

    // メニュー画面で使う変数
    #region menu_variable

    // メニュー画面の装飾objの親
    [SerializeField]
    GameObject mainmenu_decorateobj_parent;

    // メニュー画面の選択肢obj
    [SerializeField]
    GameObject[] main_choices_obj = new GameObject[3];

    // 各状態を表示させるための親obj
    [SerializeField]
    GameObject main_nenu_parent;
    [SerializeField]
    GameObject[] main_choices_parent = new GameObject[3];

    // カーソル
    [SerializeField]
    GameObject main_cursor;
    #endregion

    // アイテム購入画面で使う変数
    #region buy_variable

    // 選択するアイテムのobj
    [SerializeField]
    Image[] item_choices = new Image[8];
    // 選択しているアイテムの値段ボードのアイテム名のTextコンポ
    [SerializeField]
    Text chosen_item_name;
    // 選択しているアイテムの値段ボードの値段のTextコンポ
    [SerializeField]
    Text chosen_item_price;
    // 選択しているアイテムの説明を書くTextコンポ
    [SerializeField]
    Text chosen_item_explainer;



    //カーソル
    [SerializeField]
    GameObject buy_cursor;

    #endregion

    // アイテム整理画面で使う変数
    #region organize_variable
    
    // 移動元のストック位置 (無い(選択元をこれから選ぶ)場合は-1で表現)
    int organize_source = -1;
    // 移動元を示すカーソル 
    Image organize_souce_cursor;

    #endregion

    // 呼出元のGameControllオブジェ
    GameControll game_controller;

    // 返り値（使わない？）
    public bool return_value = false;

    // このウィンドウでの処理を完了したかどうか (多分使ってない)
    public bool? Is_complete { get; private set; } = null;

    // 何かしらのエフェクトが再生中なら(エフェクト再生中に行動させない用)
    bool is_effect_playing = false;

    // どの画面にいるか
    // -2 -> 初期値、-1 -> メニュー画面、 0,1 -> 配列main_choices_objの順番 (0 -> 購入画面、1 -> 整理画面)
    public int Choose_main { get; set; } = -2;

    int choose = 0;

    Tween cursor_tween;

    //##====================================================##
    //##          ショップウィンドウを呼び出す処理          ##
    //##====================================================##
    public static IEnumerator CreateShopWindow(GameControll game_controller) 
    {        
        GameObject window = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_UIS + "PopUpWindow_Shop")), new Vector2(game_controller.transform.position.x,-20f), Quaternion.identity, GameObject.Find("Canvas").transform);
        ShopControll shopControll = window.GetComponent<ShopControll>();


        shopControll.game_controller = game_controller;

        game_controller.enabled = false;
        shopControll.Setting();

        // 矢印ボタンへ仮想パッドを変更
        InputControll.use_virtual_stick = false;
        var inputcontroller = shopControll.transform.parent.Find("InputController");
        inputcontroller.GetComponent<InputControll>().Reset_VirtualStick();
        inputcontroller.SetAsLastSibling();
        yield return new WaitUntil(() =>
        {
            if (window != null)
                return false;

            return true;
        });

        game_controller.enabled = true;
        game_controller.Pausing(false);

        yield break;
    }

    //##====================================================##
    //##                  Update メインループ               ##
    //##====================================================##
    void Update()
    {
        // 画面の状態によって操作関数を変更
        switch (Choose_main) 
        {
            case MenuContent_MAINMENU:
                MainMenuControll();
                break;
            case MenuContent_BUY:
                BuyMenuControll();
                break;
            case MenuContent_ORGANIZE:
                OrganizeMenuControll();
                break;

            default:
                break;        
        }

    }

    //##====================================================##
    //##             最初のメニューの操作を行う             ##
    //##====================================================##
    void MainMenuControll()
    {
        // 遷移エフェクト中でなければ
        if (!is_effect_playing)
        {
            // 選択終了していないなら
            if (Is_complete == false)
            {
                // カーソルを上に移動
                if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);

                    if (choose <= 0)
                        choose = main_choices_obj.Length - 1;
                    else
                        choose -= 1;
                    Renew_Board();
                }
                // カーソルを下に移動
                if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                    if (choose >= main_choices_obj.Length - 1)
                        choose = 0;
                    else
                        choose += 1;
                    Renew_Board();
                }

                // 決定キーを押したら返答を返す
                if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                    switch (choose)
                    {
                        // アイテムを買う
                        case 0:
                            {
                                // アイテムを買う状態へ遷移
                                Choose_main = MenuContent_BUY;
                                main_choices_parent[0].SetActive(true);
                                main_nenu_parent.SetActive(false);

                                choose = 0;
                                Renew_Board();
                                break;
                            }
                        // アイテムを整理する
                        case 1:
                            {
                                // アイテムを整理する状態へ遷移
                                Choose_main = MenuContent_ORGANIZE;
                                main_choices_parent[1].SetActive(true);
                                main_nenu_parent.SetActive(false);

                                choose = 0;
                                Renew_Board();
                                is_effect_playing = true;
                                game_controller.item_stock_parent.transform.DOMove(ITEMSTOCK_POS_WHEN_ORGANIZE, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
                                {
                                    is_effect_playing = false;
                                });
                                break;
                            }
                        // ミセを出る
                        case 2:
                            {
                                WindowClose();
                                break;
                            }
                        default:
                            break;

                    }
                }
                // ミセを出る
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
                {
                    return_value = true;
                    WindowClose();
                }
            }
        }
    }

    //##====================================================##
    //##            アイテムを買う状態の操作を行う          ##
    //##====================================================##
    void BuyMenuControll() 
    {
        // エフェクトが動作中でなければ
        if (!is_effect_playing)
        {
            // アイテムストックのカーソル位置を移動できる
            bool need_renewboard = game_controller.ItemStockCursorControll();

            // カーソルを右に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_LEFTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose % 4 == 0)
                    choose += 3;
                else
                    choose -= 1;
                need_renewboard = true;
            }
            // カーソルを左に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_RIGHTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose % 4 == 4 - 1)
                    choose -= 3;
                else
                    choose += 1;
                need_renewboard = true;
            }
            // カーソルを下に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose > item_choices.Length - 1 - 4)
                    choose = (choose + 4) % 4;
                else
                    choose += 4;

                need_renewboard = true;
            }
            // カーソルを上に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose < 4)
                    choose = (choose - 4) + item_choices.Length;
                else
                    choose -= 4;

                need_renewboard = true;
            }

            // 決定キーを押したら選択しているアイテムストックの位置にアイテムを購入
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                ItemData itemdata = EigenValue.GetItemData(item_choices[choose].name);
                // 購入可能なら購入処理
                if (Is_Buyable(itemdata))
                {
                    BuyItem(itemdata);
                }
            }
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                ReturnMenu();
            }

            // 画面更新が必要な場合は更新
            if (need_renewboard) Renew_Board();
        }
    }

    // サブ関数：対象となるアイテムについて、選択中のストック位置へ購入可能かどうか判定する
    bool Is_Buyable(ItemData itemdata) 
    {
        // 未実装データはスルー
        if (itemdata != null)
            // 所持金が値段を上回っている（つまり買える）なら
            if (game_controller.Money() >= itemdata.price)
                // 同種類のアイテムもしくは空のストックじゃないとそのストックには格納できない
                if (game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].num == 0
                    || itemdata.item_id == game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].item_id)
                    // アイテムストックに入っているアイテムが所持上限に達していなければ
                    if (game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].num < itemdata.max_hold)
                        return true;
        return false;
    }

    // サブ関数：アイテム購入処理をする
    void BuyItem(ItemData itemdata) 
    {
        is_effect_playing = true;

        game_controller.Money(-itemdata.price);

        // 購入アイテムのアイコンをコピーし、アイテムストックへ飛んでいくエフェクト
        GameObject bought_item_icon = Instantiate(item_choices[choose].gameObject, item_choices[choose].transform);
        for (int i = 0; i < bought_item_icon.transform.childCount; i++)
            bought_item_icon.transform.GetChild(i).gameObject.SetActive(false);
        bought_item_icon.transform.localPosition = Vector3.zero;

        bought_item_icon.GetComponent<RectTransform>().DOSizeDelta(new Vector2(10f, 10f), 0.5f);
        GameObject target_item_stock = game_controller.transform.parent.Find("Item_Stock/StockID_" + (game_controller.Item_stock_cursor + 1)).gameObject;

        bought_item_icon.transform.DOMoveX(target_item_stock.transform.position.x, 0.5f).SetEase(Ease.InOutSine);
        bought_item_icon.transform.DOMoveY(target_item_stock.transform.position.y, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            // 実際の獲得処理
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.EFFECT.GETITEM);
            game_controller.Set_item_stock(itemdata.item_id, 1, game_controller.Item_stock_cursor);
            Resources.Load<Items>((EigenValue.PREFAB_DIRECTORY_ITEMS + itemdata.prefab_name)).Hold_Effect();
            Destroy(bought_item_icon);
            Renew_Board();
            is_effect_playing = false;
        });
    }

    //##====================================================##
    //##         アイテムを整理する状態の操作を行う         ##
    //##====================================================##
    void OrganizeMenuControll()
    {
        // エフェクトが動作中でなければ
        if (!is_effect_playing)
        {
            // アイテムストックのカーソル位置を移動できる
            bool need_renewboard = game_controller.ItemStockCursorControll();

            // アイテムを選択して入れ替える処理
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                switch (organize_source) 
                {
                    case -1: 
                        {
                            organize_source = game_controller.Item_stock_cursor;
                            // カーソルを複製し、色を変更
                            organize_souce_cursor = Instantiate<Image>(game_controller.item_stock_cursor_obj, game_controller.item_stock_cursor_obj.transform.parent);
                            organize_souce_cursor.color = Color.black;

                            break;
                        }

                    default:
                        {
                            // 同じ位置を示していなければ、２つのアイテムストックの中身を入れ替える
                            if (organize_source != game_controller.Item_stock_cursor)
                            {
                                // アイテムストックに格納されているアイテムIDが同じだったら
                                if (game_controller.Item_stocks.item_stocks[organize_source].item_id == game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].item_id)
                                    MoveSameItem_In_ItemStock(organize_source, game_controller.Item_stock_cursor);
                                else
                                    ExchangeItem_In_ItemStock(organize_source, game_controller.Item_stock_cursor);
                            }
                            organize_source = -1;
                            Destroy(organize_souce_cursor.gameObject);
                            break;
                        }
                }
            
            }
            // アイテムを捨てる処理
            if (InputControll.GetInputDown(InputControll.INPUT_ID_X))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.ITEMDROP);
                if (game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].num > 0)
                {
                    // 捨てた際に必要な処理があったらする
                    Resources.Load<Items>(EigenValue.PREFAB_DIRECTORY_ITEMS + (EigenValue.GetItemData(game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].item_id).prefab_name)).Drop();
                    // アイテムストックからアイテムを１減らす
                    game_controller.Set_item_stock(game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].item_id, -1, game_controller.Item_stock_cursor);
                }
            }
            // メインメニューにもどる
            if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                ReturnMenu();
                is_effect_playing = true;
                game_controller.item_stock_parent.transform.DOMove(INITIAL_ITEMSTOCK_POS, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    is_effect_playing = false;
                });
            }
        }
    }

    // サブ関数：選択した２つのアイテムストックの中身を入れ替える
    void ExchangeItem_In_ItemStock(int source_stock_id, int destination_stock_id) 
    {
        ItemStock src_cache = game_controller.Item_stocks.item_stocks[source_stock_id];
        ItemStock dst_cache = game_controller.Item_stocks.item_stocks[destination_stock_id];
            
        // 値を初期化
        game_controller.Item_stocks.item_stocks[destination_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[destination_stock_id].num = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].num = 0;

        // それぞれに値を入れる
        game_controller.Set_item_stock(src_cache.item_id, src_cache.num, destination_stock_id);
        game_controller.Set_item_stock(dst_cache.item_id, dst_cache.num, source_stock_id);
    }

    // サブ関数：選択した２つのアイテムストックの種類が同じのとき、送信元のアイテムを送信先へ移せる分だけ移す
    void MoveSameItem_In_ItemStock(int source_stock_id,int destination_stock_id) 
    {
        ItemStock src_cache = game_controller.Item_stocks.item_stocks[source_stock_id];
        ItemStock dst_cache = game_controller.Item_stocks.item_stocks[destination_stock_id];
        int max_hold = EigenValue.GetItemData(game_controller.Item_stocks.item_stocks[destination_stock_id].item_id).max_hold;

        // 最大所次数を越えないようにアイテムを移動させる
        if (max_hold - dst_cache.num < src_cache.num)
        { 
            src_cache.num -= max_hold - dst_cache.num; 
            dst_cache.num = max_hold; 
        }
        else // 送信元に最大所次数を越えるだけの数が無いならすべて移動させて送信元を空にする
        { 
            dst_cache.num += src_cache.num;
            src_cache.num = 0;
        }

        // 値を初期化
        game_controller.Item_stocks.item_stocks[destination_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[destination_stock_id].num = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].num = 0;

        // それぞれに値を入れる
        game_controller.Set_item_stock(dst_cache.item_id, dst_cache.num, destination_stock_id);

        if (src_cache.num > 0) // 送信元がなくなっていたら空のストックになるように入力する
            game_controller.Set_item_stock(src_cache.item_id, src_cache.num, source_stock_id);
        else
            game_controller.Set_item_stock(0, 0, source_stock_id);
    }



    //##====================================================##
    //##                メインメニューに戻る                ##
    //##====================================================##
    void ReturnMenu() 
    {
        choose = Choose_main;
        Choose_main = -1;
        main_nenu_parent.SetActive(true);
        // ストック整理から戻った際、キャッシュが残ってたら消す
        organize_source = -1;
        if (organize_souce_cursor != null)
            Destroy(organize_souce_cursor.gameObject);
        foreach (GameObject obj in main_choices_parent)
            obj.SetActive(false);
        Renew_Board();
        // 効果音を再生
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
    }


    //##====================================================##
    //##                メニューを更新する処理              ##
    //##====================================================##
    void Renew_Board()
    {
        switch (Choose_main) 
        {
            case MenuContent_MAINMENU:
                {
                    // カーソルを更新
                    main_cursor.transform.SetParent(main_choices_obj[choose].transform);
                    Vector3 vec = main_cursor.transform.localPosition;
                    vec.y = 0f;
                    main_cursor.transform.localPosition = vec;

                    break;
                }
            case MenuContent_BUY:
                {
                    // カーソルを更新
                    buy_cursor.transform.SetParent(item_choices[choose].transform);
                    buy_cursor.transform.localPosition = Vector3.zero;

                    ItemData itemdata;

                    // 購入できるアイテムはそのまま、できないアイテムは暗く表示させておく
                    for (int i = 0; i < item_choices.Length; i++)
                    {
                        itemdata = EigenValue.GetItemData(item_choices[i].name);

                        item_choices[i].color = Is_Buyable(itemdata) ? Color.white : DIMMER_COLOR;
                    }
                    itemdata = EigenValue.GetItemData(item_choices[choose].name);

                    if (itemdata != null)
                    {
                        // 値段ボードと説明文を更新
                        chosen_item_price.text = itemdata.price.ToString();
                        chosen_item_name.text = itemdata.item_name;
                        chosen_item_explainer.text = itemdata.item_comment;
                    }
                    else  // 未実装のアイテムだった場合はエラー文を表示させておく
                    {
                        chosen_item_price.text = "-----";
                        chosen_item_name.text = "このアイテムは未実装です";
                        chosen_item_explainer.text = "";

                    }
                    break;
                }
            case MenuContent_ORGANIZE: 
                {
                    break; 
                }

            default:
                break;
        
        }
    }

    //##====================================================##
    //##                    内容の設定する                  ##
    //##====================================================##
    // 引数　１→ウィンドウの大きさ、２→表示されるテキスト
    public void Setting() 
    {
        Vector2 scale = new Vector2(320f, 180f);

        RectTransform rectTransform = GetComponent<RectTransform>();

        // カーソルにアニメーションをつける
        cursor_tween = main_cursor.transform.DOLocalMoveX(main_cursor.transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);

        // 店主コメントをランダムに一つ表示
        var owner_comment_parent = mainmenu_decorateobj_parent.transform.Find("OwnerComment");
        owner_comment_parent.GetChild(Random.Range(0, owner_comment_parent.childCount)).gameObject.SetActive(true);

        // 使用しているコントローラーに応じてキーアイコンを決定
        foreach(GameObject parent_obj in explainer_icon_parents) 
            parent_obj.transform.Find("icon-" + OptionData.current_options.controller).gameObject.SetActive(true);

        // ウィンドウ開閉の効果音
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.MENUOPENCLOSE);

        // ウィンドウを開く
        rectTransform.DOSizeDelta(new Vector2(scale.x, rectTransform.sizeDelta.y), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
         {
             rectTransform.DOSizeDelta(scale, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
             {

                 // メニュー画面から始める
                 Choose_main = -1;

                 Is_complete = false;
                 main_nenu_parent.SetActive(true);
                 mainmenu_decorateobj_parent.SetActive(true);

             });

         });

    }

    //##====================================================##
    //##               ウィンドウを閉じる処理               ##
    //##====================================================##
    void WindowClose()
    {
        Is_complete = true;

        cursor_tween?.Kill();

        main_nenu_parent.SetActive(false);
        mainmenu_decorateobj_parent.SetActive(false);

        RectTransform rectTransform = GetComponent<RectTransform>();
        // 仮想スティックに戻す
        InputControll.use_virtual_stick = true;

        // 効果音を再生
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.MENUOPENCLOSE);

        // 閉じるエフェクト
        rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 18f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            rectTransform.DOSizeDelta(new Vector2(18f,18f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        });
    }
}
