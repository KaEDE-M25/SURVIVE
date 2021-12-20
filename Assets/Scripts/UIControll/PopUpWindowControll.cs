using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public delegate void Choose_Yes();
public delegate void Choose_No();

//--====================================================--
//--   �͂��������E�B���h�E���o�������𓝊�����N���X   --
//--====================================================--
public class PopUpWindowControll : MonoBehaviour
{
    [SerializeField,Tooltip("��������\������Text�R���|�[�l���g")]
    Text explain_text;

    [SerializeField,Tooltip("�E�B���h�E�̔w�i")]
    Image background;

    [SerializeField,Tooltip("�I���� YES �� NO ��\������Text�R���|�[�l���g")]
    Text[] yes_no = new Text[2];

    // �Ԃ�l (�ǂ���̑I������I��������)
    public bool return_value = false;

    // �E�B���h�E��������ǂ���
    public bool? Is_complete { get; private set; } = null;

    // �I�𒆂̑I���� (0 or 1)
    int choose = 0;

    Tween active_chooses_tween;


    // �͂��E�������E�B���h�E���Ăяo���A�I�������鏈��

    // ����
    // 1 -> �͂���I�񂾎��ɂ��鏈�����\�b�h
    // 2 -> ������ ���l
    // 3 -> ���̂��R���[�`�����g��Component
    // 4 -> �E�B���h�E�̑傫��
    // 5 -> �E�B���h�E�ɕ\������ē��e�L�X�g
    // 6 -> �E�B���h�E�̕\���ʒu(��ʂ̒��S��(0,0))

    /*  �L�q��
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
    //##              �E�B���h�E���Ăяo������              ##
    //##====================================================##
    public static IEnumerator CreateSmallWindow(Choose_Yes yes_func,Choose_No no_func,MonoBehaviour calling_component,Vector2 window_size,string guide_text, Vector2 position) 
    {        
        // �E�B���h�E�����
        GameObject window = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_UIS + "PopUpWindow_YorN")), position, Quaternion.identity, GameObject.Find("Canvas").transform);
        PopUpWindowControll popUpWindowControll = window.GetComponent<PopUpWindowControll>();

        // �E�B���h�E�̐ݒ������
        popUpWindowControll.Setting(window_size, guide_text);

        bool return_value = false;
        calling_component.enabled = false;

        // �E�B���h�E��������܂ő҂�
        yield return new WaitUntil(() =>
        {
            if (window != null)
            {
                return_value = popUpWindowControll.return_value;
                return false;
            }

            return true;
        });

        if (return_value) // �͂� �̏���
            yes_func();
        else // ������ �̏���
            no_func();

        yield break;
    }

    //##====================================================##
    //##              Update    �E�B���h�E����              ##
    //##====================================================##
    void Update()
    {
        // �I���I�����Ă��Ȃ��Ȃ�
        if (Is_complete == false)
        {
            // �J�[�\�������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_LEFTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = 1;
                else
                    choose -= 1;
                Renew_Board();
            }
            // �J�[�\�����E�Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_RIGHTARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board();
            }

            // ����L�[����������E�B���h�E�����
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
            // �L�����Z���L�[�ł������
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
                return_value = false;
                WindowClose();            
            }
        }
    }

    //##====================================================##
    //##               ���j���[���X�V���鏈��               ##
    //##====================================================##
    void Renew_Board()
    {
        // �㉺�^��������e�L�X�g��ύX
        active_chooses_tween.timeScale = 0f;
        active_chooses_tween?.Kill();
        yes_no[choose == 1 ? 0 : 1].rectTransform.localPosition = new Vector3(yes_no[choose == 1 ? 0 : 1].rectTransform.localPosition.x, yes_no[choose].rectTransform.localPosition.y, 0f);
        active_chooses_tween = yes_no[choose].rectTransform.DOMoveY(yes_no[choose].rectTransform.localPosition.y + 5f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
    }

    //##====================================================##
    //##                    ���e�̐ݒ肷��                  ##
    //##====================================================##
    // �����@�P���E�B���h�E�̑傫���A�Q���\�������e�L�X�g
    public void Setting(Vector2 scale,string text) 
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        explain_text.text = text;

        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.MENUOPENCLOSE);

        rectTransform.DOSizeDelta(new Vector2(scale.x, rectTransform.sizeDelta.y), 0.2f).SetEase(Ease.InOutCirc).OnComplete(() =>
         {
             rectTransform.DOSizeDelta(scale, 0.2f).SetEase(Ease.InOutCirc).OnComplete(() =>
             {
                 // ��������\��
                 explain_text.transform.parent.gameObject.SetActive(true);
                 explain_text.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.sizeDelta.x - 20f, rectTransform.sizeDelta.y * 0.75f - 40f);

                 // yes��no�̑I�����̈ʒu�𒲐�
                 yes_no[0].rectTransform.localPosition = new Vector3(rectTransform.sizeDelta.x / -6f, rectTransform.sizeDelta.y * -0.125f, 0f);
                 yes_no[1].rectTransform.localPosition = new Vector3(rectTransform.sizeDelta.x / 6f, rectTransform.sizeDelta.y * -0.125f, 0f);

                 active_chooses_tween = yes_no[choose].rectTransform.DOMoveY(yes_no[choose].rectTransform.localPosition.y + 5f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);

                 Is_complete = false;
             });

         });

    }

    //##====================================================##
    //##               �E�B���h�E����鏈��               ##
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
