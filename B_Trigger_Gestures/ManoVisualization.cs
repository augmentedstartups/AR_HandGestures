using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("ManoMotion/ManoVisualization")]
public class ManoVisualization : MonoBehaviour
{
    #region variables



    [Space(20)]

    [Header("Triggers")]
    [SerializeField]
    Color _outlinePointsColor;
    [SerializeField]
    Color _innerPointsColor, _clickLineColor, _tapLineColor, _grabLineColor, _releaseLineColor, _releasePinchLineColor, _grabPinchLineColor, _fingertipsLineColor;
    [SerializeField]
    Color _holdColor, _baseColor, _pinchColor, _openHandColor, _closedHandColor, _pushColor, _pointerColor;

    [SerializeField]
    private Transform outer_dots_prefab, inner_dots_prefab, bounding_box_prefab;

    [SerializeField]
    private bool _show_border_flags, _show_bounding_box, _show_contour, _show_inner, _show_fingertips, _show_fingertip_labels, _show_palm_center, _show_hand_layer, _show_background_layer, _show_joints;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    GameObject manomotionGenericLayer, layering_object;


    //public Text PositionText;

    private Transform[] bounding_box;
    private Transform[] palm;
    private Transform[][] contour_particles;
    private Transform[][] inner_particles;
    private LineRenderer[] contour_line_renderer;
    private BoundingBoxUI[] bounding_box_ui;
    private Transform[][] fingertips_t;
    private Transform[][] fingertip_labels;
    private String[] finger_labels = { "Pinky", "Ring", "Mid", "Index", "Thumb" };
    private GameObject contour_parent, inner_parent, fingertip_parent, palmcenter_parent, fingertip_label_parent, joints_parent, bounding_box_parent;
    private ManoUtils mano_utils;
    private MeshRenderer[] _layer_hands;
    private MeshRenderer _layer_background, _layer_transparent;

    int handsSupportedByLicence;



    #endregion


    #region Properties

    public bool Show_inner
    {
        get
        {
            return _show_inner;
        }

        set
        {
            _show_inner = value;
        }
    }

    public bool Show_fingertips
    {
        get
        {
            return _show_fingertips;
        }

        set
        {
            _show_fingertips = value;
        }
    }

    public bool Show_fingertip_labels
    {
        get
        {
            return _show_fingertip_labels;
        }

        set
        {
            _show_fingertip_labels = value;
        }
    }

    public bool Show_palm_center
    {
        get
        {
            return _show_palm_center;
        }

        set
        {
            _show_palm_center = value;
        }
    }

    public bool Show_contour
    {
        get
        {
            return _show_contour;
        }

        set
        {
            _show_contour = value;
        }
    }

    public bool Show_hand_layer
    {
        get
        {
            return _show_hand_layer;
        }

        set
        {
            _show_hand_layer = value;
        }
    }

    public bool Show_background_layer
    {
        get
        {
            return _show_background_layer;
        }

        set
        {
            _show_background_layer = value;
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

    public bool Show_joints
    {
        get
        {
            return _show_joints;
        }

        set
        {
            _show_joints = value;
        }
    }

    #endregion

    #region Initializing Components

    void Start()
    {

        if (!cam)
            cam = Camera.main;

        mano_utils = ManoUtils.Instance;
        SetHandsSupportedByLicence();
        SetPointsBasedOnHandNumber();
        CreatePalmCenterParticle();
        CreateFingerTipLabelParticles();
        CreateFingerTipParticles();
        CreateContourParticles();
        CreateInnerParticles();
        InstantiateManomotionMeshes();
        CreateBoundingBoxes();
    }

    /// <summary>
    /// Creates the bounding boxes that surround the hand. 
    /// </summary>
    void CreateBoundingBoxes()
    {
        bounding_box_parent = (new GameObject());
        bounding_box_parent.name = "bounding box parent";
        bounding_box = new Transform[handsSupportedByLicence];
        bounding_box_ui = new BoundingBoxUI[bounding_box.Length];


        for (int i = 0; i < bounding_box.Length; i++)
        {
            bounding_box[i] = GameObject.Instantiate(bounding_box_prefab);
            bounding_box[i].SetParent(bounding_box_parent.transform);
            bounding_box[i].gameObject.name = " bounding_box";
            bounding_box_ui[i] = bounding_box[i].gameObject.GetComponent<BoundingBoxUI>();
        }

    }

    /// <summary>
    /// Set the maximum number of hands that can be simultaneously detected by Manomotion Manager based on the licence.
    /// This process is based on your Licence privilliges.
    /// </summary>
    void SetHandsSupportedByLicence()
    {
        handsSupportedByLicence = 1;

    }


    /// <summary>
    /// Initializes the values of contour points, inner points,bounding boxes (formed by Line Renderers), fingertip points and fingertip label points
    /// </summary>
    void SetPointsBasedOnHandNumber()
    {
        contour_particles = new Transform[handsSupportedByLicence][];
        inner_particles = new Transform[handsSupportedByLicence][];
        contour_line_renderer = new LineRenderer[handsSupportedByLicence];
        fingertips_t = new Transform[handsSupportedByLicence][];
        fingertip_labels = new Transform[handsSupportedByLicence][];
    }

    /// <summary>
    /// Creates the particle point for the palmcenter in order to be used in the dots hand representation
    /// </summary>
    void CreatePalmCenterParticle()
    {
        palmcenter_parent = (new GameObject());
        palmcenter_parent.name = "palmcenter parent";
        palmcenter_parent.transform.SetParent(transform);
        palm = new Transform[handsSupportedByLicence];
        for (int i = 0; i < palm.Length; i++)
        {
            palm[i] = GameObject.Instantiate(inner_dots_prefab);
            palm[i].SetParent(palmcenter_parent.transform);
            palm[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    /// <summary>
    /// Creates the particles for the fingertips points in order to be used in the dots hand representation
    /// </summary>
    void CreateFingerTipParticles()
    {

        fingertip_parent = (new GameObject());
        fingertip_parent.name = "fingertip parent";
        fingertip_parent.transform.SetParent(transform);

        for (int i = 0; i < fingertips_t.Length; i++)
        {
            fingertips_t[i] = new Transform[5];
            for (int j = 0; j < fingertips_t[i].Length; j++)
            {
                Transform pointer = GameObject.Instantiate(inner_dots_prefab);
                pointer.localScale = pointer.localScale * UnityEngine.Random.Range(2.5f, 3.5f);
                pointer.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                pointer.SetParent(fingertip_parent.transform);
                fingertips_t[i][j] = pointer;
            }

        }
    }

    /// <summary>
    /// Creates the particles for the fingertips points in order to display their name (e.x thumb,index,middle,ring,pinky)
    /// </summary>
	void CreateFingerTipLabelParticles()
    {

        fingertip_label_parent = (new GameObject());
        fingertip_label_parent.name = "fingertip label parent";
        fingertip_label_parent.transform.SetParent(transform);
        for (int i = 0; i < fingertip_labels.Length; i++)
        {
            fingertip_labels[i] = new Transform[5];
            for (int j = 0; j < fingertip_labels[i].Length; j++)
            {
                Transform pointer = GameObject.Instantiate(inner_dots_prefab);
                pointer.localScale = pointer.localScale * UnityEngine.Random.Range(2.5f, 3.5f);
                pointer.gameObject.GetComponent<MeshRenderer>().enabled = false;
                pointer.SetParent(fingertip_label_parent.transform);
                TextMesh label = pointer.GetComponentInChildren<TextMesh>();
                label.text = finger_labels[j];
                fingertip_labels[i][j] = pointer;
            }
        }

    }

    /// <summary>
    /// Creates the particles for the contour/outer points in order to be used in the dots hand representation
    /// </summary>
    void CreateContourParticles()
    {
        contour_parent = (new GameObject());
        contour_parent.name = "Contour parent";
        contour_parent.transform.SetParent(transform);
        LineRenderer base_line_renderer = GetComponent<LineRenderer>();

        for (int i = 0; i < handsSupportedByLicence; i++)
        {
            contour_line_renderer[i] = new GameObject().AddComponent<LineRenderer>();
            contour_line_renderer[i].startWidth = base_line_renderer.startWidth;
            contour_line_renderer[i].endWidth = base_line_renderer.endWidth;
            contour_line_renderer[i].material = base_line_renderer.material;
            contour_line_renderer[i].transform.SetParent(Camera.main.transform);

        }

        for (int i = 0; i < contour_particles.Length; i++)
        {
            contour_particles[i] = new Transform[100];
            for (int j = 0; j < contour_particles[i].Length; j++)
            {
                contour_particles[i][j] = GameObject.Instantiate(outer_dots_prefab);
                contour_particles[i][j].localScale = contour_particles[i][j].localScale * UnityEngine.Random.Range(0.5f, 1.5f);
                contour_particles[i][j].gameObject.GetComponent<MeshRenderer>().material.color = _outlinePointsColor;
                contour_particles[i][j].SetParent(contour_parent.transform);
            }
        }
    }

    /// <summary>
    /// Creates the particles for the inner points of the hand in order to be used in the dots hand representation
    /// </summary>
    void CreateInnerParticles()
    {
        inner_parent = (new GameObject());
        inner_parent.name = "inner parent";
        inner_parent.transform.SetParent(transform);
        for (int i = 0; i < inner_particles.Length; i++)
        {
            inner_particles[i] = new Transform[100];
            for (int j = 0; j < inner_particles[i].Length; j++)
            {
                inner_particles[i][j] = GameObject.Instantiate(inner_dots_prefab);
                inner_particles[i][j].gameObject.GetComponent<MeshRenderer>().material.color = _innerPointsColor;
                inner_particles[i][j].localScale = inner_particles[i][j].localScale * UnityEngine.Random.Range(0.5f, 1f);
                inner_particles[i][j].SetParent(inner_parent.transform);
            }
        }
    }

    #endregion

    void Update()
    {

        for (int handIndex = 0; handIndex < handsSupportedByLicence; handIndex++)
        {

            handsSupportedByLicence = ManomotionManager.Instance.Manomotion_Session.is_two_hands_enabled_by_developer + 1;
            ShowFingerTips(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.tracking_info, handIndex);
            ShowFingerTipLabels(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.tracking_info, handIndex);
            ShowPalmCenter(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.tracking_info, handIndex);
            ShowContour(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.tracking_info, handIndex);
            ShowInnerPoints(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.tracking_info, handIndex);
            UpdateLineColor(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.gesture_info, handIndex);
            UpdatePalmColor(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.gesture_info, handIndex);
            ShowLayering(ManomotionManager.Instance.Hand_infos[handIndex], _layer_hands[handIndex], ManomotionManager.Instance);
            ShowBoundingBoxInfo(ManomotionManager.Instance.Hand_infos[handIndex].hand_info.tracking_info, handIndex);
        }
        if (_layer_background)
        {
            ShowBackground(ManomotionManager.Instance.Visualization_info.rgb_image, _layer_background);
        }

    }

    /// <summary>
    /// Updates the color of the line renderer, that connects the outer/contour points, according to the gesture performed.
    /// </summary>
    /// <param name="gesture_info">Requires the gesture information from the detected hand</param>
    /// <param name="index">Requires the gesture information from the detected hand</param>
    void UpdateLineColor(GestureInfo gesture_info, int index)
    {
        ManoGestureTrigger trigger = gesture_info.mano_gesture_trigger;
        switch (trigger)
        {
            case ManoGestureTrigger.NO_GESTURE:
                break;
            case ManoGestureTrigger.CLICK:
                HighLightWithColor(_clickLineColor, index);
                break;
            case ManoGestureTrigger.SWIPE_LEFT:
                break;
            case ManoGestureTrigger.SWIPE_RIGHT:
                break;
            case ManoGestureTrigger.GRAB:
                HighLightWithColor(_grabLineColor, index);
                break;
            case ManoGestureTrigger.TAP_POINTING:
                HighLightWithColor(_tapLineColor, index);
                break;
            case ManoGestureTrigger.DROP:
                HighLightWithColor(_releasePinchLineColor, index);
                break;
            case ManoGestureTrigger.PICK:
                HighLightWithColor(_grabPinchLineColor, index);
                break;
            case ManoGestureTrigger.RELEASE:
                HighLightWithColor(_releaseLineColor, index);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Updates the color of the palm center particle according to the gesture performed.
    /// </summary>
    /// <param name="gesture_info">Requires the gesture information from the detected hand</param>
    void UpdatePalmColor(GestureInfo gesture_info, int index)
    {
        ManoGestureContinuous continous = gesture_info.mano_gesture_continuous;
        switch (continous)
        {
            case ManoGestureContinuous.NO_GESTURE:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _baseColor;
                break;
            case ManoGestureContinuous.HOLD_GESTURE:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _holdColor;
                break;
            case ManoGestureContinuous.OPEN_HAND_GESTURE:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _openHandColor;
                break;
            case ManoGestureContinuous.OPEN_PINCH_GESTURE:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _pinchColor;
                break;
            case ManoGestureContinuous.CLOSED_HAND_GESTURE:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _closedHandColor;
                break;
            case ManoGestureContinuous.POINTER_GESTURE:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _pointerColor;
                break;
            case ManoGestureContinuous.PUSH_POINTING_GESTURE:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _pushColor;
                break;
            default:
                palm[index].gameObject.GetComponent<MeshRenderer>().material.color = _baseColor;
                break;
        }
    }

    /// <summary>
    /// Displays the point aligned in the palm center of the detected hand
    /// </summary>
    /// <param name="tracking_info">Requires tracking information of the hand</param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    void ShowPalmCenter(TrackingInfo tracking_info, int index)
    {
        palmcenter_parent.SetActive(_show_palm_center);
        if (_show_palm_center)
        {
            palm[index].localPosition = Vector3.one;
            Vector3 palm_cent = tracking_info.palm_center;
            float depth = tracking_info.relative_depth;
            ManoUtils.Instance.CalculateNewPosition(palm_cent, depth);
            //PositionText.text = (palm_cent.ToString());  //RK
            layering_object.gameObject.transform.position = (3.0f*(palm_cent) + new Vector3(-1.5f,0.0f,-9.0f)); //RK
            //PositionText.text = (layering_object.transform.position.ToString());  //RK
        }
    }

    /// <summary>
    /// Shows the bounding box info.
    /// </summary>
    /// <param name="tracking_info">Requires tracking information of the hand</param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    private void ShowBoundingBoxInfo(TrackingInfo tracking_info, int index)
    {

        bounding_box_ui[index].gameObject.SetActive(Show_bounding_box && ManomotionManager.Instance.Hand_infos[0].hand_info.warning != Warning.WARNING_HAND_NOT_FOUND);
        if (Show_bounding_box)
        {
            bounding_box_ui[index].UpdateInfo(tracking_info.bounding_box);
        }

    }

    /// <summary>
    /// Displays the fingertip points as detected from the hand
    /// </summary>
    /// <param name="tracking_info">Requires tracking information of the hand</param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    private void ShowFingerTips(TrackingInfo tracking_info, int index)
    {
        fingertip_parent.SetActive(_show_fingertips && ManomotionManager.Instance.Hand_infos[0].hand_info.warning != Warning.WARNING_HAND_NOT_FOUND);
        if (_show_fingertips)
        {
            for (int i = 0; i < 5; i++)
            {

                if (tracking_info.finger_tips.Length > i && tracking_info.finger_tips[i].x >= 0)
                {
                    fingertips_t[index][i].gameObject.SetActive(true);
                    Vector3 newPos = mano_utils.CalculateNewPosition(tracking_info.finger_tips[i], tracking_info.relative_depth - 0.1f);
                    fingertips_t[index][i].localPosition = Vector3.Lerp(fingertips_t[index][i].position, newPos, .8f);
                    fingertips_t[index][i].localRotation = Quaternion.identity;
                }
                else
                    fingertips_t[index][i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Displays the labels(finger type) of the detected fingers
    /// </summary>
    /// <param name="tracking_info">Requires tracking information of the hand</param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    private void ShowFingerTipLabels(TrackingInfo tracking_info, int index)
    {
        fingertip_label_parent.SetActive(_show_fingertip_labels && ManomotionManager.Instance.Hand_infos[0].hand_info.warning != Warning.WARNING_HAND_NOT_FOUND);
        if (_show_fingertip_labels)
        {

            for (int i = 0; i < 5; i++)
            {
                if (tracking_info.finger_tips[i].x >= 0)
                {
                    fingertip_labels[index][i].gameObject.SetActive(true);
                    Vector3 newPos = mano_utils.CalculateNewPosition(tracking_info.finger_tips[i], tracking_info.relative_depth);
                    fingertip_labels[index][i].localPosition = Vector3.Lerp(fingertip_labels[index][i].position, newPos, .8f);
                    fingertip_labels[index][i].transform.localRotation = cam.transform.localRotation;
                }
                else
                    fingertip_labels[index][i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Displays the contour/outer points of the detected hand
    /// </summary>
    /// <param name="tracking_info">Requires tracking information of the hand</param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    private void ShowContour(TrackingInfo tracking_info, int index)
    {

        contour_parent.SetActive(Show_contour && ManomotionManager.Instance.Hand_infos[0].hand_info.warning != Warning.WARNING_HAND_NOT_FOUND);

        contour_line_renderer[index].enabled = Show_contour;
        if (Show_contour)
        {
            if (tracking_info.contour_points != null)
            {
                contour_line_renderer[index].positionCount = tracking_info.amount_of_contour_points + 1; // +1 so that it loops 

                for (int i = 0; i < contour_particles[index].Length; i++)
                {

                    if (i < tracking_info.amount_of_contour_points)
                    {
                        contour_particles[index][i].gameObject.SetActive(true);
                        Vector3 newPos = mano_utils.CalculateNewPosition(tracking_info.contour_points[i], tracking_info.relative_depth - .1f);

                        contour_particles[index][i].localPosition = newPos;
                        contour_line_renderer[index].SetPosition(i, contour_particles[index][i].localPosition);
                    }
                    else
                    {

                        contour_particles[index][i].gameObject.SetActive(false);
                    }

                }
                contour_line_renderer[index].SetPosition(tracking_info.amount_of_contour_points, contour_particles[index][0].localPosition);

            }
        }
    }

    /// <summary>
    /// Displays points inside the area of the detected hand
    /// </summary>
    /// <param name="tracking_info">Requires tracking information of the hand</param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    private void ShowInnerPoints(TrackingInfo tracking_info, int index)
    {
        inner_parent.SetActive(_show_inner && ManomotionManager.Instance.Hand_infos[0].hand_info.warning != Warning.WARNING_HAND_NOT_FOUND);
        if (_show_inner)
        {
            for (int point = 0; point < inner_particles[index].Length; point++)
            {
                if (tracking_info.inner_points != null)
                {
                    if (point < tracking_info.amount_of_inner_points)
                    {
                        inner_particles[index][point].gameObject.SetActive(true);
                        inner_particles[index][point].localPosition = mano_utils.CalculateNewPosition(tracking_info.inner_points[point], tracking_info.relative_depth - 0.1f);

                    }
                    else
                    {
                        inner_particles[index][point].gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("tracking info inner points are null");
                }
            }
        }

    }

    /// <summary>
    /// Sets the color of the linerender that connects the contour points to a given color in order to achieve the highlight effect.
    /// </summary>
    /// <param name="color">Requires a color</param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    private void HighLightWithColor(Color color, int index)
    {
        contour_line_renderer[index].startColor = color;
        contour_line_renderer[index].endColor = color;
        StartCoroutine(FadeColorAfterDelay(.01f, index));
    }

    /// <summary>
    ///   Displays the hand layer that can be on top or behind the virtual objects
    /// </summary>
    /// <param name="hand_info">Hand information provided by Manomotion Manager class</param>
    /// <param name="layer_hands">Reference to the meshrenderer of the hands</param>
    /// <param name="mano_manager">Reference to the Manomotion Manager class</param>
    void ShowLayering(HandInfoUnity hand_info, MeshRenderer layer_hands, ManomotionManager mano_manager)
    {

        layer_hands.gameObject.SetActive(_show_hand_layer);
        layering_object.SetActive(_show_hand_layer);
        if (_show_hand_layer)
        {

            ManoUtils.Instance.OrientMeshRenderer(layer_hands);

            //Assign the texture
            layer_hands.material.mainTexture = hand_info.cut_rgb;

            //Move forward according to depth
            layer_hands.transform.localPosition = Vector3.forward * hand_info.hand_info.tracking_info.relative_depth;

            //Adjust the size in order to fit the screen
            mano_utils.AjustBorders(layer_hands, ManomotionManager.Instance.Manomotion_Session);
        }
    }

    /// <summary>
    /// Projects the texture received from the camera as the background
    /// </summary>
    /// <param name="backgroundTexture">Requires the texture captured from the camera</param>
    /// <param name="backgroundMeshRenderer">Requires the MeshRenderer that the texture will be displayed</param>
    void ShowBackground(Texture2D backgroundTexture, MeshRenderer backgroundMeshRenderer)
    {
        // local is always 0 0 1
        // Debug.LogFormat("Background local position {0}  rotation{1}", backgroundMeshRenderer.transform.localPosition, backgroundMeshRenderer.transform.localRotation);
        // Debug.LogFormat("Background base position {0}  rotation{1}", backgroundMeshRenderer.transform.position, backgroundMeshRenderer.transform.rotation);

        backgroundMeshRenderer.gameObject.SetActive(Show_background_layer);
        if (Show_background_layer)
        {
            //Take the orientation
            ManoUtils.Instance.OrientMeshRenderer(backgroundMeshRenderer);
            //content
            backgroundMeshRenderer.material.mainTexture = backgroundTexture;
            //Adjust the size in order to fit the screen
            mano_utils.AjustBorders(backgroundMeshRenderer, ManomotionManager.Instance.Manomotion_Session);
        }

    }

    /// <summary>
    /// Gradually fades the color of the line renderer that is being used to connect the outer/contour points.
    /// </summary>
    /// <param name="delay">Requires duration float value to wait before executing </param>
    /// <param name="index">Requires the int index value that refers to a given hand from the array of hands </param>
    IEnumerator FadeColorAfterDelay(float delay, int index)
    {
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(delay);

            float r = contour_line_renderer[index].startColor.r + .02f;
            float g = contour_line_renderer[index].startColor.g + .02f;
            float b = contour_line_renderer[index].startColor.b + .02f;
            float a = Math.Max(1f, contour_line_renderer[index].startColor.a - .02f);
            Color newColor = new Color(r, g, b, a);
            contour_line_renderer[index].startColor = newColor;
            contour_line_renderer[index].endColor = newColor;
            if (r >= 1 && g >= 1 && b >= 1 && a <= 0.1f)
                break;
        }
    }

    /// <summary>
    /// Toggles the visibility of the given gameobject
    /// </summary>
    /// <param name="givenObject">Requires a gameObject that will change layers</param>
    private void ToggleObjectVisibility(GameObject givenObject)
    {
        givenObject.SetActive(!givenObject.activeInHierarchy);
    }

    /// <summary>
	/// Creates the meshes needed by the different Manomotion Layers and also parents them to the scene's Main Camera
	/// </summary>
    private void InstantiateManomotionMeshes()
    {

        _layer_hands = new MeshRenderer[handsSupportedByLicence];
        for (int i = 0; i < handsSupportedByLicence; i++)
        {
            GameObject hand = Instantiate(manomotionGenericLayer);
            hand.transform.name = "Hand" + i;
            hand.transform.SetParent(cam.transform);
            hand.transform.localPosition = new Vector3(0, 0, 0.5f);
            _layer_hands[i] = hand.GetComponent<MeshRenderer>();
        }

        GameObject background = Instantiate(manomotionGenericLayer);
        background.transform.name = "Background";
        background.transform.SetParent(cam.transform);



        _layer_background = background.GetComponent<MeshRenderer>();
        _layer_background.transform.localPosition = new Vector3(0, 0, 5);
        _layer_background.transform.localRotation = Quaternion.identity;
    }

    [SerializeField]
    ManomotionUIManagment manomotionUI;



    /// <summary>
    /// Toggles the support for one or two hands.
    /// </summary>
    public void ToggleTwoHandSupport()
    {
        if (ManomotionManager.Instance.Manomotion_Session.is_two_hands_enabled_by_developer == 1)
        {
            ManomotionManager.Instance.Set2HandSupport(0);
            manomotionUI.ShowHandButtons();
        }
        else if (ManomotionManager.Instance.Manomotion_Session.is_two_hands_enabled_by_developer == 0)
        {
            ManomotionManager.Instance.Set2HandSupport(1);
            manomotionUI.HideHandButtons();
        }
    }
}
