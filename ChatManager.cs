// Developed by Roberto C. Tan Jr. Copyright (c) 2018, all rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;

namespace FrameWork.Network
{
    /// <summary>
    /// Is a class that manages the chat logic and messaging between client and the server.
    /// </summary>
    public class ChatManager : MonoBehaviour, IChatClientListener
    {
        #region Variables
        public static ChatManager managed;

        [Header("General Settings")]

        [Tooltip("Is the default channel that the player can automatically get into after entering the game.")]
        [SerializeField]
        private string lobbyChannel;

        [Space(40)]

        [Header("Debug")]

        [Tooltip("Should this send console messages?")]
        [SerializeField]
        private bool printConsoleMessages = false;

        private ChatUIManager chatUIManager;
        private string activeChannel;
        private ChatClient chatClient;
        private string chatName = string.Empty;
        private bool isConnected = false;

        private List<string> subscribedChannels;

        public delegate void OnConnectedDelegate();
        public delegate void OnDisconnectedDelegate();
        public delegate void OnChatStateChangeDelegate(ChatState state);
        public delegate void OnGetMessagesDelegate(string channelName, string[] senders, object[] messages, string activeChannelMessages);
        public delegate void OnPrivateMessageDelegate(string sender, object message, string channelName);
        public delegate void OnStatusUpdateDelegate(string user, int status, bool gotMessage, object message);
        public delegate void OnSubscribedDelegate(string[] channels, bool[] results);
        public delegate void OnUnsubscribedDelegate(string[] channels);

        private OnConnectedDelegate onConnectedEvent;
        private OnDisconnectedDelegate onDisconnectedEvent;
        private OnChatStateChangeDelegate onChatStateChangeEvent;
        private OnGetMessagesDelegate onGetMessagesEvent;
        private OnPrivateMessageDelegate onPrivateMessageEvent;
        private OnStatusUpdateDelegate onStatusUpdateEvent;
        private OnSubscribedDelegate onSubscribedEvent;
        private OnUnsubscribedDelegate onUnsubscribedEvent;
        #endregion

        #region Properties
        public bool IsConnected { get { return isConnected; } }
        public string LobbyChannel { get { return lobbyChannel; } }
        public string ActiveChannel { get { return activeChannel; } }
        #endregion

        #region Unity Standard Logic
        // Use this for initialization before the object starts to initialize.
        void Awake()
        {
            if (managed != null) // Is there an existing chat manager?
            {
                // So, destroy us.
                Destroy(gameObject);
            }
            else
            {
                managed = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        // Update is called once per frame.
        void Update()
        {
            if (chatClient != null) // Is this valid?
            {
                chatClient.Service();
            }
        }
        #endregion

        #region Custom Logic - Subscribe Methods
        /// <summary>
        /// Is called to subscribe a method to on connected event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnConnectedEvent(OnConnectedDelegate callback)
        {
            onConnectedEvent += callback;

        }

        /// <summary>
        /// Is called to subscribe a method to on disconnected event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnDisconnectedEvent(OnDisconnectedDelegate callback)
        {
            onDisconnectedEvent += callback;
        }

        /// <summary>
        /// Is called to subscribe a method to on chat state change event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnChatStateChangeEvent(OnChatStateChangeDelegate callback)
        {
            onChatStateChangeEvent += callback;
        }

        /// <summary>
        /// Is called to subscribe a method to on get messages event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnGetMessagesEvent(OnGetMessagesDelegate callback)
        {
            onGetMessagesEvent += callback;
        }

        /// <summary>
        /// Is called to subscribe a method to on private message event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnPrivateMessageEvent(OnPrivateMessageDelegate callback)
        {
            onPrivateMessageEvent += callback;
        }

        /// <summary>
        /// Is called to subscribe a method to on status update event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnStatusUpdateEvent(OnStatusUpdateDelegate callback)
        {
            onStatusUpdateEvent += callback;
        }

        /// <summary>
        /// Is called to subscribe a method to on subscribe event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnSubscribedEvent(OnSubscribedDelegate callback)
        {
            onSubscribedEvent += callback;
        }

        /// <summary>
        /// Is called to subscribe a method to on unsubscribe event.
        /// </summary>
        /// <param name="callback">Method to subscribe.</param>
        public void SubscribeToOnUnsubscribedEvent(OnUnsubscribedDelegate callback)
        {
            onUnsubscribedEvent += callback;
        }
        #endregion

        #region Custom Logic - Public
        public void ChatToggle()
        {
            if (!isConnected) return;

            if (chatUIManager == null) chatUIManager = GetComponentInChildren<ChatUIManager>();

            chatUIManager.OnChatToggle();
        }

        /// <summary>
        /// Is called to connect the photon chat service.
        /// </summary>
        public void Connect()
        {
            chatClient = new ChatClient(this);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
                               PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion,
                               new AuthenticationValues(chatName));
        }

        /// <summary>
        /// Disconnects the chat.
        /// </summary>
        public void Disconnect(string otherPlayerName)
        {
            if (subscribedChannels == null || subscribedChannels.Count <= 0) return;

            // We notify others that we are leaving the room.
            for (int i = 0; i < subscribedChannels.Count; i++)
            {
                chatClient.PublishMessage(subscribedChannels[i], otherPlayerName + ": !(#CODE0001#)");
            }
        }

        /// <summary>
        /// Set the chat name.
        /// </summary>
        /// <param name="targetChatName">Target chat name.</param>
        public void SetChatName(string targetChatName)
        {
            chatName = targetChatName;
        }

        /// <summary>
        /// Returns the players chat name.
        /// </summary>
        public string GetChatName()
        {
            return chatName;
        }

        /// <summary>
        /// Sends a message to subscribed channel(s).
        /// </summary>
        /// <param name="channels">Channels where to send the message to.</param>
        /// <param name="chatMessage">Message to send.</param>
        public void SendChatMessage(string[] channels, string chatMessage)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                chatClient.PublishMessage(channels[i], chatMessage);
            }
        }

        /// <summary>
        /// Is called to subscribe to a new channel.
        /// </summary>
        /// <param name="newChannel">New channel to subscribe.</param>
        /// <param name="isSetToActive">Make the new channel as the active one.</param>
        public void Subscribe(string newChannel, bool isSetToActive)
        {
            if (subscribedChannels == null) // Is this not initiated yet?
            {
                subscribedChannels = new List<string>();
            }

            if (!subscribedChannels.Contains(newChannel)) subscribedChannels.Add(newChannel);

            // Check if this should be the active channel.
            activeChannel = isSetToActive ? newChannel : activeChannel;

            string[] newAllocatedChannels = new string[subscribedChannels.Count];

            for (int i = 0; i < subscribedChannels.Count; i++)
            {
                newAllocatedChannels[i] = subscribedChannels[i];
            }

            chatClient.Subscribe(newAllocatedChannels);
        }

        /// <summary>
        /// Is called to unsubscribe from channel(s).
        /// </summary>
        /// <param name="channelsToUnsubscribe"></param>
        public void UnSubscribe(string[] channelsToUnsubscribe)
        {
            // Return back to the lobby.
            activeChannel = lobbyChannel;
            chatClient.Unsubscribe(channelsToUnsubscribe);
        }

        /// <summary>
        /// Is called to unsubscribe from all channels.
        /// </summary>
        public void UnSubscribeToAll()
        {
            string[] newAllocatedChannels = new string[subscribedChannels.Count];

            for (int i = 0; i < subscribedChannels.Count; i++)
            {
                newAllocatedChannels[i] = subscribedChannels[i];
            }

            chatClient.Unsubscribe(newAllocatedChannels);
        }
        #endregion

        #region Custom Logic - Chat Callbacks
        /// <summary>
        /// Is called when all debug output of the library will be reported through this method. Print it or put it in a buffer to use it on-screen.
        /// </summary>
        /// <param name="level">DebugLevel (severity) of the message.</param>
        /// <param name="message">Debug text. Print to System.Console or screen.</param>
        public void DebugReturn(DebugLevel level, string message)
        {

        }

        /// <summary>
        /// Is called whenb the ChatClient's state changed. Usually, OnConnected and OnDisconnected are the callbacks to react to.
        /// </summary>
        /// <param name="state">The new state.</param>    
        public void OnChatStateChange(ChatState state)
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> On chat state changed: " + state.ToString());

            if (onChatStateChangeEvent != null)
            {
                onChatStateChangeEvent(state);
            }
        }

        /// <summary>
        /// Is called when the client is connected.
        /// </summary>
        /// <remarks>Clients have to be connected before they can send their state, subscribe to channels and send any messages.</remarks>
        public void OnConnected()
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> Successfully connected...");

            isConnected = true;
            activeChannel = lobbyChannel;
            Subscribe(lobbyChannel, true);
            chatClient.SetOnlineStatus(ChatUserStatus.Online);

            if (onConnectedEvent != null)
            {
                onConnectedEvent();
            }
        }

        /// <summary>
        /// Is called when disconnection happens.
        /// </summary>
        public void OnDisconnected()
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> Successfully disconnected...");

            isConnected = false;

            if (onDisconnectedEvent != null)
            {
                onDisconnectedEvent();
            }
        }

        /// <summary>
        /// Notifies app that client got new messages from server
        /// Number of senders is equal to number of messages in 'messages'. Sender with number '0' corresponds to message with
        /// number '0', sender with number '1' corresponds to message with number '1' and so on
        /// </summary>
        /// <param name="channelName">channel from where messages came</param>
        /// <param name="senders">list of users who sent messages</param>
        /// <param name="messages">list of messages it self</param>
        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> OnGetMessages is being called from channel: " + channelName);

            if (string.IsNullOrEmpty(channelName)) return;

            ChatChannel channel = null;
            bool foundChannel = chatClient.TryGetChannel(channelName, out channel);

            if (foundChannel)
            {
                if (onGetMessagesEvent != null)
                {
                    onGetMessagesEvent(channelName, senders, messages, channel.ToStringMessages());
                }
            }
        }

        /// <summary>
        /// Notifies client about private message
        /// </summary>
        /// <param name="sender">user who sent this message</param>
        /// <param name="message">message it self</param>
        /// <param name="channelName">channelName for private messages (messages you sent yourself get added to a channel per target username)</param>
        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> OnPrivateMessage is being called from channel: " + channelName + " sender: " + sender + " message: " + message);

            if (onPrivateMessageEvent != null)
            {
                onPrivateMessageEvent(sender, message, channelName);
            }
        }

        /// <summary>
        /// New status of another user (you get updates for users set in your friends list).
        /// </summary>
        /// <param name="user">Name of the user.</param>
        /// <param name="status">New status of that user.</param>
        /// <param name="gotMessage">True if the status contains a message you should cache locally. False: This status update does not include a message (keep any you have).</param>
        /// <param name="message">Message that user set.</param>
        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> OnStatusUpdate is being called");

            if (onStatusUpdateEvent != null)
            {
                onStatusUpdateEvent(user, status, gotMessage, message);
            }
        }

        /// <summary>
        /// Result of Subscribe operation. Returns subscription result for every requested channel name.
        /// </summary>
        /// <remarks>
        /// If multiple channels sent in Subscribe operation, OnSubscribed may be called several times, each call with part of sent array or with single channel in "channels" parameter. 
        /// Calls order and order of channels in "channels" parameter may differ from order of channels in "channels" parameter of Subscribe operation.
        /// </remarks>
        /// <param name="channels">Array of channel names.</param>
        /// <param name="results">Per channel result if subscribed.</param>
        public void OnSubscribed(string[] channels, bool[] results)
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> OnSubscribed is called...");

            if (onSubscribedEvent != null)
            {
                onSubscribedEvent(channels, results);
            }
        }

        /// <summary>
        /// Result of Unsubscribe operation. Returns for channel name if the channel is now unsubscribed.
        /// </summary>
        /// If multiple channels sent in Unsubscribe operation, OnUnsubscribed may be called several times, each call with part of sent array or with single channel in "channels" parameter. 
        /// Calls order and order of channels in "channels" parameter may differ from order of channels in "channels" parameter of Unsubscribe operation.
        /// <param name="channels">Array of channel names that are no longer subscribed.</param>
        public void OnUnsubscribed(string[] channels)
        {
            if (printConsoleMessages) Debug.Log("ChatManager::Console Message -> OnUnSubscribed is called...");

            if (onUnsubscribedEvent != null)
            {
                onUnsubscribedEvent(channels);
            }
        }
        #endregion
    }
}