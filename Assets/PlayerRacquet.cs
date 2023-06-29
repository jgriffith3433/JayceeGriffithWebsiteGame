using UnityEngine;
using System.Collections;
using GNet;


[RequireComponent(typeof(TNObject))]
public class PlayerRacquet : MonoBehaviour
{
	[System.NonSerialized]
	public TNObject tno;

	[Range(1f, 20f)]
	public float posUpdates = 10f;

	public Rigidbody m_Rigidbody = null;

	public Rigidbody RB { get { return m_Rigidbody; } }

	private Vector3 m_PreviousPosition;

	private Vector3 m_DecayingVelocity;

	public Vector3 DecayingVelocity { get { return m_DecayingVelocity; } }

    [SerializeField] private Vector3 m_HitBallDirection = new Vector3(0, 0, 1);

	[SerializeField] private float m_RacquetVelocityInfluence = 0.25f;

    public float m_RacquetDistanceFromCamera = 2f;

	protected Vector3 mPosLast;
	protected Quaternion mRotLast;
	protected float mPosLastSend = 0f;
    
    public int PlayerNumber = -1;
    public bool IsSpectating = false;
    private TableTennis m_TableTennis = null;
	private Vector3 m_MousePositionInWorld;

	private void Awake ()
	{
		tno = GetComponent<TNObject>();
	}

    private IEnumerator Start()
    {
        while(m_TableTennis == null)
        {
            var tts = GameObject.FindObjectsOfType<TableTennis>();
            if (tts.Length > 0)
            {
                m_TableTennis = tts[0];
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        m_TableTennis.AddPlayerRacquet(this);
    }
		Vector3 vel = Vector3.zero;

	private void Update ()
	{
		if (!tno.isMine) return;
		if (tno.hasBeenDestroyed) return;
        if (IsSpectating) return;
		if (m_PreviousPosition == Vector3.zero)
		{
			m_PreviousPosition = transform.position;
		}
		var cVel = (transform.position - m_PreviousPosition) / Time.deltaTime;
		if (cVel.magnitude > m_DecayingVelocity.magnitude)
		{
			m_DecayingVelocity = cVel;
		}
		else
		{
			var magDelta = m_DecayingVelocity.magnitude - cVel.magnitude;
			if (magDelta <= 1f)
			{
				m_DecayingVelocity = cVel;
			}
			else
			{
				m_DecayingVelocity = Vector3.SmoothDamp(m_DecayingVelocity, cVel, ref vel, 1f / magDelta);
			}
		}
		float time = Time.time;
		float posDelta = time - mPosLastSend;
		float posDelay = 1f / posUpdates;

		// Don't send updates more than 20 times per second
		if (posDelta > 0.05f)
		{
			//TODO: Look into this more
			//float threshold = Mathf.Clamp01(posDelta - posDelay) * 0.5f;

			// If the deviation is significant enough, send the update to other players
			// if (Tools.IsNotEqual(mPosLast.x, newPos.x, threshold) ||
			// 	Tools.IsNotEqual(mPosLast.y, newPos.y, threshold))
			{
				mPosLastSend = time;
				mPosLast = transform.position;
				mRotLast = transform.rotation;
				tno.Send("SetPosRot", ForwardType.Others, transform.position, transform.rotation);
			}
		}
    	m_PreviousPosition = transform.position;
	}

	private void FixedUpdate()
	{
        if (IsSpectating) return;
		if (!tno.isMine) return;
		if (tno.hasBeenDestroyed) return;
		if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Began)))
		{
			m_MousePositionInWorld = Camera.main.ScreenToWorldPoint(new Vector3(TouchHandler.screenPos.x, TouchHandler.screenPos.y, m_RacquetDistanceFromCamera));
			transform.position = m_MousePositionInWorld;
		}
	}

	[RFC]
	protected void SetPosRot (Vector3 newPos, Quaternion rot)
	{
        transform.position = newPos;
		transform.rotation = rot;
	}

	public void HitBall(Ball ball, Vector3 playerRacquetVelocity)
	{
		var otherPlayerRacquet = m_TableTennis.GetPlayerRacquetByPlayerNumber(PlayerNumber == 1 ? 2 : 1);
		var mirroredPosition = new Vector3(transform.position.x, transform.position.y, otherPlayerRacquet.transform.position.z);
		var goalVector = mirroredPosition - transform.position;
		var goalPosition = goalVector.normalized * (goalVector.magnitude * 0.5f);
        var velocityToGoal = CalculateBallisticVelocityVector(transform.position, goalPosition, 30f);
		var newVelocity = velocityToGoal + (playerRacquetVelocity * m_RacquetVelocityInfluence);
		ball.RB.velocity = newVelocity;
	}

    private Vector3 CalculateBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
         Vector3 direction = target - source;
         float h = direction.y;
         direction.y = 0;
         float distance = direction.magnitude;
         float a = angle * Mathf.Deg2Rad;
         direction.y = distance * Mathf.Tan(a);
         distance += h/Mathf.Tan(a);

         float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2*a));
         return velocity * direction.normalized;
     }
}
