using UnityEngine;
using System.Collections;
public class AspectControll : MonoBehaviour
{
    // �c����ł��B�C���X�y�N�^����C�����܂��B
    public float m_x_aspect = 4.0f;
    public float m_y_aspect = 3.0f;
    void Awake()
    {
        // �J�������������܂��B
        Camera camera = GetComponent<Camera>();
        // �w�肳�ꂽ�䗦����T�C�Y���o���܂��B
        Rect rect = CalcAspect(m_x_aspect, m_y_aspect);
        // �J�����̔䗦��ύX���܂��B
        camera.rect = rect;
    }
    // �A�X�y�N�g��v�Z
    public Rect CalcAspect(float width, float height)
    {
        float target_aspect = width / height;
        float window_aspect = (float)Screen.width / (float)Screen.height;
        float scale_height = window_aspect / target_aspect;
        Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        if (1.0f > scale_height)
        {
            rect.x = 0;
            rect.y = (1.0f - scale_height) / 2.0f;
            rect.width = 1.0f;
            rect.height = scale_height;
        }
        else
        {
            float scale_width = 1.0f / scale_height;
            rect.x = (1.0f - scale_width) / 2.0f;
            rect.y = 0.0f;
            rect.width = scale_width;
            rect.height = 1.0f;
        }
        return rect;
    }
}

