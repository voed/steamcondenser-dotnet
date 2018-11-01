using System.Collections.Generic;

/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2015, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers
{

	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;

	/// <summary>
	/// This class represents a player connected to a game server
	/// 
	/// @author  Sebastian Staudt
	/// </summary>
	public class SteamPlayer
	{

		private int clientPort;
		private int connectionId;
		private float connectTime;
		private bool extended;
		private int id;
		private string ipAddress;
		private int loss;
		private string name;
		private int ping;
		private int rate;
		private int score;
		private string state;
		private string steamId;

		/// <summary>
		/// Creates a new player instancewith the given information
		/// </summary>
		/// <param name="id"> The ID of the player on the server </param>
		/// <param name="name"> The name of the player </param>
		/// <param name="score"> The score of the player </param>
		/// <param name="connectTime"> The time the player is connected to the server </param>
		public SteamPlayer(int id, string name, int score, float connectTime)
		{
			this.connectTime = connectTime;
			this.id = id;
			this.name = name;
			this.score = score;
			this.extended = false;
		}

		/// <summary>
		/// Extends a player object with information retrieved from a RCON call to
		/// the status command
		/// </summary>
		/// <param name="playerData"> The player data retrieved from <code>rcon
		///        status</code> </param>
		/// <exception cref="SteamCondenserException"> if the information belongs to another
		///         player </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInformation(java.util.Map<String, String> playerData) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public virtual void addInformation(IDictionary<string, string> playerData)
		{
			if (!playerData["name"].Equals(this.name))
			{
				throw new SteamCondenserException("Information to add belongs to a different player.");
			}

			this.extended = true;
			this.connectionId = int.Parse(playerData["userid"]);
			this.steamId = playerData["uniqueid"];

			if (playerData.ContainsKey("state"))
			{
				this.state = playerData["state"];
			}

			if (!this.Bot && !this.HLTV)
			{
				this.loss = int.Parse(playerData["loss"]);
				this.ping = int.Parse(playerData["ping"]);

				if (playerData.ContainsKey("adr"))
				{
					string[] address = playerData["adr"].Split(":", true);
					this.ipAddress = address[0];
					this.clientPort = int.Parse(address[1]);
				}

				if (playerData.ContainsKey("rate"))
				{
					this.rate = int.Parse(playerData["rate"]);
				}
			}
		}

		/// <summary>
		/// Returns the client port of this player
		/// </summary>
		/// <returns> The client port of the player </returns>
		public virtual int ClientPort
		{
			get
			{
				return this.clientPort;
			}
		}

		/// <summary>
		/// Returns the connection ID (as used on the server) of this player
		/// </summary>
		/// <returns> The connection ID of this player </returns>
		public virtual int ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		/// <summary>
		/// Returns the time this player is connected to the server
		/// </summary>
		/// <returns> The connection time of the player </returns>
		public virtual float ConnectTime
		{
			get
			{
				return this.connectTime;
			}
		}

		/// <summary>
		/// Returns the ID of this player
		/// </summary>
		/// <returns> The ID of this player </returns>
		public virtual int Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Returns the IP address of this player
		/// </summary>
		/// <returns> The IP address of this player </returns>
		public virtual string IpAddress
		{
			get
			{
				return this.ipAddress;
			}
		}

		/// <summary>
		/// Returns the packet loss of this player's connection
		/// </summary>
		/// <returns> The packet loss of this player's connection </returns>
		public virtual int Loss
		{
			get
			{
				return this.loss;
			}
		}

		/// <summary>
		/// Returns the nickname of this player
		/// </summary>
		/// <returns> The name of this player </returns>
		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Returns the ping of this player
		/// </summary>
		/// <returns> The ping of this player </returns>
		public virtual int Ping
		{
			get
			{
				return this.ping;
			}
		}

		/// <summary>
		/// Returns the rate of this player
		/// </summary>
		/// <returns> The rate of this player </returns>
		public virtual int Rate
		{
			get
			{
				return this.rate;
			}
		}

		/// <summary>
		/// Returns the score of this player
		/// </summary>
		/// <returns> The score of this player </returns>
		public virtual int Score
		{
			get
			{
				return this.score;
			}
		}

		/// <summary>
		/// Returns the connection state of this player
		/// </summary>
		/// <returns> The connection state of this player </returns>
		public virtual string State
		{
			get
			{
				return this.state;
			}
		}

		/// <summary>
		/// Returns the SteamID of this player
		/// </summary>
		/// <returns> The SteamID of this player </returns>
		public virtual string SteamId
		{
			get
			{
				return this.steamId;
			}
		}

		/// <summary>
		/// Returns whether this player is a bot
		/// </summary>
		/// <returns> <code>true</code> if this player is a bot </returns>
		public virtual bool Bot
		{
			get
			{
				return this.steamId.Equals("BOT");
			}
		}

		/// <summary>
		/// Returns whether this client is a HLTV
		/// </summary>
		/// <returns> <code>true</code> if this client is a HLTV </returns>
		public virtual bool HLTV
		{
			get
			{
				return this.steamId.Equals("HLTV");
			}
		}

		/// <summary>
		/// Returns whether this player object has extended information gathered
		/// using RCON
		/// </summary>
		/// <returns> <code>true</code> if extended information for this player is
		///         available </returns>
		public virtual bool Extended
		{
			get
			{
				return this.extended;
			}
		}

		/// <summary>
		/// Returns a string representation of this player
		/// </summary>
		/// <returns> A string representing this player </returns>
		public override string ToString()
		{
			if (this.extended)
			{
				return "#" + this.connectionId + " \"" + this.name + "\", SteamID: " + this.steamId + ", Score: " + this.score + ", Time: " + this.connectTime;
			}
			else
			{
				return "#" + this.id + " \"" + this.name + "\", Score: " + this.score + ", Time: " + this.connectTime;
			}
		}
	}

}