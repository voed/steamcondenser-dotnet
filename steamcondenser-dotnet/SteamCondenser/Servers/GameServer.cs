using System;
using System.Collections.Generic;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers
{

	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using A2S_INFO_Packet = com.github.koraktor.steamcondenser.servers.packets.A2S_INFO_Packet;
	using A2S_PLAYER_Packet = com.github.koraktor.steamcondenser.servers.packets.A2S_PLAYER_Packet;
	using A2S_RULES_Packet = com.github.koraktor.steamcondenser.servers.packets.A2S_RULES_Packet;
	using S2A_INFO_BasePacket = com.github.koraktor.steamcondenser.servers.packets.S2A_INFO_BasePacket;
	using S2A_PLAYER_Packet = com.github.koraktor.steamcondenser.servers.packets.S2A_PLAYER_Packet;
	using S2A_RULES_Packet = com.github.koraktor.steamcondenser.servers.packets.S2A_RULES_Packet;
	using S2C_CHALLENGE_Packet = com.github.koraktor.steamcondenser.servers.packets.S2C_CHALLENGE_Packet;
	using SteamPacket = com.github.koraktor.steamcondenser.servers.packets.SteamPacket;
	using QuerySocket = com.github.koraktor.steamcondenser.servers.sockets.QuerySocket;

	/// <summary>
	/// This class is subclassed by classes representing different game server
	/// implementations and provides the basic functionality to communicate with
	/// them using the common query protocol
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public abstract class GameServer : Server
	{

		protected internal const int REQUEST_CHALLENGE = 0;
		protected internal const int REQUEST_INFO = 1;
		protected internal const int REQUEST_PLAYER = 2;
		protected internal const int REQUEST_RULES = 3;
		protected internal int challengeNumber = unchecked((int)0xFFFFFFFF);
		protected internal int ping;
		protected internal Dictionary<string, SteamPlayer> playerHash;
		protected internal bool rconAuthenticated;
		protected internal int rconRequestId;
		protected internal Dictionary<string, string> rulesHash;
		protected internal Dictionary<string, object> serverInfo;
		protected internal QuerySocket socket;

		/// <summary>
		/// Creates a new instance of a game server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected GameServer(Object address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		protected internal GameServer(object address, int? port) : base(address, port)
		{

			this.rconAuthenticated = false;
		}

		public override void disconnect()
		{
			if (this.socket != null)
			{
				this.socket.close();
				this.socket = null;
			}
		}

		/// <summary>
		/// Parses the player attribute names supplied by <code>rcon status</code>
		/// </summary>
		/// <param name="statusHeader"> The header line provided by <code>rcon status</code> </param>
		/// <returns> array Split player attribute names </returns>
		/// <seealso cref= #splitPlayerStatus </seealso>
		protected internal static IList<string> getPlayerStatusAttributes(string statusHeader)
		{
			IList<string> statusAttributes = new List<string>();
			foreach (string attribute in statusHeader.Split("\\s+", true))
			{
				if (attribute.Equals("connected"))
				{
					statusAttributes.Add("time");
				}
				else if (attribute.Equals("frag"))
				{
					statusAttributes.Add("score");
				}
				else
				{
					statusAttributes.Add(attribute);
				}
			}

			return statusAttributes;
		}

		/// <summary>
		/// Splits the player status obtained with <code>rcon status</code>
		/// </summary>
		/// <param name="attributes"> The attribute names </param>
		/// <param name="playerStatus"> The status line of a single player </param>
		/// <returns> array The attributes with the corresponding values for this
		///         player </returns>
		/// <seealso cref= #getPlayerStatusAttributes </seealso>
		protected internal static IDictionary<string, string> splitPlayerStatus(IList<string> attributes, string playerStatus)
		{
			if (!attributes[0].Equals("userid"))
			{
				playerStatus = playerStatus.replaceAll("^\\d+ +", "");
			}

			int firstQuote = playerStatus.IndexOf('"');
			int lastQuote = playerStatus.LastIndexOf('"');
			IList<string> tmpData = new List<string>();
			tmpData.Add(playerStatus.Substring(0, firstQuote));
			tmpData.Add(playerStatus.Substring(firstQuote + 1, lastQuote - (firstQuote + 1)));
			tmpData.Add(playerStatus.Substring(lastQuote + 1));

			IList<string> data = new List<string>();
			((IList<string>)data).AddRange(Arrays.asList(tmpData[0].Trim().Split("\\s+", true)));
			data.Add(tmpData[1]);
			((IList<string>)data).AddRange(Arrays.asList(tmpData[2].Trim().Split("\\s+", true)));
			data.Remove("");

			if (attributes.Count > data.Count && attributes.Contains("state"))
			{
				data.Insert(3, null);
				data.Insert(3, null);
				data.Insert(3, null);
			}
			else if (attributes.Count < data.Count)
			{
				data.RemoveAt(1);
			}

			IDictionary<string, string> playerData = new Dictionary<string, string>();
			for (int i = 0; i < data.Count; i++)
			{
				playerData[attributes[i]] = data[i];
			}

			return playerData;
		}

		/// <summary>
		/// Returns the last measured response time of this server
		/// <p/>
		/// If the latency hasn't been measured yet, it is done when calling this
		/// method for the first time.
		/// <p/>
		/// If this information is vital to you, be sure to call
		/// <seealso cref="#updatePing"/> regularly to stay up-to-date.
		/// </summary>
		/// <returns> The latency of this server in milliseconds </returns>
		/// <seealso cref= #updatePing </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getPing() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual int Ping
		{
			get
			{
				if (this.ping == 0)
				{
					this.updatePing();
				}
    
				return this.ping;
			}
		}

		/// <summary>
		/// Returns a list of players currently playing on this server
		/// <p/>
		/// If the players haven't been fetched yet, it is done when calling this
		/// method for the first time.
		/// <p/>
		/// As the players and their scores change quite often be sure to update
		/// this list regularly by calling <seealso cref="#updatePlayers"/> if you rely on
		/// this information.
		/// </summary>
		/// <returns> The players on this server </returns>
		/// <seealso cref= #updatePlayers </seealso>
		/// <exception cref="SteamCondenserException"> if a problem occurs while parsing the
		///         reply </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.HashMap<String, SteamPlayer> getPlayers() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual Dictionary<string, SteamPlayer> Players
		{
			get
			{
				return this.getPlayers(null);
			}
		}

		/// <summary>
		/// Returns a list of players currently playing on this server
		/// <p/>
		/// If the players haven't been fetched yet, it is done when calling this
		/// method for the first time.
		/// <p/>
		/// As the players and their scores change quite often be sure to update
		/// this list regularly by calling <seealso cref="#updatePlayers"/> if you rely on
		/// this information.
		/// </summary>
		/// <param name="rconPassword"> The RCON password of this server may be provided to
		///        gather more detailed information on the players, like STEAM_IDs. </param>
		/// <returns> The players on this server </returns>
		/// <seealso cref= #updatePlayers </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.HashMap<String, SteamPlayer> getPlayers(String rconPassword) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual Dictionary<string, SteamPlayer> getPlayers(string rconPassword)
		{
			if (this.playerHash == null)
			{
				this.updatePlayers(rconPassword);
			}

			return this.playerHash;
		}

		/// <summary>
		/// Returns the settings applied on the server. These settings are also
		/// called rules.
		/// <p/>
		/// If the rules haven't been fetched yet, it is done when calling this
		/// method for the first time.
		/// <p/>
		/// As the rules usually don't change often, there's almost no need to
		/// update this hash. But if you need to, you can achieve this by calling
		/// <seealso cref="#updateRules"/>.
		/// </summary>
		/// <returns> The currently active server rules </returns>
		/// <seealso cref= #updateRules </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.HashMap<String, String> getRules() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual Dictionary<string, string> Rules
		{
			get
			{
				if (this.rulesHash == null)
				{
					this.updateRules();
				}
    
				return this.rulesHash;
			}
		}

		/// <summary>
		/// Returns an associative array with basic information on the server.
		/// <p/>
		/// If the server information haven't been fetched yet, it is done when
		/// calling this method for the first time.
		/// <p/>
		/// The server information usually only changes on map change and when
		/// players join or leave. As the latter changes can be monitored by calling
		/// <seealso cref="#updatePlayers"/>, there's no need to call
		/// <seealso cref="#updateServerInfo"/> very often.
		/// </summary>
		/// <returns> Server attributes with their values </returns>
		/// <seealso cref= #updateServerInfo </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.HashMap<String, Object> getServerInfo() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual Dictionary<string, object> ServerInfo
		{
			get
			{
				if (this.serverInfo == null)
				{
					this.updateServerInfo();
				}
    
				return this.serverInfo;
			}
		}

		/// <summary>
		/// Receives a response from the server
		/// </summary>
		/// <returns> The response packet replied by the server </returns>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected com.github.koraktor.steamcondenser.servers.packets.SteamPacket getReply() throws java.util.concurrent.TimeoutException, com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		protected internal virtual SteamPacket Reply
		{
			get
			{
				return this.socket.Reply;
			}
		}

		/// <summary>
		/// Sends the specified request to the server and handles the returned
		/// response
		/// <p/>
		/// Depending on the given request type this will fill the various data
		/// attributes of the server object.
		/// </summary>
		/// <param name="requestType"> The type of request to send to the server </param>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void handleResponseForRequest(int requestType) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		protected internal virtual void handleResponseForRequest(int requestType)
		{
			this.handleResponseForRequest(requestType, true);
		}

		/// <summary>
		/// Sends the specified request to the server and handles the returned
		/// response
		/// <p/>
		/// Depending on the given request type this will fill the various data
		/// attributes of the server object.
		/// </summary>
		/// <param name="requestType"> The type of request to send to the server </param>
		/// <param name="repeatOnFailure"> Whether the request should be repeated, if
		///        the replied packet isn't expected. This is useful to handle
		///        missing challenge numbers, which will be automatically filled in,
		///        although not requested explicitly. </param>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void handleResponseForRequest(int requestType, boolean repeatOnFailure) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		protected internal virtual void handleResponseForRequest(int requestType, bool repeatOnFailure)
		{
			Type expectedResponse = typeof(SteamPacket);
			SteamPacket requestPacket = null;

			switch (requestType)
			{
				case GameServer.REQUEST_CHALLENGE:
					expectedResponse = typeof(S2C_CHALLENGE_Packet);
					requestPacket = new A2S_PLAYER_Packet();
					break;
				case GameServer.REQUEST_INFO:
					expectedResponse = typeof(S2A_INFO_BasePacket);
					requestPacket = new A2S_INFO_Packet();
					break;
				case GameServer.REQUEST_PLAYER:
					expectedResponse = typeof(S2A_PLAYER_Packet);
					requestPacket = new A2S_PLAYER_Packet(this.challengeNumber);
					break;
				case GameServer.REQUEST_RULES:
					expectedResponse = typeof(S2A_RULES_Packet);
					requestPacket = new A2S_RULES_Packet(this.challengeNumber);
					break;
			}

			this.sendRequest(requestPacket);

			SteamPacket responsePacket = this.Reply;

			if (typeof(S2A_INFO_BasePacket).IsInstanceOfType(responsePacket))
			{
				this.serverInfo = ((S2A_INFO_BasePacket) responsePacket).Info;
			}
			else if (responsePacket is S2A_PLAYER_Packet)
			{
				this.playerHash = ((S2A_PLAYER_Packet) responsePacket).PlayerHash;
			}
			else if (responsePacket is S2A_RULES_Packet)
			{
				this.rulesHash = ((S2A_RULES_Packet) responsePacket).RulesHash;
			}
			else if (responsePacket is S2C_CHALLENGE_Packet)
			{
				this.challengeNumber = ((S2C_CHALLENGE_Packet) responsePacket).ChallengeNumber;
			}
			else
			{
				throw new SteamCondenserException("Response of type " + responsePacket.GetType() + " cannot be handled by this method.");
			}

			if (!expectedResponse.IsInstanceOfType(responsePacket))
			{
                //TODO:replace it
				//LOG.warn("Expected " + expectedResponse + ", got " + responsePacket.GetType() + "."); 
				if (repeatOnFailure)
				{
					this.handleResponseForRequest(requestType, false);
				}
			}
		}

		/// <summary>
		/// Initializes this server object with basic information
		/// </summary>
		/// <seealso cref= #updateChallengeNumber </seealso>
		/// <seealso cref= #updatePing </seealso>
		/// <seealso cref= #updateServerInfo </seealso>
		/// <exception cref="SteamCondenserException"> if a request fails </exception>
		/// <exception cref="TimeoutException"> if a request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void initialize()
		{
			this.updatePing();
			this.updateServerInfo();
			this.updateChallengeNumber();
		}

		/// <summary>
		/// Returns whether the RCON connection to this server is already
		/// authenticated
		/// </summary>
		/// <returns> <code>true</code> if the RCON connection is authenticated </returns>
		/// <seealso cref= #rconAuth </seealso>
		public virtual bool RconAuthenticated
		{
			get
			{
				return this.rconAuthenticated;
			}
		}

		/// <summary>
		/// Authenticates with the server for RCON communication
		/// </summary>
		/// <param name="password"> The RCON password of the server </param>
		/// <returns> <code>true</code>, if the authentication was successful </returns>
		/// <seealso cref= #rconExec </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean rconAuth(String password) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException;
		public abstract bool rconAuth(string password);

		/// <summary>
		/// Remotely executes a command on the server via RCON
		/// </summary>
		/// <param name="command"> The command to execute on the server via RCON </param>
		/// <returns> The output of the executed command </returns>
		/// <seealso cref= #rconAuth </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract String rconExec(String command) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException;
		public abstract string rconExec(string command);

		/// <summary>
		/// Sends a request packet to the server
		/// </summary>
		/// <param name="requestData"> The request packet to send to the server </param>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sendRequest(com.github.koraktor.steamcondenser.servers.packets.SteamPacket requestData) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		protected internal virtual void sendRequest(SteamPacket requestData)
		{
			if (this.socket == null)
			{
				this.initSocket();
			}

			this.socket.send(requestData);
		}

		/// <summary>
		/// Returns a human-readable text representation of the server
		/// </summary>
		/// <returns> string Available information about the server in a
		///         human-readable format </returns>
		public override string ToString()
		{
			string returnString = "";

			returnString += "Ping: " + this.ping + "\n";
			returnString += "Challenge number: " + this.challengeNumber + "\n";

			if (this.serverInfo != null)
			{
				returnString += "Info:" + "\n";
				foreach (KeyValuePair<string, object> info in this.serverInfo.SetOfKeyValuePairs())
				{
					returnString += "  " + info.Key + ": " + info.Value + "\n";
				}
			}

			if (this.playerHash != null)
			{
				returnString += "Players:" + "\n";
				foreach (SteamPlayer player in this.playerHash.Values)
				{
					returnString += "  " + player + "\n";
				}
			}

			if (this.rulesHash != null)
			{
				returnString += "Rules:" + "\n";
				foreach (KeyValuePair<string, string> rule in this.rulesHash.SetOfKeyValuePairs())
				{
					returnString += "  " + rule.Key + ": " + rule.Value + "\n";
				}
			}

			return returnString;
		}

		/// <summary>
		/// Sends a A2S_SERVERQUERY_GETCHALLENGE request to the server and updates
		/// the challenge number used to communicate with this server
		/// <p/>
		/// There's usually no need to call this method explicitly, because
		/// <seealso cref="#handleResponseForRequest"/> will automatically get the challenge
		/// number when the server assigns a new one.
		/// </summary>
		/// <seealso cref= #handleResponseForRequest </seealso>
		/// <seealso cref= #initialize </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateChallengeNumber() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void updateChallengeNumber()
		{
			this.handleResponseForRequest(GameServer.REQUEST_CHALLENGE);
		}

		/// <summary>
		/// Sends a A2S_INFO request to the server and measures the time needed for
		/// the reply
		/// <p/>
		/// If this information is vital to you, be sure to call this method
		/// regularly to stay up-to-date.
		/// </summary>
		/// <seealso cref= #getPing </seealso>
		/// <seealso cref= #initialize </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updatePing() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void updatePing()
		{
			this.sendRequest(new A2S_INFO_Packet());
			long startTime = DateTimeHelper.CurrentUnixTimeMillis();
			this.Reply;
			long endTime = DateTimeHelper.CurrentUnixTimeMillis();
			this.ping = Convert.ToInt64(endTime - startTime).intValue();
		}

		/// <summary>
		/// Sends a A2S_PLAYERS request to the server and updates the players' data
		/// for this server
		/// <p/>
		/// As the players and their scores change quite often be sure to update
		/// this list regularly by calling this method if you rely on this
		/// information.
		/// </summary>
		/// <seealso cref= #getPlayers </seealso>
		/// <seealso cref= #handleResponseForRequest </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updatePlayers() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void updatePlayers()
		{
			this.updatePlayers(null);
		}

		/// <summary>
		/// Sends a A2S_PLAYERS request to the server and updates the players' data
		/// for this server
		/// <p/>
		/// As the players and their scores change quite often be sure to update
		/// this list regularly by calling this method if you rely on this
		/// information.
		/// </summary>
		/// <param name="rconPassword"> The RCON password of this server may be provided to
		///        gather more detailed information on the players, like STEAM_IDs. </param>
		/// <seealso cref= #getPlayers </seealso>
		/// <seealso cref= #handleResponseForRequest </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updatePlayers(String rconPassword) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void updatePlayers(string rconPassword)
		{
			this.handleResponseForRequest(GameServer.REQUEST_PLAYER);

			if (!this.rconAuthenticated)
			{
				if (string.ReferenceEquals(rconPassword, null))
				{
					return;
				}
				this.rconAuth(rconPassword);
			}

			IList<string> players = new List<string>();
			foreach (string line in Arrays.asList(this.rconExec("status").Split("\n", true)))
			{
				if (line.StartsWith("#", StringComparison.Ordinal) && !line.Equals("#end"))
				{
					players.Add(line.Substring(1).Trim());
				}
			}
			IList<string> attributes = getPlayerStatusAttributes(players.RemoveAt(0));

			foreach (string player in players)
			{
				IDictionary<string, string> playerData = splitPlayerStatus(attributes, player);
				string playerName = playerData["name"];
				if (this.playerHash.ContainsKey(playerName))
				{
					this.playerHash[playerName].addInformation(playerData);
				}
			}
		}

		/// <summary>
		/// Sends a A2S_RULES request to the server and updates the rules of this
		/// server
		/// <p/>
		/// As the rules usually don't change often, there's almost no need to
		/// update this hash. But if you need to, you can achieve this by calling
		/// this method.
		/// </summary>
		/// <seealso cref= #getRules </seealso>
		/// <seealso cref= #handleResponseForRequest </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateRules() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void updateRules()
		{
			this.handleResponseForRequest(GameServer.REQUEST_RULES);
		}

		/// <summary>
		/// Sends a A2S_INFO request to the server and updates this server's basic
		/// information
		/// <p/>
		/// The server information usually only changes on map change and when
		/// players join or leave. As the latter changes can be monitored by calling
		/// <seealso cref="#updatePlayers"/>, there's no need to call this method very often.
		/// </summary>
		/// <seealso cref= #getServerInfo </seealso>
		/// <seealso cref= #handleResponseForRequest </seealso>
		/// <seealso cref= #initialize </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateServerInfo() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void updateServerInfo()
		{
			this.handleResponseForRequest(GameServer.REQUEST_INFO);
		}
	}

}