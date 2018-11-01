using System;
using System.Collections.Generic;
using System.Net;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers
{

	using RCONBanException = com.github.koraktor.steamcondenser.exceptions.RCONBanException;
	using RCONNoAuthException = com.github.koraktor.steamcondenser.exceptions.RCONNoAuthException;
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using RCONAuthRequestPacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONAuthRequestPacket;
	using RCONAuthResponse = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONAuthResponse;
	using RCONExecRequestPacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONExecRequestPacket;
	using RCONExecResponsePacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONExecResponsePacket;
	using RCONPacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONPacket;
	using RCONTerminator = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONTerminator;
	using RCONSocket = com.github.koraktor.steamcondenser.servers.sockets.RCONSocket;
	using SourceSocket = com.github.koraktor.steamcondenser.servers.sockets.SourceSocket;

	/// <summary>
	/// This class represents a Source game server and can be used to query
	/// information about and remotely execute commands via RCON on the server
	/// <p/>
	/// A Source game server is an instance of the Source Dedicated Server (SrcDS)
	/// running games using Valve's Source engine, like Counter-Strike: Source,
	/// Team Fortress 2 or Left4Dead.
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= GoldSrcServer </seealso>
	public class SourceServer : GameServer
	{

		protected internal RCONSocket rconSocket;

		/// <summary>
		/// Returns a master server instance for the default master server for
		/// Source games
		/// </summary>
		/// <returns> The Source master server </returns>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MasterServer getMaster() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public static MasterServer Master
		{
			get
			{
				return new MasterServer(MasterServer.SOURCE_MASTER_SERVER);
			}
		}

		/// <summary>
		/// Creates a new instance of a server object representing a Source server,
		/// i.e. SrcDS instance
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them
		///        combined with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SourceServer(String address) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public SourceServer(string address) : base(address, 27015)
		{
		}

		/// <summary>
		/// Creates a new instance of a server object representing a Source server,
		/// i.e. SrcDS instance
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them
		///        combined with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SourceServer(String address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public SourceServer(string address, int? port) : base(address, port)
		{
		}

		/// <summary>
		/// Creates a new instance of a server object representing a Source server,
		/// i.e. SrcDS instance
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them
		///        combined with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SourceServer(java.net.IPAddress address) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public SourceServer(IPAddress address) : base(address, 27015)
		{
		}

		/// <summary>
		/// Creates a new instance of a server object representing a Source server,
		/// i.e. SrcDS instance
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them
		///        combined with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SourceServer(java.net.IPAddress address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public SourceServer(IPAddress address, int? port) : base(address, port)
		{
		}

		/// <summary>
		/// Disconnects the TCP-based channel used for RCON commands
		/// </summary>
		/// <seealso cref= RCONSocket#close </seealso>
		public override void disconnect()
		{
			base.disconnect();

			this.rconSocket.close();
		}

		/// <summary>
		/// Initializes the sockets to communicate with the Source server
		/// </summary>
		/// <seealso cref= RCONSocket </seealso>
		/// <seealso cref= SourceSocket </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initSocket() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public override void initSocket()
		{
			this.rconSocket = new RCONSocket(this.ipAddress, this.port);
			this.socket = new SourceSocket(this.ipAddress, this.port);
		}

		/// <summary>
		/// Authenticates the connection for RCON communication with the server
		/// </summary>
		/// <param name="password"> The RCON password of the server </param>
		/// <returns> whether authentication was successful </returns>
		/// <seealso cref= #rconAuth </seealso>
		/// <exception cref="RCONBanException"> if banned by the server </exception>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean rconAuth(String password) throws java.util.concurrent.TimeoutException, com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public override bool rconAuth(string password)
		{
			this.rconRequestId = (new Random()).Next();

			this.rconSocket.send(new RCONAuthRequestPacket(this.rconRequestId, password));
			RCONPacket reply = this.rconSocket.Reply;
			if (reply == null)
			{
				throw new RCONBanException();
			}
			reply = this.rconSocket.Reply;
			this.rconAuthenticated = reply.RequestId == this.rconRequestId;

			return this.rconAuthenticated;
		}

		/// <summary>
		/// Remotely executes a command on the server via RCON
		/// </summary>
		/// <param name="command"> The command to execute on the server via RCON </param>
		/// <returns> The output of the executed command </returns>
		/// <seealso cref= #rconExec </seealso>
		/// <exception cref="RCONBanException"> if banned by the server </exception>
		/// <exception cref="RCONNoAuthException"> if not authenticated with the server </exception>
		/// <exception cref="SteamCondenserException"> if a problem occurs while parsing the
		///         reply </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String rconExec(String command) throws java.util.concurrent.TimeoutException, com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public override string rconExec(string command)
		{
			if (!this.rconAuthenticated)
			{
				throw new RCONNoAuthException();
			}

			this.rconSocket.send(new RCONExecRequestPacket(this.rconRequestId, command));

			bool isMulti = false;
			 RCONPacket responsePacket;
			List<string> response = new List<string>();
			do
			{
				responsePacket = this.rconSocket.Reply;

				if (responsePacket == null || responsePacket is RCONAuthResponse)
				{
					this.rconAuthenticated = false;
					throw new RCONNoAuthException();
				}

				if (!isMulti && ((RCONExecResponsePacket) responsePacket).Response.Length > 0)
				{
					isMulti = true;
					this.rconSocket.send(new RCONTerminator(this.rconRequestId));
				}
				response.Add(((RCONExecResponsePacket) responsePacket).Response);
			} while (isMulti && !(response.Count > 2 && response[response.Count - 2].Equals("") && response[response.Count - 1].Equals("")));

			return StringUtils.join(response.ToArray()).Trim();
		}

	}

}