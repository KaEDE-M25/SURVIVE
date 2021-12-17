using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--           �E�F�[�u�̐i�s���Ǘ�����N���X           --
//--====================================================--
public class WaveControll : MonoBehaviour
{
    // �^�C�}�[
    [SerializeField]
    Timer timer;
    // �^�C�}�[��\������e�L�X�g
    [SerializeField]
    Text timer_text;
    // �Q�[���i�s�𑀍삷��R���|�[�l���g
    [SerializeField]
    GameControll game_controll;
    // �E�F�[�u�i�s�x��\���o�[�̃X�v���C�g
    [SerializeField]
    Image fill_gauge;
    [SerializeField]
    Image empty_gauge;

    //�@�v�Z�p�ϐ�
    Vector2 fill_gauge_size = new Vector2(150f,6f);

    private void Update()
    {
            // �e�L�X�g�ɔ��f
            timer_text.text = timer.Current_time.ToString();
            // �Q�[�W�ɔ��f
            fill_gauge_size.x = ((float)timer.Max_count - (float)timer.Current_time) / (float)timer.Max_count * empty_gauge.rectTransform.sizeDelta.x;
            fill_gauge.rectTransform.sizeDelta = fill_gauge_size;

    }

    // timer�̃Q�b�^�[
    public Timer Timer() { return timer; }
}
