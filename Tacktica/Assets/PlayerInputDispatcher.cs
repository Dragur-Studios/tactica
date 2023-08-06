using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputDispatcher : MonoBehaviour
{
    TackticaInput input;

    public Vector2 cameraMoveInput;
    public Vector2 cameraZoomInput;
    public bool isRotationEnabled;
    public Vector2 delta;

    private void OnEnable()
    {
        if(input == null)
        {
            input = new TackticaInput();
            input.Camera.Movement.performed += (ctx) => { cameraMoveInput = ctx.ReadValue<Vector2>(); };
            input.Camera.Zoom.performed += (ctx) => { cameraZoomInput = ctx.ReadValue<Vector2>(); };
            input.Camera.EnableRotate.performed += (ctx) => { isRotationEnabled = ctx.ReadValue<float>() != 0; };
            input.Camera.Delta.performed += (ctx) => { delta = ctx.ReadValue<Vector2>(); };
            input.Enable();
        }    
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
