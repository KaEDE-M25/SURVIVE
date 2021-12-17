using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--            �G�t�F�N�g�F�΂�����̔���              --
//--====================================================--
public class EF_Bomb : MonoBehaviour
{
    // �q�ɂ��锚���I�u�W�F�N�g
    [SerializeField]
    GameObject explosion_effect;

    // �������甚���܂łɂ����鎞��
    public static readonly float EXPLODE_TIME = 3f;

    // ���Ԍv���J�E���^�[
    float time_count = 0f;

    void Update()
    {
        time_count += Time.deltaTime;

        // �������ԂɂȂ�����
        if (time_count >= EXPLODE_TIME) 
        {
            // �����G�t�F�N�g�����g����O���ĕ\���J�n
            explosion_effect.transform.parent = transform.parent;
            explosion_effect.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
