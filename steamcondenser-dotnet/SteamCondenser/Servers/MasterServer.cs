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


	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using A2M_GET_SERVERS_BATCH2_Packet = com.github.koraktor.steamcondenser.servers.packets.A2M_GET_SERVERS_BATCH2_Packet;
	using M2A_SERVER_BATCH_Packet = com.github.koraktor.steamcondenser.servers.packets.M2A_SERVER_BATCH_Packet;
	using MasterServerSocket = com.github.koraktor.steamcondenser.servers.sockets.MasterServerSocket;

	/// <summary>
	/// This class represents a Steam master server and can be used to get game
	/// servers which are publicly available
	/// <p/>
	/// An instance of this class can be used much like Steam's server browser to
	/// get a list of available game servers, including filters to narrow down the
	/// search results.
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public class MasterServer : Server
	{

		/// <summary>
		/// The master server address to query for GoldSrc game servers
		/// </summary>
		public const string GOLDSRC_MASTER_SERVER = "hl1master.steampowered.com:27011";

		/// <summary>
		/// The master server address to query for GoldSrc game servers
		/// </summary>
		public const string SOURCE_MASTER_SERVER = "hl2master.steampowered.com:27011";

		/// <summary>
		/// The region code for the US east coast
		/// </summary>
		public const sbyte REGION_US_EAST_COAST = 0x00;

		/// <summary>
		/// The region code for the US west coast
		/// </summary>
		public const sbyte REGION_US_WEST_COAST = 0x01;

		/// <summary>
		/// The region code for South America
		/// </summary>
		public const sbyte REGION_SOUTH_AMERICA = 0x02;

		/// <summary>
		/// The region code for Europe
		/// </summary>
		public const sbyte REGION_EUROPE = 0x03;

		/// <summary>
		/// The region code for Asia
		/// </summary>
		public const sbyte REGION_ASIA = 0x04;

		/// <summary>
		/// The region code for Australia
		/// </summary>
		public const sbyte REGION_AUSTRALIA = 0x05;

		/// <summary>
		/// The region code for the Middle East
		/// </summary>
		public const sbyte REGION_MIDDLE_EAST = 0x06;

		/// <summary>
		/// The region code for Africa
		/// </summary>
		public const sbyte REGION_AFRICA = 0x07;

		/// <summary>
		/// The region code for the whole world
		/// </summary>
		public static readonly sbyte REGION_ALL = unchecked((sbyte)0xFF);

		public static int retries = 3;

		protected internal MasterServerSocket socket;

		/// <summary>
		/// Sets the number of consecutive requests that may fail, before getting
		/// the server list is cancelled (default: 3)
		/// </summary>
		/// <param name="newRetries"> The number of allowed retries </param>
		public static int Retries
		{
			set
			{
				retries = value;
			}
		}

		/// <summary>
		/// Creates a new instance of a master server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MasterServer(String address) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public MasterServer(string address) : base(address, null)
		{
		}

		/// <summary>
		/// Creates a new instance of a master server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MasterServer(String address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public MasterServer(string address, int? port) : base(address, port)
		{
		}

		/// <summary>
		/// Creates a new instance of a GoldSrc server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MasterServer(java.net.IPAddress address) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public MasterServer(IPAddress address) : base(address, null)
		{
		}

		/// <summary>
		/// Creates a new instance of a master server object
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MasterServer(java.net.IPAddress address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public MasterServer(IPAddress address, int? port) : base(address, port)
		{
		}

		/// <summary>
		/// Returns a list of all available game servers
		/// <p/>
		/// <strong>Note:</strong> Receiving all servers from the master server is
		/// taking quite some time.
		/// </summary>
		/// <returns> A list of game servers matching the given
		///         region and filters </returns>
		/// <seealso cref= A2M_GET_SERVERS_BATCH2_Packet </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if too many timeouts occur while querying the
		///         master server </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<java.net.InetSocketAddress> getServers() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual ISet<InetSocketAddress> Servers
		{
			get
			{
				return this.getServers(MasterServer.REGION_ALL, "", false);
			}
		}

		/// <summary>
		/// Returns a list of game server matching the given region and filters
		/// <p/>
		/// Filtering:
		/// Instead of filtering the results sent by the master server locally, you
		/// should at least use the filters listed at {@link
		/// MasterServer#getServers(byte, String, boolean)} to narrow down the
		/// results sent by the master server.
		/// <p/>
		/// <strong>Note:</strong> Receiving all servers from the master server is
		/// taking quite some time.
		/// </summary>
		/// <param name="regionCode"> The region code to specify a location of the game
		///        servers </param>
		/// <param name="filter"> The filters that game servers should match </param>
		/// <returns> A list of game servers matching the given
		///         region and filters </returns>
		/// <seealso cref= A2M_GET_SERVERS_BATCH2_Packet </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if too many timeouts occur while querying the
		///         master server </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<java.net.InetSocketAddress> getServers(byte regionCode, String filter) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual ISet<InetSocketAddress> getServers(sbyte regionCode, string filter)
		{
			return this.getServers(regionCode, filter, false);
		}

		/// <summary>
		/// Returns a list of game server matching the given region and filters
		/// <p/>
		/// Filtering:
		/// Instead of filtering the results sent by the master server locally, you
		/// should at least use the following filters to narrow down the results
		/// sent by the master server.
		/// <p/>
		/// <strong>Note:</strong> Receiving all servers from the master server is
		/// taking quite some time.
		/// 
		/// Available filters:
		/// 
		/// <ul>
		/// <li><code>\type\d</code>: Request only dedicated servers
		/// <li><code>\secure\1</code>: Request only secure servers
		/// <li><code>\gamedir\[mod]</code>: Request only servers of a specific mod
		/// <li><code>\map\[mapname]</code>: Request only servers running a specific
		///     map
		/// <li><code>\linux\1</code>: Request only linux servers
		/// <li><code>\emtpy\1</code>: Request only **non**-empty servers
		/// <li><code>\full\1</code>: Request only servers **not** full
		/// <li><code>\proxy\1</code>: Request only spectator proxy servers
		/// </ul>
		/// </summary>
		/// <param name="regionCode"> The region code to specify a location of the game
		///        servers </param>
		/// <param name="filter"> The filters that game servers should match </param>
		/// <returns> A list of game servers matching the given
		///         region and filters </returns>
		/// <seealso cref= A2M_GET_SERVERS_BATCH2_Packet </seealso>
		/// <exception cref="SteamCondenserException"> if the request fails </exception>
		/// <exception cref="TimeoutException"> if too many timeouts occur while querying the
		///         master server </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<java.net.InetSocketAddress> getServers(byte regionCode, String filter, boolean force) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual ISet<InetSocketAddress> getServers(sbyte regionCode, string filter, bool force)
		{
			int failCount = 0;
			bool finished = false;
			int portNumber = 0;
			string hostName = "0.0.0.0";
			List<string> serverStringArray;
			ISet<InetSocketAddress> serverSet = new HashSet<InetSocketAddress>();

			while (true)
			{
				try
				{
					failCount = 0;
					do
					{
						this.socket.send(new A2M_GET_SERVERS_BATCH2_Packet(regionCode, hostName + ":" + portNumber, filter));
						try
						{
							serverStringArray = ((M2A_SERVER_BATCH_Packet) this.socket.Reply).Servers;

							foreach (string serverString in serverStringArray)
							{
								hostName = serverString.Substring(0, serverString.LastIndexOf(":", StringComparison.Ordinal));
								portNumber = Convert.ToInt32(serverString.Substring(serverString.LastIndexOf(":", StringComparison.Ordinal) + 1));

								if (!hostName.Equals("0.0.0.0") && portNumber != 0)
								{
									serverSet.Add(new InetSocketAddress(hostName, portNumber));
								}
								else
								{
									finished = true;
								}
							}
							failCount = 0;
						}
						catch (TimeoutException e)
						{
							failCount++;
							if (failCount == retries)
							{
								throw e;
							}
							Console.WriteLine("Request to master server " + this.ipAddress + " timed out, retrying...");
						}
					} while (!finished);
					break;
				}
				catch (TimeoutException e)
				{
					if (force)
					{
						break;
					}
					else if (this.rotateIp())
					{
						throw e;
					}
					Console.WriteLine("Request to master server failed, retrying " + this.ipAddress + "...");
				}
			}

			return serverSet;
		}

		/// <summary>
		/// Initializes the socket to communicate with the master server
		/// </summary>
		/// <seealso cref= MasterServerSocket </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initSocket() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public override void initSocket()
		{
			this.socket = new MasterServerSocket(this.ipAddress, this.port);
		}

	}

}