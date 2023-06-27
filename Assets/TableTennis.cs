using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GNet;
using System.Linq;

[RequireComponent(typeof(TNObject))]
public class TableTennis : MonoBehaviour
{
	[System.NonSerialized]
	public TNObject tno;
    public Ball Ball = null;
    public bool Serving = false;

    [SerializeField] private Vector3 m_ServingImpulseVector = new Vector3(0, 0, 1);
    [SerializeField] private int m_WaitTimeBeforeServing = 3;
    [SerializeField] private Transform m_GroundHelper = null;
    [SerializeField] private Transform m_BallSpawnPositionHelper = null;
    [SerializeField] private Transform m_Player1PositionHelper = null;
    [SerializeField] private Transform m_Player1RacquetPositionHelper = null;
    [SerializeField] private Transform m_Player2PositionHelper = null;
    [SerializeField] private Transform m_Player2RacquetPositionHelper = null;
    [SerializeField] private Transform m_SpectatorPositionHelper = null;
    [SerializeField] private Transform m_SpectatorRacquetPositionHelper = null;
    [SerializeField] private float m_MaxTimeBallReset = 3f;
    [SerializeField] private float m_MinBallVelocity = 0.4f;
    [SerializeField] private TableTennisClient m_TableTennisClient = null;
    [SerializeField] private GameObject[] m_PredictedPointHelpers = null;
    [SerializeField] private List<PlayerRacquet> m_PlayerRacquets = new List<PlayerRacquet>();

    private float m_TimeBallIsStationary = 0f;
    private PlayerRacquet m_LocalPlayerRacquet = null;
    private PlayerRacquet m_PlayerRacquetWithBall = null;

    private void Awake()
    {
		tno = GetComponent<TNObject>();
    }
    
	[RCC]
	private static GameObject CreatePlayerRacquet(GameObject prefab, int playerId)
    {
		var p = prefab;
		GameObject go = p.Instantiate();
		go.name = "Player_" + playerId;
		return go;
    }

    public void AddPlayerRacquet(PlayerRacquet newPlayerRacquet)
    {
        var playerRacquet = m_PlayerRacquets.FirstOrDefault(p => p.tno.owner.id == newPlayerRacquet.tno.owner.id);
        if (playerRacquet != null)
        {
            Debug.Log("Found existing racquet for playerid: " + newPlayerRacquet.tno.owner.id + ". Destroying old racquet..");
            m_PlayerRacquets.Remove(playerRacquet);
            playerRacquet.tno.DestroySelf();
        }
        m_PlayerRacquets.Add(newPlayerRacquet);
        UpdateCurrentPlayers();
        if (newPlayerRacquet.tno.isMine)
        {
            m_LocalPlayerRacquet = newPlayerRacquet;
            UpdateLocalPlayerRacquetTransform();
        }
    }

    public void RemovePlayerRacquet(Player player)
    {
        var playerRacquet = m_PlayerRacquets.FirstOrDefault(p => p.tno.owner.id == player.id);
        if (playerRacquet != null)
        {
            m_PlayerRacquets.Remove(playerRacquet);
            playerRacquet.tno.DestroySelf();
            UpdateCurrentPlayers();
            UpdateLocalPlayerRacquetTransform();
        }
        m_PlayerRacquets = m_PlayerRacquets.Where(p => p != null).ToList();
    }

    private void UpdateLocalPlayerRacquetTransform()
    {
        if (m_LocalPlayerRacquet.PlayerNumber == 1)
        {
            m_LocalPlayerRacquet.transform.position = m_Player1RacquetPositionHelper.position;
            m_LocalPlayerRacquet.transform.rotation = m_Player1RacquetPositionHelper.rotation;
            Camera.main.transform.position = m_Player1PositionHelper.position;
            Camera.main.transform.rotation = m_Player1PositionHelper.rotation;
        }
        else if (m_LocalPlayerRacquet.PlayerNumber == 2)
        {
            m_LocalPlayerRacquet.transform.position = m_Player2RacquetPositionHelper.position;
            m_LocalPlayerRacquet.transform.rotation = m_Player2RacquetPositionHelper.rotation;
            Camera.main.transform.position = m_Player2PositionHelper.position;
            Camera.main.transform.rotation = m_Player2PositionHelper.rotation;
        }
        else
        {
            m_LocalPlayerRacquet.transform.position = m_SpectatorRacquetPositionHelper.position;
            m_LocalPlayerRacquet.transform.rotation = m_SpectatorRacquetPositionHelper.rotation;
            Camera.main.transform.position = m_SpectatorPositionHelper.position;
            Camera.main.transform.rotation = m_SpectatorPositionHelper.rotation;
        }
    }

    private void UpdateCurrentPlayers()
    {
        m_PlayerRacquets = m_PlayerRacquets.OrderBy(p => p.tno.owner.id).ToList();
        for (var i = 0; i < m_PlayerRacquets.Count; i++)
        {
            var playerRacquet = m_PlayerRacquets[i];
            if (i == 0)
            {
                playerRacquet.IsSpectating = false;
                playerRacquet.PlayerNumber = 1;
            }
            else if (i == 1)
            {
                playerRacquet.IsSpectating = false;
                playerRacquet.PlayerNumber = 2;
            }
            else
            {
                playerRacquet.IsSpectating = true;
                playerRacquet.PlayerNumber = -1;
            }
        }
    }

    private IEnumerator Start()
    {
        while (TNManager.isJoiningChannel || !TNManager.IsInChannel(tno.channelID) || Application.isLoadingLevel)
        {
            yield return new WaitForSeconds(0.1f);
        }

        TNManager.Instantiate(tno.channelID, "CreatePlayerRacquet", "PlayerRacquet", false, TNManager.playerID);
        while (m_PlayerRacquets.Count < 2)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (m_PlayerRacquets[0].tno.isMine)
        {
            TNManager.Instantiate(tno.channelID, "CreateBall", "Ball", false, TNManager.playerID);
        }

        while(Ball == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        m_PlayerRacquetWithBall = m_PlayerRacquets[0];
    }

    public PlayerRacquet GetPlayerRacquetByPlayerNumber(int playerNumber)
    {
        return m_PlayerRacquets[playerNumber - 1];
    }
    
    [RCC]
	static GameObject CreateBall(GameObject prefab, int playerId)
	{
		GameObject go = prefab.Instantiate();
		go.name = "Ball_" + playerId;
		return go;
	}

    public void AddBall(Ball ball)
    {
        Ball = ball;
        Ball.transform.position = m_BallSpawnPositionHelper.position;
    }

    private void Update()
    {
        if (Ball == null)
        {
            return;
        }
        if (m_PlayerRacquetWithBall == null)
        {
            return;
        }
        if (Serving)
        {
            Ball.transform.position = m_PlayerRacquetWithBall.transform.position;
            return;
        }
        if (m_PlayerRacquets.Count < 2)
        {
            return;
        }
        for (var i = 0; i < m_PredictedPointHelpers.Length; i++)
        {
            if (i == 0)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(Ball.PredictedPoint1);
                var pos = Camera.main.ScreenToWorldPoint(screenPoint);
                m_PredictedPointHelpers[i].transform.position = pos;
            }
            else if (i == 1)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(Ball.PredictedPoint2);
                var pos = Camera.main.ScreenToWorldPoint(screenPoint);
                m_PredictedPointHelpers[i].transform.position = pos;
            }
            else if (i == 2)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(Ball.PredictedPoint3);
                var pos = Camera.main.ScreenToWorldPoint(screenPoint);
                m_PredictedPointHelpers[i].transform.position = pos;
            }
        }
        
        if (m_PlayerRacquetWithBall != m_LocalPlayerRacquet)
        {
            return;
        }
        
        if (Ball.RB.velocity.magnitude <= m_MinBallVelocity)
        {
            m_TimeBallIsStationary += Time.deltaTime;
            if (m_TimeBallIsStationary >= m_MaxTimeBallReset)
            {
                m_TimeBallIsStationary = 0f;
                Serving = true;
                tno.Send("Serve", ForwardType.All, GetServingImpulseVector());
                return;
            }
        }
        else
        {
            m_TimeBallIsStationary = 0f;
        }
        if (Ball.transform.position.y <= m_GroundHelper.position.y)
        {
            Serving = true;
            tno.Send("Serve", ForwardType.All, GetServingImpulseVector());
        }
    }

    private Vector3 GetServingImpulseVector()
    {
        var impulseServingVector = m_ServingImpulseVector;
        if (m_LocalPlayerRacquet.PlayerNumber == 2)
        {
            impulseServingVector = new Vector3(impulseServingVector.x, impulseServingVector.y, impulseServingVector.z * -1);
        }
        return impulseServingVector;
    }

    [RFC]
    private void Serve(Vector3 serveVector)
    {
        StartCoroutine(StartServe(serveVector));
    }

    private IEnumerator StartServe(Vector3 serveVector)
    {
        Serving = true;
        Ball.RB.Sleep();
        Ball.RB.velocity = Vector3.zero;
        Ball.RB.angularVelocity = Vector3.zero;
        Ball.RB.useGravity = false;
        Ball.transform.position = m_PlayerRacquetWithBall.transform.position;
        yield return new WaitForSeconds(m_WaitTimeBeforeServing);
        Ball.RB.useGravity = true;
        Ball.RB.WakeUp();
        Ball.RB.AddForce(serveVector, ForceMode.Impulse);
        Serving = false;
    }

    public PlayerRacquet GetPlayerRacquetById(int id)
    {
        PlayerRacquet foundRacquet = null;
        foreach(var playerRacquet in m_PlayerRacquets)
        {
            if (playerRacquet.tno.owner.id == id)
            {
                foundRacquet = playerRacquet;
                break;
            }
        }
        return foundRacquet;
    }
}
