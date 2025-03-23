using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerTouchInputs : MonoBehaviour
{
    public static PlayerTouchInputs instance;

    [SerializeField] private LayerMask shootLayer;

    [Header("TouchControls")] 
    [SerializeField] private Transform crosshairSprite;
    [SerializeField] Vector2 joystickSize = new Vector2(250, 250);
    [SerializeField] public FloatingJoystick joystickSC;

    private Finger movementFinger;
    private Vector2 movementAmount;


    private void OnEnable()
    {
        instance = this;
        EnableTouchStuff();
    }

    private void OnDisable()
    {
        DisableTouchStuff();
    }

    private void Awake()
    {
        instance = this;
    }
    
    private void Touch_onFingerMove(Finger movedFinger)
    {
        if(movedFinger == movementFinger)
        {
            Vector2 knobPos;
            float maxMovement = joystickSize.x / 2f;
            ETouch.Touch currentTouch = movedFinger.currentTouch;

            if(Vector2.Distance(currentTouch.screenPosition, joystickSC.rectTransform.anchoredPosition) > maxMovement)
            {
                knobPos = (currentTouch.screenPosition - joystickSC.rectTransform.anchoredPosition).normalized * maxMovement;
            }
            else
            {
                knobPos = currentTouch.screenPosition - joystickSC.rectTransform.anchoredPosition;
            }

            joystickSC.knob.anchoredPosition = knobPos;
            movementAmount = knobPos / maxMovement;
        }
    }

    private void Touch_onFingerDown(Finger touchedFinger)
    {
        //Only move if the finger is on the left side of the screen.
        if(movementFinger == null && touchedFinger.screenPosition.x < Screen.width / 4f && touchedFinger.screenPosition.y < Screen.height / 4f) //&& touchedFinger.screenPosition.y <= Screen.height / 2f)//Instead of checking if the finger is in left side of the screen we are checking if its below half of the screen to implement movement.
        {
            movementFinger = touchedFinger;
            movementAmount = Vector2.zero;
            joystickSC.gameObject.SetActive(true);
            joystickSC.rectTransform.sizeDelta = joystickSize;
            joystickSC.rectTransform.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
        }

        //if (touchedFinger.screenPosition.x > Screen.width / 2 && touchedFinger.screenPosition.y < Screen.height / 2) //If the finger touched rightmost corner of the screen, apply firing functions.
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(touchedFinger.screenPosition.x, touchedFinger.screenPosition.y, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, shootLayer))
        {
            crosshairSprite.position = touchedFinger.screenPosition;
            PlayerSC.instance.transform.LookAt(hit.point, Vector3.up);
            PlayerSC.instance.Attack();
        }
    }

    private void Touch_onFingerUp(Finger lostFinger)
    {
        if(lostFinger == movementFinger)
        {
            movementFinger = null;
            joystickSC.knob.anchoredPosition = Vector2.zero;
            joystickSC.gameObject.SetActive(false);
            movementAmount = Vector2.zero;
        }
    }

    private Vector2 ClampStartPosition(Vector2 startPos)
    {
        if(startPos.x < joystickSize.x / 2)
        {
            startPos.x = joystickSize.x / 2;
        }

        if (startPos.y < joystickSize.y / 2)
        {
            startPos.y = joystickSize.y / 2;
        }
        else if (startPos.y > Screen.height - joystickSize.y / 2)
            startPos.y = Screen.height - joystickSize.y / 2;

        return startPos;
    }

    public void EnableTouchStuff()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_onFingerDown;
        ETouch.Touch.onFingerUp += Touch_onFingerUp;
        ETouch.Touch.onFingerMove += Touch_onFingerMove;
    }

    public void DisableTouchStuff()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown -= Touch_onFingerDown;
        ETouch.Touch.onFingerUp -= Touch_onFingerUp;
        ETouch.Touch.onFingerMove -= Touch_onFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if(!PlayerSC.instance.isDead)
        {
            PlayerSC.instance.transform.LookAt(PlayerSC.instance.transform.position + new Vector3(movementAmount.x, 0, movementAmount.y), Vector3.up);
            PlayerSC.instance.Movement(movementAmount);
        }

        if (Input.GetKey(KeyCode.W))
        {
            //PlayerSC.instance.transform.LookAt(PlayerSC.instance.transform.position + new Vector3(0, 0, 1), Vector3.up);
            PlayerSC.instance.Movement(new Vector2(0, 1));
        }
        if (Input.GetKey(KeyCode.S))
        {
            //PlayerSC.instance.transform.LookAt(PlayerSC.instance.transform.position + new Vector3(0, 0, -1), Vector3.up);
            PlayerSC.instance.Movement(new Vector2(0, -1));
        }
        if (Input.GetKey(KeyCode.A))
        {
            //PlayerSC.instance.transform.LookAt(PlayerSC.instance.transform.position + new Vector3(-1, 0, 0), Vector3.up);
            PlayerSC.instance.Movement(new Vector2(-1, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            //PlayerSC.instance.transform.LookAt(PlayerSC.instance.transform.position + new Vector3(1, 0, 0), Vector3.up);
            PlayerSC.instance.Movement(new Vector2(1, 0));
        }
        if (Input.GetKey(KeyCode.Space))
        {
            PlayerSC.instance.Attack();
        }
    }
}
