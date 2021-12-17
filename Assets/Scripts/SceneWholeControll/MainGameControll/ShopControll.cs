using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


//--====================================================--
//--            �V���b�v��ʂ��Ǘ�����N���X            --
//--====================================================--
public class ShopControll : MonoBehaviour
{
    // �e��ʂ�ID
    const int MenuContent_MAINMENU = -1;
    const int MenuContent_BUY = 0;
    const int MenuContent_ORGANIZE = 1;
    //const int MenuContent_EXIT = 2;

    // �A�C�e�����w���s�ł��邱�Ƃ��������߂̈É��J���[
    static readonly Color DIMMER_COLOR = new Color(0.3f, 0.3f, 0.3f, 1f);
    static readonly Vector3 INITIAL_ITEMSTOCK_POS = new Vector3(75f, 81f, 0f);
    static readonly Vector3 ITEMSTOCK_POS_WHEN_ORGANIZE = new Vector3(-75f, 20f, 0f);

    // �~�Z��ʑS�̂̃E�B���h�E�w�i
    [SerializeField]
    Image background;
    // ����������b�Z�[�W�ɕt�����Ă���L�[�A�C�R���̐eobj
    [SerializeField]
    GameObject[] explainer_icon_parents;

    // ���j���[��ʂŎg���ϐ�
    #region menu_variable

    // ���j���[��ʂ̑���obj�̐e
    [SerializeField]
    GameObject mainmenu_decorateobj_parent;

    // ���j���[��ʂ̑I����obj
    [SerializeField]
    GameObject[] main_choices_obj = new GameObject[3];

    // �e��Ԃ�\�������邽�߂̐eobj
    [SerializeField]
    GameObject main_nenu_parent;
    [SerializeField]
    GameObject[] main_choices_parent = new GameObject[3];

    // �J�[�\��
    [SerializeField]
    GameObject main_cursor;
    #endregion

    // �A�C�e���w����ʂŎg���ϐ�
    #region buy_variable

    // �I������A�C�e����obj
    [SerializeField]
    Image[] item_choices = new Image[8];
    // �I�����Ă���A�C�e���̒l�i�{�[�h�̃A�C�e������Text�R���|
    [SerializeField]
    Text chosen_item_name;
    // �I�����Ă���A�C�e���̒l�i�{�[�h�̒l�i��Text�R���|
    [SerializeField]
    Text chosen_item_price;
    // �I�����Ă���A�C�e���̐���������Text�R���|
    [SerializeField]
    Text chosen_item_explainer;



    //�J�[�\��
    [SerializeField]
    GameObject buy_cursor;

    #endregion

    // �A�C�e��������ʂŎg���ϐ�
    #region organize_variable
    
    // �ړ����̃X�g�b�N�ʒu (����(�I���������ꂩ��I��)�ꍇ��-1�ŕ\��)
    int organize_source = -1;
    // �ړ����������J�[�\�� 
    Image organize_souce_cursor;

    #endregion

    // �ďo����GameControll�I�u�W�F
    GameControll game_controller;

    // �Ԃ�l�i�g��Ȃ��H�j
    public bool return_value = false;

    // ���̃E�B���h�E�ł̏����������������ǂ��� (�����g���ĂȂ�)
    public bool? Is_complete { get; private set; } = null;

    // ��������̃G�t�F�N�g���Đ����Ȃ�(�G�t�F�N�g�Đ����ɍs�������Ȃ��p)
    bool is_effect_playing = false;

    // �ǂ̉�ʂɂ��邩
    // -2 -> �����l�A-1 -> ���j���[��ʁA 0,1 -> �z��main_choices_obj�̏��� (0 -> �w����ʁA1 -> �������)
    public int Choose_main { get; set; } = -2;

    int choose = 0;

    Tween cursor_tween;

    //##====================================================##
    //##          �V���b�v�E�B���h�E���Ăяo������          ##
    //##====================================================##
    public static IEnumerator CreateShopWindow(GameControll game_controller) 
    {        
        GameObject window = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_UIS + "PopUpWindow_Shop")), new Vector2(game_controller.transform.position.x,-20f), Quaternion.identity, GameObject.Find("Canvas").transform);
        ShopControll shopControll = window.GetComponent<ShopControll>();


        shopControll.game_controller = game_controller;

        game_controller.enabled = false;
        shopControll.Setting();

        // ���{�^���։��z�p�b�h��ύX
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
    //##                  Update ���C�����[�v               ##
    //##====================================================##
    void Update()
    {
        // ��ʂ̏�Ԃɂ���đ���֐���ύX
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
    //##             �ŏ��̃��j���[�̑�����s��             ##
    //##====================================================##
    void MainMenuControll()
    {
        // �J�ڃG�t�F�N�g���łȂ����
        if (!is_effect_playing)
        {
            // �I���I�����Ă��Ȃ��Ȃ�
            if (Is_complete == false)
            {
                // �J�[�\������Ɉړ�
                if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);

                    if (choose <= 0)
                        choose = main_choices_obj.Length - 1;
                    else
                        choose -= 1;
                    Renew_Board();
                }
                // �J�[�\�������Ɉړ�
                if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                    if (choose >= main_choices_obj.Length - 1)
                        choose = 0;
                    else
                        choose += 1;
                    Renew_Board();
                }

                // ����L�[����������ԓ���Ԃ�
                if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                    switch (choose)
                    {
                        // �A�C�e���𔃂�
                        case 0:
                            {
                                // �A�C�e���𔃂���Ԃ֑J��
                                Choose_main = MenuContent_BUY;
                                main_choices_parent[0].SetActive(true);
                                main_nenu_parent.SetActive(false);

                                choose = 0;
                                Renew_Board();
                                break;
                            }
                        // �A�C�e���𐮗�����
                        case 1:
                            {
                                // �A�C�e���𐮗������Ԃ֑J��
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
                        // �~�Z���o��
                        case 2:
                            {
                                WindowClose();
                                break;
                            }
                        default:
                            break;

                    }
                }
                // �~�Z���o��
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
                {
                    return_value = true;
                    WindowClose();
                }
            }
        }
    }

    //##====================================================##
    //##            �A�C�e���𔃂���Ԃ̑�����s��          ##
    //##====================================================##
    void BuyMenuControll() 
    {
        // �G�t�F�N�g�����쒆�łȂ����
        if (!is_effect_playing)
        {
            // �A�C�e���X�g�b�N�̃J�[�\���ʒu���ړ��ł���
            bool need_renewboard = game_controller.ItemStockCursorControll();

            // �J�[�\�����E�Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_LEFTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose % 4 == 0)
                    choose += 3;
                else
                    choose -= 1;
                need_renewboard = true;
            }
            // �J�[�\�������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_RIGHTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose % 4 == 4 - 1)
                    choose -= 3;
                else
                    choose += 1;
                need_renewboard = true;
            }
            // �J�[�\�������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose > item_choices.Length - 1 - 4)
                    choose = (choose + 4) % 4;
                else
                    choose += 4;

                need_renewboard = true;
            }
            // �J�[�\������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose < 4)
                    choose = (choose - 4) + item_choices.Length;
                else
                    choose -= 4;

                need_renewboard = true;
            }

            // ����L�[����������I�����Ă���A�C�e���X�g�b�N�̈ʒu�ɃA�C�e�����w��
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                ItemData itemdata = EigenValue.GetItemData(item_choices[choose].name);
                // �w���\�Ȃ�w������
                if (Is_Buyable(itemdata))
                {
                    BuyItem(itemdata);
                }
            }
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                ReturnMenu();
            }

            // ��ʍX�V���K�v�ȏꍇ�͍X�V
            if (need_renewboard) Renew_Board();
        }
    }

    // �T�u�֐��F�ΏۂƂȂ�A�C�e���ɂ��āA�I�𒆂̃X�g�b�N�ʒu�֍w���\���ǂ������肷��
    bool Is_Buyable(ItemData itemdata) 
    {
        // �������f�[�^�̓X���[
        if (itemdata != null)
            // ���������l�i�������Ă���i�܂蔃����j�Ȃ�
            if (game_controller.Money() >= itemdata.price)
                // ����ނ̃A�C�e���������͋�̃X�g�b�N����Ȃ��Ƃ��̃X�g�b�N�ɂ͊i�[�ł��Ȃ�
                if (game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].num == 0
                    || itemdata.item_id == game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].item_id)
                    // �A�C�e���X�g�b�N�ɓ����Ă���A�C�e������������ɒB���Ă��Ȃ����
                    if (game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].num < itemdata.max_hold)
                        return true;
        return false;
    }

    // �T�u�֐��F�A�C�e���w������������
    void BuyItem(ItemData itemdata) 
    {
        is_effect_playing = true;

        game_controller.Money(-itemdata.price);

        // �w���A�C�e���̃A�C�R�����R�s�[���A�A�C�e���X�g�b�N�֔��ł����G�t�F�N�g
        GameObject bought_item_icon = Instantiate(item_choices[choose].gameObject, item_choices[choose].transform);
        for (int i = 0; i < bought_item_icon.transform.childCount; i++)
            bought_item_icon.transform.GetChild(i).gameObject.SetActive(false);
        bought_item_icon.transform.localPosition = Vector3.zero;

        bought_item_icon.GetComponent<RectTransform>().DOSizeDelta(new Vector2(10f, 10f), 0.5f);
        GameObject target_item_stock = game_controller.transform.parent.Find("Item_Stock/StockID_" + (game_controller.Item_stock_cursor + 1)).gameObject;

        bought_item_icon.transform.DOMoveX(target_item_stock.transform.position.x, 0.5f).SetEase(Ease.InOutSine);
        bought_item_icon.transform.DOMoveY(target_item_stock.transform.position.y, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            // ���ۂ̊l������
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.EFFECT.GETITEM);
            game_controller.Set_item_stock(itemdata.item_id, 1, game_controller.Item_stock_cursor);
            Resources.Load<Items>((EigenValue.PREFAB_DIRECTORY_ITEMS + itemdata.prefab_name)).Hold_Effect();
            Destroy(bought_item_icon);
            Renew_Board();
            is_effect_playing = false;
        });
    }

    //##====================================================##
    //##         �A�C�e���𐮗������Ԃ̑�����s��         ##
    //##====================================================##
    void OrganizeMenuControll()
    {
        // �G�t�F�N�g�����쒆�łȂ����
        if (!is_effect_playing)
        {
            // �A�C�e���X�g�b�N�̃J�[�\���ʒu���ړ��ł���
            bool need_renewboard = game_controller.ItemStockCursorControll();

            // �A�C�e����I�����ē���ւ��鏈��
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                switch (organize_source) 
                {
                    case -1: 
                        {
                            organize_source = game_controller.Item_stock_cursor;
                            // �J�[�\���𕡐����A�F��ύX
                            organize_souce_cursor = Instantiate<Image>(game_controller.item_stock_cursor_obj, game_controller.item_stock_cursor_obj.transform.parent);
                            organize_souce_cursor.color = Color.black;

                            break;
                        }

                    default:
                        {
                            // �����ʒu�������Ă��Ȃ���΁A�Q�̃A�C�e���X�g�b�N�̒��g�����ւ���
                            if (organize_source != game_controller.Item_stock_cursor)
                            {
                                // �A�C�e���X�g�b�N�Ɋi�[����Ă���A�C�e��ID��������������
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
            // �A�C�e�����̂Ă鏈��
            if (InputControll.GetInputDown(InputControll.INPUT_ID_X))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.ITEMDROP);
                if (game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].num > 0)
                {
                    // �̂Ă��ۂɕK�v�ȏ������������炷��
                    Resources.Load<Items>(EigenValue.PREFAB_DIRECTORY_ITEMS + (EigenValue.GetItemData(game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].item_id).prefab_name)).Drop();
                    // �A�C�e���X�g�b�N����A�C�e�����P���炷
                    game_controller.Set_item_stock(game_controller.Item_stocks.item_stocks[game_controller.Item_stock_cursor].item_id, -1, game_controller.Item_stock_cursor);
                }
            }
            // ���C�����j���[�ɂ��ǂ�
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

    // �T�u�֐��F�I�������Q�̃A�C�e���X�g�b�N�̒��g�����ւ���
    void ExchangeItem_In_ItemStock(int source_stock_id, int destination_stock_id) 
    {
        ItemStock src_cache = game_controller.Item_stocks.item_stocks[source_stock_id];
        ItemStock dst_cache = game_controller.Item_stocks.item_stocks[destination_stock_id];
            
        // �l��������
        game_controller.Item_stocks.item_stocks[destination_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[destination_stock_id].num = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].num = 0;

        // ���ꂼ��ɒl������
        game_controller.Set_item_stock(src_cache.item_id, src_cache.num, destination_stock_id);
        game_controller.Set_item_stock(dst_cache.item_id, dst_cache.num, source_stock_id);
    }

    // �T�u�֐��F�I�������Q�̃A�C�e���X�g�b�N�̎�ނ������̂Ƃ��A���M���̃A�C�e���𑗐M��ֈڂ��镪�����ڂ�
    void MoveSameItem_In_ItemStock(int source_stock_id,int destination_stock_id) 
    {
        ItemStock src_cache = game_controller.Item_stocks.item_stocks[source_stock_id];
        ItemStock dst_cache = game_controller.Item_stocks.item_stocks[destination_stock_id];
        int max_hold = EigenValue.GetItemData(game_controller.Item_stocks.item_stocks[destination_stock_id].item_id).max_hold;

        // �ő及�������z���Ȃ��悤�ɃA�C�e�����ړ�������
        if (max_hold - dst_cache.num < src_cache.num)
        { 
            src_cache.num -= max_hold - dst_cache.num; 
            dst_cache.num = max_hold; 
        }
        else // ���M���ɍő及�������z���邾���̐��������Ȃ炷�ׂĈړ������đ��M������ɂ���
        { 
            dst_cache.num += src_cache.num;
            src_cache.num = 0;
        }

        // �l��������
        game_controller.Item_stocks.item_stocks[destination_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[destination_stock_id].num = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].item_id = 0;
        game_controller.Item_stocks.item_stocks[source_stock_id].num = 0;

        // ���ꂼ��ɒl������
        game_controller.Set_item_stock(dst_cache.item_id, dst_cache.num, destination_stock_id);

        if (src_cache.num > 0) // ���M�����Ȃ��Ȃ��Ă������̃X�g�b�N�ɂȂ�悤�ɓ��͂���
            game_controller.Set_item_stock(src_cache.item_id, src_cache.num, source_stock_id);
        else
            game_controller.Set_item_stock(0, 0, source_stock_id);
    }



    //##====================================================##
    //##                ���C�����j���[�ɖ߂�                ##
    //##====================================================##
    void ReturnMenu() 
    {
        choose = Choose_main;
        Choose_main = -1;
        main_nenu_parent.SetActive(true);
        // �X�g�b�N��������߂����ہA�L���b�V�����c���Ă������
        organize_source = -1;
        if (organize_souce_cursor != null)
            Destroy(organize_souce_cursor.gameObject);
        foreach (GameObject obj in main_choices_parent)
            obj.SetActive(false);
        Renew_Board();
        // ���ʉ����Đ�
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
    }


    //##====================================================##
    //##                ���j���[���X�V���鏈��              ##
    //##====================================================##
    void Renew_Board()
    {
        switch (Choose_main) 
        {
            case MenuContent_MAINMENU:
                {
                    // �J�[�\�����X�V
                    main_cursor.transform.SetParent(main_choices_obj[choose].transform);
                    Vector3 vec = main_cursor.transform.localPosition;
                    vec.y = 0f;
                    main_cursor.transform.localPosition = vec;

                    break;
                }
            case MenuContent_BUY:
                {
                    // �J�[�\�����X�V
                    buy_cursor.transform.SetParent(item_choices[choose].transform);
                    buy_cursor.transform.localPosition = Vector3.zero;

                    ItemData itemdata;

                    // �w���ł���A�C�e���͂��̂܂܁A�ł��Ȃ��A�C�e���͈Â��\�������Ă���
                    for (int i = 0; i < item_choices.Length; i++)
                    {
                        itemdata = EigenValue.GetItemData(item_choices[i].name);

                        item_choices[i].color = Is_Buyable(itemdata) ? Color.white : DIMMER_COLOR;
                    }
                    itemdata = EigenValue.GetItemData(item_choices[choose].name);

                    if (itemdata != null)
                    {
                        // �l�i�{�[�h�Ɛ��������X�V
                        chosen_item_price.text = itemdata.price.ToString();
                        chosen_item_name.text = itemdata.item_name;
                        chosen_item_explainer.text = itemdata.item_comment;
                    }
                    else  // �������̃A�C�e���������ꍇ�̓G���[����\�������Ă���
                    {
                        chosen_item_price.text = "-----";
                        chosen_item_name.text = "���̃A�C�e���͖������ł�";
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
    //##                    ���e�̐ݒ肷��                  ##
    //##====================================================##
    // �����@�P���E�B���h�E�̑傫���A�Q���\�������e�L�X�g
    public void Setting() 
    {
        Vector2 scale = new Vector2(320f, 180f);

        RectTransform rectTransform = GetComponent<RectTransform>();

        // �J�[�\���ɃA�j���[�V����������
        cursor_tween = main_cursor.transform.DOLocalMoveX(main_cursor.transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);

        // �X��R�����g�������_���Ɉ�\��
        var owner_comment_parent = mainmenu_decorateobj_parent.transform.Find("OwnerComment");
        owner_comment_parent.GetChild(Random.Range(0, owner_comment_parent.childCount)).gameObject.SetActive(true);

        // �g�p���Ă���R���g���[���[�ɉ����ăL�[�A�C�R��������
        foreach(GameObject parent_obj in explainer_icon_parents) 
            parent_obj.transform.Find("icon-" + OptionData.current_options.controller).gameObject.SetActive(true);

        // �E�B���h�E�J�̌��ʉ�
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.MENUOPENCLOSE);

        // �E�B���h�E���J��
        rectTransform.DOSizeDelta(new Vector2(scale.x, rectTransform.sizeDelta.y), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
         {
             rectTransform.DOSizeDelta(scale, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
             {

                 // ���j���[��ʂ���n�߂�
                 Choose_main = -1;

                 Is_complete = false;
                 main_nenu_parent.SetActive(true);
                 mainmenu_decorateobj_parent.SetActive(true);

             });

         });

    }

    //##====================================================##
    //##               �E�B���h�E����鏈��               ##
    //##====================================================##
    void WindowClose()
    {
        Is_complete = true;

        cursor_tween?.Kill();

        main_nenu_parent.SetActive(false);
        mainmenu_decorateobj_parent.SetActive(false);

        RectTransform rectTransform = GetComponent<RectTransform>();
        // ���z�X�e�B�b�N�ɖ߂�
        InputControll.use_virtual_stick = true;

        // ���ʉ����Đ�
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.MENUOPENCLOSE);

        // ����G�t�F�N�g
        rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 18f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            rectTransform.DOSizeDelta(new Vector2(18f,18f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        });
    }
}
