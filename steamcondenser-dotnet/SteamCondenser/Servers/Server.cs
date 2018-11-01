using System.Collections.Generic;
using System.Net;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2011-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers
{

	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;

	/// <summary>
	/// This class is subclassed by all classes implementing server functionality
	/// <p/>
	/// It provides basic name resolution features and the ability to rotate
	/// between different IP addresses belonging to a single DNS name.
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public abstract class Server
	{

		protected internal List<string> hostNames;

		protected internal IPAddress ipAddress;

		protected internal List<IPAddress> ipAddresses;

		protected internal int ipIndex;

		protected internal int port;

		/// <summary>
		/// Creates a new server instance with the given address and port
		/// </summary>
		/// <param name="address"> Either an IP address, a DNS name or one of them combined
		///        with the port number. If a port number is given, e.g.
		///        'server.example.com:27016' it will override the second argument. </param>
		/// <param name="port"> The port the server is listening on </param>
		/// <seealso cref= #initSocket </seealso>
		/// <exception cref="SteamCondenserException"> if an host name cannot be resolved </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Server(Object address, System.Nullable<int> port) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		protected internal Server(object address, int? port)
		{
			this.hostNames = new List<string>();
			this.ipAddresses = new List<IPAddress>();
			this.ipIndex = 0;

			if (address is string)
			{
				if (((string) address).IndexOf(':') >= 0)
				{
					string[] tmpAddress = ((string) address).Split(":", 2);
					port = int.Parse(tmpAddress[1]);
					address = tmpAddress[0];
				}
				if (port == null)
				{
					port = 27015;
				}

				try
				{
					foreach (IPAddress ipAddress in IPAddress.getAllByName((string) address))
					{
						this.hostNames.Add(ipAddress.HostName);
						this.ipAddresses.Add(ipAddress);
					}
				}
				catch (UnknownHostException e)
				{
					throw new SteamCondenserException("Cannot resolve " + address + ": " + e.Message);
				}
			}
			else if (address is IPAddress)
			{
				this.hostNames.Add(((IPAddress) address).HostName);
				this.ipAddresses.Add((IPAddress) address);
			}

			if (port == null)
			{
				throw new System.ArgumentException("No port given");
			}

			this.ipAddress = this.ipAddresses[0];
			this.port = port.Value;

			this.initSocket();
		}

		/// <summary>
		/// Disconnect the connections to this server
		/// <para>
		/// <em><strong>Note:</strong>
		/// In the base implementation this does nothing, only connection-based
		/// communication channels have to be disconnected.</em>
		/// </para>
		/// </summary>
		public virtual void disconnect()
		{
		}

		/// <summary>
		/// Returns a list of host names associated with this server
		/// </summary>
		/// <returns> The host names of this server </returns>
		public virtual IList<string> HostNames
		{
			get
			{
				return this.hostNames;
			}
		}

		/// <summary>
		/// Returns a list of IP addresses associated with this server
		/// </summary>
		/// <returns> The IP addresses of this server </returns>
		public virtual IList<IPAddress> IpAddresses
		{
			get
			{
				return this.ipAddresses;
			}
		}

		/// <summary>
		/// Rotate this server's IP address to the next one in the IP list
		/// <p/>
		/// If this method returns <code>true</code>, it indicates that all IP
		/// addresses have been used, hinting at the server(s) being unreachable. An
		/// appropriate action should be taken to inform the user.
		/// <p/>
		/// Servers with only one IP address will always cause this method to return
		/// <code>true</code> and the sockets will not be reinitialized.
		/// <p/> </summary>
		/// <returns> bool <code>true</code>, if the IP list reached its end. If the
		///         list contains only one IP address, this method will instantly
		///         return <code>true</code> </returns>
		/// <seealso cref= #initSocket </seealso>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean rotateIp() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public virtual bool rotateIp()
		{
			if (this.ipAddresses.Count == 1)
			{
				return true;
			}

			this.ipIndex = (this.ipIndex + 1) % this.ipAddresses.Count;
			this.ipAddress = this.ipAddresses[this.ipIndex];

			this.initSocket();

			return this.ipIndex == 0;
		}

		/// <summary>
		/// Disconnects the connections to this server
		/// </summary>
		/// <seealso cref= #disconnect </seealso>
		~Server()
		{
			this.disconnect();
		}

		/// <summary>
		/// Initializes the socket(s) to communicate with the server
		/// <p/>
		/// Must be implemented in subclasses to prepare sockets for server
		/// communication
		/// </summary>
		/// <exception cref="SteamCondenserException"> if initializing the socket fails </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void initSocket() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
		protected internal abstract void initSocket();

	}

}