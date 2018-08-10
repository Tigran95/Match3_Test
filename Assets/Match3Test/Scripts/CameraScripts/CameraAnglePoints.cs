using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnglePoints : MonoBehaviour
{

    public static GameObject[] cameraAngels;

    private void Start()
    {
        cameraAngels = new GameObject[2];
        for (int i = 0; i < cameraAngels.Length; i++)
        {
            cameraAngels[i] = new GameObject();
            cameraAngels[i].transform.SetParent(Camera.main.transform, false);
            cameraAngels[i].transform.localScale = new Vector3(1, 1, 1);
        }
        cameraAngels[0].name = "bottomLeft";
        cameraAngels[1].name = "bottomRight";
    }

    private void Update()
    {
        float width = Camera.main.pixelWidth;
        float height = Camera.main.pixelHeight;
        cameraAngels[0].transform.position = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        cameraAngels[1].transform.position = Camera.main.ScreenToWorldPoint(new Vector2(width, 0));
    }
}
