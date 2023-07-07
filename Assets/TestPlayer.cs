using GNet;
using MHLab.ReactUI.Core;
using RootMotion.Demos;
using UnityEngine;
using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private PuppetMaster m_PuppetMaster = null;
    [SerializeField] private CharacterThirdPerson m_Character = null;
    [SerializeField] private GravPoint m_GravPoint = null;
    [SerializeField] private GravDraggedObject m_GravDraggedObject = null;
    [SerializeField] private UserControlAI m_UserControlAI = null;
    [SerializeField] private Transform[] m_Ts = null;
    [SerializeField] private TextMesh m_PlayerNameText = null;

    private Vector3[] m_PosLastSent = null;
    private Quaternion[] m_RotLastSent = null;

    /// <summary>
    /// Maximum number of updates per second when synchronizing the rigidbody.
    /// </summary>

    [Range(0.25f, 5f)]
    public float tUpdates = 1f;
    
    public PuppetMaster.StateSettings stateSettings = PuppetMaster.StateSettings.Default;

    /// <summary>
    /// We want to cache the network object (TNObject) we'll use for network communication.
    /// If the script was derived from TNBehaviour, this wouldn't have been necessary.
    /// </summary>

    [System.NonSerialized]
    public TNObject tno;

    private Coroutine m_GetUpCoroutine = null;

    protected float mNextState = 0f;
    protected float mNextT = 0f;
    public BehaviourFall m_FallBehavior = null;
    public BehaviourPuppet m_PuppetBehavior = null;
    public GravPoint GravPoint
    {
        get { return m_GravPoint; }
    }

    public GravDraggedObject GravDraggedObject
    {
        get { return m_GravDraggedObject; }
    }

    public UserControlAI UserControlAI
    {
        get { return m_UserControlAI; }
    }

    public PuppetMaster PuppetMaster
    {
        get { return m_PuppetMaster; }
    }

    public CharacterThirdPerson Character
    {
        get { return m_Character; }
    }
    private Rigidbody[] m_Rigidbodies = null;
    private void Awake()
    {
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();
        tno = GetComponent<TNObject>();
        m_PosLastSent = new Vector3[m_Ts.Length];
        m_RotLastSent = new Quaternion[m_Ts.Length];
    }

    public void Ragdoll()
    {
        m_PuppetMaster.state = PuppetMaster.State.Dead;
        m_FallBehavior.enabled = true;
        m_Character.enabled = false;
        if (m_GetUpCoroutine != null)
        {
            StopCoroutine(m_GetUpCoroutine);
            m_GetUpCoroutine = null;
        }
        m_GetUpCoroutine = StartCoroutine(GetUp());
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(3f);
        m_PuppetBehavior.enabled = true;
        m_Character.enabled = true;
        m_PuppetMaster.state = PuppetMaster.State.Alive;
        m_GetUpCoroutine = null;
    }

    private void Start()
    {
        var player = TNManager.GetPlayer(tno.ownerID);
        if (player != null)
        {
            m_PlayerNameText.text = player.name;
        }
    }

    public void Teleport(Vector3 position)
    {
        foreach (var rb in m_Rigidbodies)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
        m_PuppetMaster.mode = PuppetMaster.Mode.Disabled;
        m_PuppetMaster.Teleport(position, transform.rotation, false);
        m_GravPoint.transform.position = position;
        mNextT = 0;
        m_PuppetMaster.mode = PuppetMaster.Mode.Active;
    }

    private void OnEnable()
    {
        m_UserControlAI.ChangedInputState += OnChangedInputState;
    }

    private void OnDisable()
    {
        m_UserControlAI.ChangedInputState -= OnChangedInputState;
    }

    private void OnChangedInputState()
    {
        tno.Send("SetInputState", ForwardType.AllSaved, m_UserControlAI.state.walk, m_UserControlAI.state.crouch, m_UserControlAI.state.jump, m_UserControlAI.state.actionIndex);
    }

    /// <summary>
    /// Only the car's owner should be updating the movement axes, and the result should be sync'd with other players.
    /// </summary>

    protected void Update()
    {
        m_UserControlAI.canControl = tno.isMine;
        if (!tno.isMine)
        {
            return;
        }
        if (m_Character.onGround)
        {
            //m_PuppetMaster.state = PuppetMaster.State.Alive;
            //m_FallBehavior.enabled = false;
        }
        else
        {
            //m_PuppetMaster.state = PuppetMaster.State.Dead;
            //m_FallBehavior.enabled = true;
        }
        if (m_UserControlAI.transform.position.y < -20)
        {
            Teleport(new Vector3(transform.position.x, 10, 0));
        }
        float time = Time.time;
        // Since the input is sent frequently, rigidbody only needs to be corrected every couple of seconds.
        // Faster-paced games will require more frequent updates.
        if (mNextT < time)
        {
            mNextT = time + 1f / tUpdates;
            for (var i = 0; i < m_Ts.Length; i++)
            {
                if (m_PosLastSent[i] != m_Ts[i].position || m_RotLastSent[i] != m_Ts[i].rotation)
                {
                    m_PosLastSent[i] = m_Ts[i].position;
                    m_RotLastSent[i] = m_Ts[i].rotation;
                    tno.Send("SetT", ForwardType.OthersSaved, m_Ts[i].position, m_Ts[i].rotation, i);
                }
            }
        }
    }

    /// <summary>
    /// RFC for the rigidbody will be called once per second by default.
    /// </summary>

    [RFC]
    protected void SetT(Vector3 pos, Quaternion rot, int index)
    {
        m_Ts[index].position = pos;
        m_Ts[index].rotation = rot;
    }

    [RFC]
    protected void SetInputState(bool walk, bool crouch, bool jump, int actionIndex)
    {
        //Debug.Log("New state: " + walk + " " + crouch + " " + jump + " " + actionIndex);
        m_UserControlAI.state.walk = walk;
        m_UserControlAI.state.crouch = crouch;
        m_UserControlAI.state.jump = jump;
        m_UserControlAI.state.actionIndex = actionIndex;
    }

    public void SendChangeSlowlyMoveTowardsTarget(bool value)
    {
        tno.Send("ChangeSlowlyMoveTowardsTarget", ForwardType.OthersSaved, value);
    }

    [RFC]
    protected void ChangeSlowlyMoveTowardsTarget(bool value)
    {
        m_GravPoint.ChangeSlowlyMoveTowardsTarget(value);
    }
}