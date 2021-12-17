using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CautionWindowControll : MonoBehaviour
{
    [SerializeField]
    GameObject texts;

    [SerializeField]
    Text explain_text;

    [SerializeField]
    Image background;

    [SerializeField]
    Text under_triangle_text;

    string[] under_triangle_chars = new string[2] { "��", "��" };


    public bool? Is_complete { get; private set; } = null;


    //�x������\������E�B���h�E�i����{�^���ŃE�B���h�E�����j

    // ����
    // 3 -> ���̂��R���[�`�����g��Component
    // 4 -> �E�B���h�E�̑傫��
    // 5 -> �E�B���h�E�ɕ\������ē��e�L�X�g
    // 6 -> �E�B���h�E�̕\���ʒu(��ʂ̒��S��(0,0))

    /*  �L�q��
            StartCoroutine(PopUpWindowControll.CreateSmallWindow(this,new Vector2(200f, 160f), CONFIRM_SAVE_TEXT,Vector2.zero));
    */
    public static IEnumerator CreateSmallWindow(MonoBehaviour calling_component,Vector2 window_size,string guide_text, Vector2 position) 
    {
        Transform canvas_transform = GameObject.Find("Canvas").transform;
        GameObject window = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_UIS + "PopUpWindow_Caution")), canvas_transform.position + (Vector3)position, Quaternion.identity, canvas_transform);
        CautionWindowControll cautionWindowControll = window.GetComponent<CautionWindowControll>();

        cautionWindowControll.StartCoroutine(cautionWindowControll.Blink_of_UnderTriangleIcon());

        cautionWindowControll.Setting(window_size, guide_text);

        //calling_component.gameObject.SetActive(false);
        calling_component.enabled = false;

        
        yield return new WaitUntil(() =>
        {
            if (window != null)
            {
                return false;
            }

            return true;
        });


        calling_component.enabled = true;
        yield break;
    }

    // Update is called once per frame
    void Update()
    {

        // �I�����Ă��Ȃ��Ȃ�
        if (Is_complete == false)
        {
            // ����L�[�������������
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                WindowClose();
            }
        }

    }

    // ���e�̐ݒ肷��
    // �����@�P���E�B���h�E�̑傫���A�Q���\�������e�L�X�g
    public void Setting(Vector2 scale,string text) 
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        explain_text.text = text;

        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.MENUOPENCLOSE);

        rectTransform.DOSizeDelta(new Vector2(scale.x, rectTransform.sizeDelta.y), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
         {
             rectTransform.DOSizeDelta(scale, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
             {
                 // ��������\��
                 texts.gameObject.SetActive(true);
                 texts.transform.Find("text").GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.sizeDelta.x - 20f, rectTransform.sizeDelta.y * 0.75f - 40f);
                 under_triangle_text.gameObject.SetActive(true);

                 Is_complete = false;


             });

         });

    }
    // �E�B���h�E����鏈��
    void WindowClose()
    {
        Is_complete = true;

        RectTransform rectTransform = GetComponent<RectTransform>();

        texts.gameObject.SetActive(false);
        under_triangle_text.gameObject.SetActive(false);
        rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 18f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            rectTransform.DOSizeDelta(new Vector2(18f,18f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(this.gameObject);
                StopAllCoroutines();
            });

        });


    }

    public IEnumerator Blink_of_UnderTriangleIcon() 
    {
        var wait_time = new WaitForSeconds(0.5f);

        while (true) 
        {
            yield return wait_time;
            under_triangle_text.text = under_triangle_chars[1].ToString();
            yield return wait_time;
            under_triangle_text.text = under_triangle_chars[0].ToString();
        }
    }




}
