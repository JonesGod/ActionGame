using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour
{
    public Texture2D pointerImage;
    // Start is called before the first frame update
    private void Awake()
    {
        Cursor.SetCursor(pointerImage, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
