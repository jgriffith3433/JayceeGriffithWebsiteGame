using UnityEngine;
using System.Collections;
using GNet;


[RequireComponent(typeof(TNObject))]
public class Ball : MonoBehaviour
{
	[System.NonSerialized]
	public TNObject tno;

	public Rigidbody m_Rigidbody = null;

	public Rigidbody RB { get { return m_Rigidbody; } }

    public Vector3 PredictedPoint1;
    public Vector3 PredictedPoint2;
    public Vector3 PredictedPoint3;
	
    private TableTennis m_TableTennis = null;
    
    private Vector3 m_PreviousPosition;

    [SerializeField] private float m_Prediction1Time = 5f;
    [SerializeField] private float m_Prediction2Time = 10f;
    [SerializeField] private float m_Prediction3Time = 15f;
    [SerializeField] private TNSyncRigidbody m_TNSyncRigidbody = null;

    public PlayerRacquet LastPlayerRacquetToHitBall = null;
    private float m_TimeLastHitRacquet = 0f;


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
        m_TableTennis.AddBall(this);
    }

    private void FixedUpdate()
    {
        PredictedPoint1 = transform.position + m_Rigidbody.velocity * Time.deltaTime * m_Prediction1Time;
        PredictedPoint2 = transform.position + m_Rigidbody.velocity * Time.deltaTime * m_Prediction2Time;
        PredictedPoint3 = transform.position + m_Rigidbody.velocity * Time.deltaTime * m_Prediction3Time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("Racquet"))
        {
            //only allow hitting the ball 2 times per second
            if (Time.time - m_TimeLastHitRacquet >= 0.5f)
            {
                m_TimeLastHitRacquet = Time.time;
                var playerRacquet = collision.collider.gameObject.GetComponentInParent<PlayerRacquet>();
                if (playerRacquet != null && playerRacquet.tno.isMine)
                {
                    tno.Send("SyncRacquetAndHitBall", ForwardType.All, playerRacquet.tno.owner.id, playerRacquet.transform.position, playerRacquet.DecayingVelocity);
                }
            }
        }
    }

    [RFC]
    private void SyncRacquetAndHitBall(int playerId, Vector3 playerRacquetPosition, Vector3 playerRacquetVelocity)
    {
		var playerRacquet = m_TableTennis.GetPlayerRacquetById(playerId);
        if(playerRacquet != null)
        {
            playerRacquet.transform.position = playerRacquetPosition;
            playerRacquet.HitBall(this, playerRacquetVelocity);
            LastPlayerRacquetToHitBall = playerRacquet;
        }
    }
}
