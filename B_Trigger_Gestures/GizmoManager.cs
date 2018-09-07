using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GizmoManager : MonoBehaviour
{
    #region Singleton
    private static GizmoManager _instance;
    public static GizmoManager Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }
    #endregion
    [SerializeField]
    Image[] flagImages;
    [SerializeField]
    Image[] stateImages;
    [SerializeField]
    GameObject rotationGizmo;
    [SerializeField]
    GameObject handStatesGizmo;
    [SerializeField]
    GameObject manoClassGizmo;
    [SerializeField]
    GameObject triggerGestureGizmo;
    [SerializeField]
    GameObject continuousGestureGizmo;
    [SerializeField]
    GameObject depthGizmo;
    [SerializeField]
    GameObject flagGizmo;

    public float bulletSpeed = 10;
    public Rigidbody bullet;

    public Color disabledStateColor;
    [SerializeField]
    private bool _show_hand_states, _show_rotation, _show_mano_class, _show_trigger_gesture, _show_continuous_gesture, _show_bounding_box, _show_depth, _show_flags;
    public GameObject Object; //RK
    public bool Show_flags
    {
        get
        {
            return _show_flags;
        }
        set
        {
            _show_flags = value;
        }
    }
    Transform rotationIconTransform, rotationTriangleTransform;
    Text rotationValueText, depthValueText, manoClassText, continuousGestureText, triggerGestureText;
    Image depthValueImage, leftEdgeFlag, rightEdgeFlag, topEdgeFlag, botEdgeFlag;


    #region Properties


    public bool Show_hand_states
    {
        get
        {
            return _show_hand_states;
        }

        set
        {
            _show_hand_states = value;
        }
    }

    public bool Show_rotation
    {
        get
        {
            return _show_rotation;
        }

        set
        {
            _show_rotation = value;
        }
    }

    public bool Show_mano_class
    {
        get
        {
            return _show_mano_class;
        }

        set
        {
            _show_mano_class = value;
        }
    }

    public bool Show_trigger_gesture
    {
        get
        {
            return _show_trigger_gesture;
        }

        set
        {
            _show_trigger_gesture = value;
        }
    }

    public bool Show_continuous_gesture
    {
        get
        {
            return _show_continuous_gesture;
        }

        set
        {
            _show_continuous_gesture = value;
        }
    }

    public bool Show_bounding_box
    {
        get
        {
            return _show_bounding_box;
        }

        set
        {
            _show_bounding_box = value;
        }
    }

    public bool Show_depth
    {
        get
        {
            return _show_depth;
        }

        set
        {
            _show_depth = value;
        }
    }


    #endregion

    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        SetGestureDescriptionParts();
        SetDepthGizmoParts();
        SetRotationGizmoParts();
        HighlightStatesToStateDetection(0);

    

    }


    void Update()
    {
      

        


        DisplayRotationGizmo(ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info);
        DisplayManoclass(ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info);
        DisplayTriggerGesture(ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info);
        DisplayContinuousGesture(ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info);
        DisplayHandState(ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info);
        DisplayDepth(ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info);
        DisplayFlags(ManomotionManager.Instance.Hand_infos[0].hand_info.warning);

       


    }

    #region Display Methods

    void DisplayFlags(Warning warningFlag)
    {
        flagGizmo.SetActive(Show_flags);

        if (Show_flags)
        {
            if (warningFlag == Warning.WARNING_APPROACHING_UPPER_EDGE)
            {
                flagImages[0].color = new Color(85f / 255, 26f / 255, 139f / 255);

            }
            if (warningFlag == Warning.WARNING_APPROACHING_RIGHT_EDGE)
            {
                flagImages[1].color = new Color(85f / 255, 26f / 255, 139f / 255);

            }
            if (warningFlag == Warning.WARNING_APPROACHING_LOWER_EDGE)
            {
                flagImages[2].color = new Color(85f / 255, 26f / 255, 139f / 255);

            }
            if (warningFlag == Warning.WARNING_APPROACHING_LEFT_EDGE)
            {
                flagImages[3].color = new Color(85f / 255, 26f / 255, 139f / 255);

            }
        }
        for (int i = 0; i < flagImages.Length; i++)
        {
            flagImages[i].color = new Color(85f / 255, 26f / 255, 139f / 255, flagImages[i].color.a - Time.deltaTime);
        }

    }

    /// <summary>
    /// Displays the rotation tracked from a hand onto a visual gizmo
    /// </summary>
    /// <param name="tracking_info"> Requires tracking information</param>
    void DisplayRotationGizmo(TrackingInfo tracking_info)
    {
        rotationGizmo.SetActive(Show_rotation);

        if (Show_rotation)
        {
            float angle = Mathf.LerpAngle(rotationIconTransform.rotation.z, tracking_info.rotation, 0.8f);
            rotationIconTransform.eulerAngles = new Vector3(0, 0, -angle);
            rotationTriangleTransform.eulerAngles = new Vector3(0, 0, -angle);
            rotationValueText.text = tracking_info.rotation.ToString();
        }
        else
        {
            float angle = Mathf.LerpAngle(rotationIconTransform.rotation.z, 0, 0.8f);
            rotationIconTransform.eulerAngles = new Vector3(0, 0, angle);
            rotationTriangleTransform.eulerAngles = new Vector3(0, 0, angle);
            rotationValueText.text = "0";
        }
    }

    /// <summary>
    /// Displays information regarding the Mano class of the detected hand
    /// </summary>
    /// <param name="info"></param>
    void DisplayManoclass(GestureInfo gesture_info)
    {
        manoClassGizmo.SetActive(Show_mano_class);
        if (Show_mano_class)
        {
            ManoClass manoclass = gesture_info.mano_class;
            switch (manoclass)
            {
                case ManoClass.NO_HAND:
                    manoClassText.text = "Manoclass: ";
                    break;
                case ManoClass.GRAB_GESTURE_FAMILY:
                    manoClassText.text = "Manoclass: Grab Class";
                    break;
                case ManoClass.PINCH_GESTURE_FAMILY:
                    manoClassText.text = "Manoclass: Pinch Class";
                    break;
                case ManoClass.POINTER_GESTURE_FAMILY:
                    manoClassText.text = "Manoclass: Pointer Class";
                    break;
                default:
                    manoClassText.text = "Manoclass: ";
                    break;
            }

        }

    }

    /// <summary>
    /// Displays information regarding the Trigger Gesture of the detected hand
    /// </summary>
    /// <param name="gesture_info"></param>
    void DisplayTriggerGesture(GestureInfo gesture_info)
    {
        triggerGestureGizmo.SetActive(Show_trigger_gesture);
        if (Show_trigger_gesture)
        {
            ManoGestureTrigger trigger = gesture_info.mano_gesture_trigger;
            switch (trigger)
            {

                case ManoGestureTrigger.CLICK:
                    triggerGestureText.text = "Trigger: Click";
                    break;
                case ManoGestureTrigger.SWIPE_LEFT:
                    triggerGestureText.text = "Trigger: Swipe Left";
                    break;
                case ManoGestureTrigger.SWIPE_RIGHT:
                    triggerGestureText.text = "Trigger: Swipe Right";
                    break;
                case ManoGestureTrigger.GRAB:
                    triggerGestureText.text = "Trigger: Grab";
                    Object.GetComponent<Renderer>().material.color = Color.red;
                    Fire();
                    break;
                case ManoGestureTrigger.TAP_POINTING:
                    triggerGestureText.text = "Trigger: Tap";
                    break;
                case ManoGestureTrigger.DROP:
                    triggerGestureText.text = "Trigger: Drop";
                    break;
                case ManoGestureTrigger.PICK:
                    triggerGestureText.text = "Trigger: Pick";
                    break;
                case ManoGestureTrigger.RELEASE:
                    triggerGestureText.text = "Trigger: Release";
                    Object.GetComponent<Renderer>().material.color = Color.blue;
                    Fire();
                    break;
                case ManoGestureTrigger.NO_GESTURE:
                    triggerGestureText.text = "Trigger: ";
                    break;
                default:
                    triggerGestureText.text = "Trigger: ";
                    break;
            }





        }
    }

    /// <summary>
    /// Displays information regarding the Continuous Gesture of the detected hand
    /// </summary>
    /// <param name="gesture_info"></param>
    void DisplayContinuousGesture(GestureInfo gesture_info)
    {
        continuousGestureGizmo.SetActive(Show_continuous_gesture);
        if (Show_continuous_gesture)
        {
            ManoGestureContinuous continous = gesture_info.mano_gesture_continuous;
            switch (continous)
            {
                case ManoGestureContinuous.NO_GESTURE:
                    continuousGestureText.text = "Continuous: ";
                    break;
                case ManoGestureContinuous.HOLD_GESTURE:
                    continuousGestureText.text = "Continuous: Holding";
                    break;
                case ManoGestureContinuous.OPEN_HAND_GESTURE:
                    continuousGestureText.text = "Continuous: Open Hand";
                    break;
                case ManoGestureContinuous.OPEN_PINCH_GESTURE:
                    continuousGestureText.text = "Continuous: Open Pinch";
                    break;
                case ManoGestureContinuous.CLOSED_HAND_GESTURE:
                    continuousGestureText.text = "Continuous: Closed Hand";
                    break;
                case ManoGestureContinuous.POINTER_GESTURE:
                    continuousGestureText.text = "Continuous: Pointing";
                    break;
                case ManoGestureContinuous.PUSH_POINTING_GESTURE:
                    continuousGestureText.text = "Continuous: Push Pointing";
                    break;
                default:
                    continuousGestureText.text = "Continuous: ";
                    break;
            }



        }
    }


    /// <summary>
    /// Updates the visual information that showcases the hand state (how open/closed) it is
    /// </summary>
    /// <param name="gesture_info"></param>
    void DisplayHandState(GestureInfo gesture_info)
    {

        handStatesGizmo.SetActive(Show_hand_states && !ChooseBackgroundBehavior.Instance.backgroundMenuIsOpen);
        if (Show_hand_states)
        {
            HighlightStatesToStateDetection(gesture_info.state);
        }

        else
        {
            handStatesGizmo.SetActive(false);
        }


    }


    /// <summary>
    /// Displays the gizmo that shows the Depth values of the detected hand
    /// </summary>
    /// <param name="tracking_info"></param>
    void DisplayDepth(TrackingInfo tracking_info)
    {
        depthGizmo.SetActive(Show_depth);
        if (Show_depth)
        {
            depthValueText.text = tracking_info.relative_depth.ToString();
            depthValueImage.fillAmount = Mathf.Clamp(tracking_info.relative_depth * 2f, 0f, 1f);
        }
    }


    #endregion

    /// <summary>
    /// Visualizes the current hand state by coloring white the images up to that value and turning grey the rest
    /// </summary>
    /// <param name="stateValue">Requires a hand state value to assign the colors accordingly </param>
    void HighlightStatesToStateDetection(int stateValue)
    {

        for (int i = 0; i < stateImages.Length; i++)
        {
            if (i >= stateValue)
            {
                stateImages[i].color = disabledStateColor;
            }
            else
            {
                stateImages[i].color = Color.white;
            }


        }
    }

    /// <summary>
    /// Initializes the components of Rotation Gizmo
    /// </summary>
    void SetRotationGizmoParts()
    {
        rotationIconTransform = rotationGizmo.transform.Find("RotationIcon").GetComponent<Transform>();
        rotationTriangleTransform = rotationIconTransform.Find("Triangle").GetComponent<Transform>();
        rotationValueText = rotationGizmo.transform.Find("Value").GetComponent<Text>();
    }

    /// <summary>
    /// Initializes the components of the Depth Gizmo
    /// </summary>
    void SetDepthGizmoParts()
    {
        depthValueImage = depthGizmo.transform.Find("DepthLevel").GetComponent<Image>();
        depthValueText = depthGizmo.transform.Find("Value").GetComponent<Text>();
    }


    void Fire() //RK
    {
        
        Rigidbody bulletClone = (Rigidbody)Instantiate(bullet, Object.transform.position, transform.rotation);
        bulletClone.velocity = bullet.transform.forward * bulletSpeed;
       
    }
    /// <summary>
    /// Initializes the components of the Manoclass,Continuous Gesture & Trigger Gesture Gizmos
    /// </summary>
    void SetGestureDescriptionParts()
    {
        
        manoClassText = manoClassGizmo.transform.Find("Description").GetComponent<Text>();
        continuousGestureText = continuousGestureGizmo.transform.Find("Description").GetComponent<Text>();
        triggerGestureText = triggerGestureGizmo.transform.Find("Description").GetComponent<Text>();
    }


}
