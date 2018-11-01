/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>
using System.Net;

namespace com.github.koraktor.steamcondenser.servers.sockets
{
	using PacketFormatException = com.github.koraktor.steamcondenser.exceptions.PacketFormatException;
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using SteamPacket = com.github.koraktor.steamcondenser.servers.packets.SteamPacket;

	/// <summary>
	/// This class represents a socket used to communicate with master servers
	/// 
	/// @author     Sebastian Staudt
	/// </summary>
	public class MasterServerSocket : QuerySocket
	{

		/// <summary>
		/// Creates a new socket to communicate with the server on the given IP
		/// address and port
		/// </summary>
		/// <param name="ipAddress"> Either the IP address or the DNS name of the server </param>
		/// <param name="portNumber"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if the socket cannot be opened </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MasterServerSocket(java.net.IPAddress ipAddress, int portNumber) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public MasterServerSocket(IPAddress ipAddress, int portNumber) : base(ipAddress, portNumber)
		{
		}

		/// <summary>
		/// Reads a single packet from the socket
		/// </summary>
		/// <returns> The packet replied from the server </returns>
		/// <exception cref="SteamCondenserException"> if an error occurs while communicating
		///         with the server </exception>
		/// <exception cref="PacketFormatException"> if the packet has the wrong format </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public com.github.koraktor.steamcondenser.servers.packets.SteamPacket getReply() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public override SteamPacket Reply
		{
			get
			{
				this.receivePacket(1500);
    
				if (this.buffer.Int != -1)
				{
					throw new PacketFormatException("Master query response has wrong packet header.");
				}
    
				SteamPacket packet = this.PacketFromData;
    
				Console.WriteLine("Received reply of type \"" + packet.GetType().Name + "\"");
    
				return packet;
			}
		}

	}

}