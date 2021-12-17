using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ‚±‚ê‚ðŽg‚Á‚½
// https://qiita.com/pixelflag/items/ad817bdd64931e084a46
public class PixelObjectBase : MonoBehaviour
{
    Vector3 cash_position;

    void LateUpdate()
    {
        cash_position = transform.localPosition;
        transform.localPosition = new Vector3(
                Mathf.RoundToInt(cash_position.x),
                Mathf.RoundToInt(cash_position.y),
                Mathf.RoundToInt(cash_position.z)
                );
    }

    private void OnRenderObject()
    {
        transform.localPosition = cash_position;
    }
}
