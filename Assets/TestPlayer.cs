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
    [SerializeField] private CharacterMeleeDemo m_Character = null;
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

    private Sidescroller m_SideScroller = null;

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
    private Rigidbody[] m_Rigidbodies = null;
    private void Awake()
    {
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();
        tno = GetComponent<TNObject>();
        m_PosLastSent = new Vector3[m_Ts.Length];
        m_RotLastSent = new Quaternion[m_Ts.Length];
    }

    
    private IEnumerator Start()
    {
        var player = TNManager.GetPlayer(tno.ownerID);
        if (player != null)
        {
            m_PlayerNameText.text = player.name;
        }
        while(m_SideScroller == null)
        {
            var ss = GameObject.FindObjectsOfType<Sidescroller>();
            if (ss.Length > 0)
            {
                m_SideScroller = ss[0];
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        if (tno.isMine)
        {
            m_SideScroller.SetPlayer(this);
        }
    }

    public void Teleport(Vector3 position)
    {
        foreach (var rb in m_Rigidbodies)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
        m_PuppetMaster.Teleport(position, transform.rotation, false);
        m_GravPoint.transform.position = position;
        mNextT = 0;
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
        tno.Send("SetInputState", ForwardType.OthersSaved, m_UserControlAI.state.walk, m_UserControlAI.state.crouch, m_UserControlAI.state.jump, m_UserControlAI.state.actionIndex);
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
            //m_PuppetBehavior.enabled = true;
        }
        else
        {
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