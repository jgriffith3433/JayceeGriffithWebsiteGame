//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using BestHTTP.SignalRCore;
//using System;
//using BestHTTP;
//using UnityEngine.UI;
//using BestHTTP.SignalRCore.Encoders;
//using GNet;

//public class TestSignalRCoreConnection : MonoBehaviour
//{
//    public Transform ContentT;
//    public Text TextP;
//    private HubConnection hub;

//    // Use this for initialization
//    void Start()
//    {
//        hub = new HubConnection(new Uri(TNManager.HubUrl), new MessagePackProtocol());
//        hub.AuthenticationProvider = new AzureSignalRServiceAuthenticator(hub);
//        hub.OnConnected += OnConnected;

//        //conn.On<string, string>("tnetDataReceived", OnBroadcastMessage);
//        hub.On<string>("tnetDataReceived", tnetDataReceived);

//        hub.StartConnect();
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            hub.Send("Send", "Pressed space");
//        }
//    }

//    private void OnBroadcastMessage(string name, string message)
//    {
//        Debug.LogFormat("[{0}]: {1}", name, message);
//        var t = Instantiate(TextP, ContentT);
//        t.text = name + ": " + message;
//    }

//    private void tnetDataReceived(string message)
//    {
//        Debug.LogFormat("{0}", message);
//        var t = Instantiate(TextP, ContentT);
//        t.text = message;
//    }

//    private void OnConnected(HubConnection hub)
//    {
//        hub.Send("Send", "Connected");
//    }
//}

//public sealed class AzureSignalRServiceAuthenticator : IAuthenticationProvider
//{
//    /// <summary>
//    /// No pre-auth step required for this type of authentication
//    /// </summary>
//    public bool IsPreAuthRequired { get { return false; } }

//#pragma warning disable 0067
//    /// <summary>
//    /// Not used event as IsPreAuthRequired is false
//    /// </summary>
//    public event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;

//    /// <summary>
//    /// Not used event as IsPreAuthRequired is false
//    /// </summary>
//    public event OnAuthenticationFailedDelegate OnAuthenticationFailed;

//#pragma warning restore 0067

//    private HubConnection _connection;

//    public AzureSignalRServiceAuthenticator(HubConnection connection)
//    {
//        this._connection = connection;
//    }

//    /// <summary>
//    /// Not used as IsPreAuthRequired is false
//    /// </summary>
//    public void StartAuthentication()
//    { }

//    /// <summary>
//    /// Prepares the request by adding two headers to it
//    /// </summary>
//    public void PrepareRequest(BestHTTP.HTTPRequest request)
//    {
//        if (this._connection.NegotiationResult == null)
//            return;

//        // Add Authorization header to http requests, add access_token param to the uri otherwise
//        if (BestHTTP.Connections.HTTPProtocolFactory.GetProtocolFromUri(request.CurrentUri) == BestHTTP.Connections.SupportedProtocols.HTTP)
//            request.SetHeader("Authorization", "Bearer " + this._connection.NegotiationResult.AccessToken);
//        else
//            request.Uri = PrepareUriImpl(request.Uri);
//    }

//    public Uri PrepareUri(Uri uri)
//    {
//        if (uri.Query.StartsWith("??"))
//        {
//            UriBuilder builder = new UriBuilder(uri);
//            builder.Query = builder.Query.Substring(2);

//            return builder.Uri;
//        }

//        return uri;
//    }

//    private Uri PrepareUriImpl(Uri uri)
//    {
//        string query = string.IsNullOrEmpty(uri.Query) ? "" : uri.Query + "&";
//        UriBuilder uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath, query + "access_token=" + this._connection.NegotiationResult.AccessToken);
//        return uriBuilder.Uri;
//    }
//}