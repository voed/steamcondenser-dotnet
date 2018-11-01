using System.Collections.Generic;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.packets
{

	using PacketFormatException = com.github.koraktor.steamcondenser.exceptions.PacketFormatException;

	/// <summary>
	/// This packet class represents a M2A_SERVER_BATCH response replied by a master
	/// server
	/// <para>
	/// It contains a list of IP addresses and ports of game servers matching the
	/// requested criteria.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.MasterServer#getServers </seealso>
	public class M2A_SERVER_BATCH_Packet : SteamPacket
	{

		private List<string> serverArray;

		/// <summary>
		/// Creates a new M2A_SERVER_BATCH response object based on the given data
		/// </summary>
		/// <param name="data"> The raw packet data replied from the server </param>
		/// <exception cref="PacketFormatException"> if the packet data is not well formatted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public M2A_SERVER_BATCH_Packet(byte[] data) throws com.github.koraktor.steamcondenser.exceptions.PacketFormatException
		public M2A_SERVER_BATCH_Packet(sbyte[] data) : base(SteamPacket.M2A_SERVER_BATCH_HEADER, data)
		{

			if (this.contentData.Byte != 0x0A)
			{
				throw new PacketFormatException("Master query response is missing additional 0x0A byte.");
			}

			int firstOctet, secondOctet, thirdOctet, fourthOctet, portNumber;
			this.serverArray = new List<>();

			do
			{
				firstOctet = this.contentData.Byte & 0xFF;
				secondOctet = this.contentData.Byte & 0xFF;
				thirdOctet = this.contentData.Byte & 0xFF;
				fourthOctet = this.contentData.Byte & 0xFF;
				portNumber = this.contentData.Short & 0xFFFF;

				this.serverArray.Add(firstOctet + "." + secondOctet + "." + thirdOctet + "." + fourthOctet + ":" + portNumber);
			} while (this.contentData.remaining() > 0);
		}

		/// <summary>
		/// Returns the list of servers returned from the server in this packet
		/// </summary>
		/// <returns> An array of server addresses (i.e. IP addresses + port numbers) </returns>
		public virtual List<string> Servers
		{
			get
			{
				return this.serverArray;
			}
		}
	}

}