using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmitterViewer : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxStickDeflectionAngle = 30;
    Transform stickL, stickR;
    public void setSticks(float thr, float yaw, float pit, float rol)
    {
        stickL.localEulerAngles = new Vector3(
            Mathf.Lerp(-maxStickDeflectionAngle, maxStickDeflectionAngle, (thr + 1f) / 2f),
            0,
            -Mathf.Lerp(-maxStickDeflectionAngle, maxStickDeflectionAngle, (yaw + 1f) / 2f));
        stickR.localEulerAngles = new Vector3(
            Mathf.Lerp(-maxStickDeflectionAngle, maxStickDeflectionAngle, (pit + 1f) / 2f),
            0,
            -Mathf.Lerp(-maxStickDeflectionAngle, maxStickDeflectionAngle, (rol + 1f) / 2f));
        //подсветка крайних положений
        if (
            thr < -1 + 0.01 || thr > 1 - 0.01 ||
            yaw < -1 + 0.01 || yaw > 1 - 0.01)        
            stickL.transform.localScale = 1.3f * Vector3.one;
        else
            stickL.transform.localScale = 1 * Vector3.one;
        if (
            pit < -1 + 0.01 || pit > 1 - 0.01 ||
            rol < -1 + 0.01 || rol > 1 - 0.01)
            stickR.transform.localScale = 1.3f * Vector3.one;
        else
            stickR.transform.localScale = 1 * Vector3.one;

    }

    void Start()
    {
        stickR = transform.Find("3dModel/Stick R");
        stickL = transform.Find("3dModel/Stick L");
    }
}
