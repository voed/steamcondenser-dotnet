/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers
{

	using RCONNoAuthException = com.github.koraktor.steamcondenser.exceptions.RCONNoAuthException;
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using GoldSrcSocket = com.github.koraktor.steamcondenser.servers.sockets.GoldSrcSocket;
    using System.Net;

    /// <summary>
    /// This class represents a GoldSrc game server and can be used to query
    /// information about and remotely execute commands via RCON on the server
    /// <p/>
    /// A GoldSrc game server is an instance of the Half-Life Dedicated Server
    /// (HLDS) running games using Valve's GoldSrc engine, like Half-Life
    /// Deathmatch, Counter-Strike 1.6 or Team Fortress Classic.
    /// 
    /// @author Sebastian Staudt </summary>
    /// <seealso cref= SourceServer </seealso>
    public class GoldSrcServer : GameServer
	{

		private bool isHLTV;

		protected internal string rconPassword;

		/// <summary>
		/// Returns a master server instance for the default master server for
		/// GoldSrc games
		/// </summary>
		/// <returns> The GoldSrc master server </returns>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static MasterServer getMaster() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public static MasterServer Master
		{
			get
			{
				return new MasterServer(MasterServer.GOLDSRC_MASTER_SERVER);
			}
		}

		/// <summary>
		/// Creates a new instance of a GoldSrc server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcServer(String address) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcServer(string address) : this(address, 27015, false)
		{
		}

		/// <summary>
		/// Creates a new instance of a GoldSrc server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcServer(String address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcServer(string address, int? port) : this(address, port, false)
		{
		}

		/// <summary>
		/// Creates a new instance of a GoldSrc server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <param name="isHLTV"> HLTV servers need special treatment, so this is used to
		///        determine if the server is a HLTV server </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcServer(String address, System.Nullable<int> port, boolean isHLTV) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcServer(string address, int? port, bool isHLTV) : base(address, port)
		{

			this.isHLTV = isHLTV;
		}

		/// <summary>
		/// Creates a new instance of a GoldSrc server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcServer(java.net.IPAddress address) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcServer(IPAddress address) : this(address, 27015, false)
		{
		}

		/// <summary>
		/// Creates a new instance of a GoldSrc server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcServer(java.net.IPAddress address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcServer(IPAddress address, int? port) : this(address, port, false)
		{
		}

		/// <summary>
		/// Creates a new instance of a GoldSrc server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <param name="isHLTV"> HLTV servers need special treatment, so this is
		///        used to determine if the server is a HLTV server </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcServer(java.net.IPAddress address, System.Nullable<int> port, boolean isHLTV) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcServer(IPAddress address, int? port, bool isHLTV) : base(address, port)
		{

			this.isHLTV = isHLTV;
		}

		/// <summary>
		/// Initializes the socket to communicate with the GoldSrc server
		/// </summary>
		/// <seealso cref= GoldSrcSocket </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initSocket() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public override void initSocket()
		{
			this.socket = new GoldSrcSocket(this.ipAddress, this.port, this.isHLTV);
		}

		/// <summary>
		/// Saves the password for authenticating the RCON communication with the
		/// server
		/// </summary>
		/// <param name="password"> The RCON password of the server </param>
		/// <returns> GoldSrc's RCON does not preauthenticate connections so
		///         this method always returns <code>true</code> </returns>
		/// <seealso cref= #rconExec </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean rconAuth(String password) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public override bool rconAuth(string password)
		{
			this.rconPassword = password;

			try
			{
				this.rconAuthenticated = true;
				this.rconExec("");
			}
			catch (RCONNoAuthException)
			{
				this.rconAuthenticated = false;
				this.rconPassword = null;
			}

			return this.rconAuthenticated;
		}

		/// <summary>
		/// Remotely executes a command on the server via RCON
		/// </summary>
		/// <param name="command"> The command to execute on the server via RCON </param>
		/// <returns> The output of the executed command </returns>
		/// <seealso cref= #rconExec </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String rconExec(String command) throws java.util.concurrent.TimeoutException, com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public override string rconExec(string command)
		{
			if (!this.rconAuthenticated)
			{
				throw new RCONNoAuthException();
			}

			try
			{
				return ((GoldSrcSocket) this.socket).rconExec(this.rconPassword, command).Trim();
			}
			catch (RCONNoAuthException e)
			{
				this.rconAuthenticated = false;
				throw e;
			}
		}

	}

}