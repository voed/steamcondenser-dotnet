using System;
using System.Collections.Generic;
using System.Net;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.sockets
{

	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using SteamPacket = com.github.koraktor.steamcondenser.servers.packets.SteamPacket;
	using SteamPacketFactory = com.github.koraktor.steamcondenser.servers.packets.SteamPacketFactory;

	/// <summary>
	/// This class represents a socket used to communicate with game servers based
	/// on the Source engine (e.g. Team Fortress 2, Counter-Strike: Source)
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public class SourceSocket : QuerySocket
	{

		/// <summary>
		/// Creates a new socket to communicate with the server on the given IP
		/// address and port
		/// </summary>
		/// <param name="ipAddress"> Either the IP address or the DNS name of the server </param>
		/// <param name="portNumber"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if the socket cannot be opened </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SourceSocket(java.net.IPAddress ipAddress, int portNumber) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public SourceSocket(IPAddress ipAddress, int portNumber) : base(ipAddress, portNumber)
		{
		}

		/// <summary>
		/// Reads a packet from the socket
		/// <para>
		/// The Source query protocol specifies a maximum packet size of 1,400
		/// bytes. Bigger packets will be split over several UDP packets. This
		/// method reassembles split packets into single packet objects.
		/// Additionally Source may compress big packets using bzip2. Those packets
		/// will be compressed.
		/// 
		/// </para>
		/// </summary>
		/// <returns> SteamPacket The packet replied from the server </returns>
		/// <exception cref="SteamCondenserException"> if an error occurs while communicating
		///         with the server </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public com.github.koraktor.steamcondenser.servers.packets.SteamPacket getReply() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public override SteamPacket Reply
		{
			get
			{
				int bytesRead;
				bool isCompressed = false;
				SteamPacket packet;
    
				bytesRead = this.receivePacket(1400);
    
				if (this.packetIsSplit())
				{
					sbyte[] splitData;
					int packetCount, packetNumber, requestId, splitSize;
					int packetChecksum = 0;
					List<sbyte[]> splitPackets = new List<sbyte[]>();
    
					do
					{
						requestId = Integer.reverseBytes(this.buffer.Int);
						isCompressed = ((requestId & 0x80000000) != 0);
						packetCount = this.buffer.get();
						packetNumber = this.buffer.get() + 1;
    
						if (isCompressed)
						{
							splitSize = Integer.reverseBytes(this.buffer.Int);
							packetChecksum = Integer.reverseBytes(this.buffer.Int);
						}
						else
						{
							splitSize = Short.reverseBytes(this.buffer.Short);
						}
    
						splitData = new sbyte[Math.Min(splitSize, this.buffer.remaining())];
						this.buffer.get(splitData);
						splitPackets.Capacity = packetCount;
						splitPackets.Insert(packetNumber - 1, splitData);
    
						if (splitPackets.Count < packetCount)
						{
							try
							{
								bytesRead = this.receivePacket();
							}
							catch (TimeoutException)
							{
								bytesRead = 0;
							}
						}
						else
						{
							bytesRead = 0;
						}
    
						Console.WriteLine("Received packet #" + packetNumber + " of " + packetCount + " for request ID " + requestId + ".");
					} while (bytesRead > 0 && this.packetIsSplit());
    
					if (isCompressed)
					{
						packet = SteamPacketFactory.reassemblePacket(splitPackets, true, splitSize, packetChecksum);
					}
					else
					{
						packet = SteamPacketFactory.reassemblePacket(splitPackets);
					}
				}
				else
				{
					packet = this.PacketFromData;
				}
    
				this.buffer.flip();
    
				if (isCompressed)
				{
					Console.WriteLine("Received compressed reply of type \"" + packet.GetType().Name + "\"");
				}
				else
				{
					Console.WriteLine("Received reply of type \"" + packet.GetType().Name + "\"");
				}
    
				return packet;
			}
		}
	}

}