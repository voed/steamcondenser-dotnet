using System;

/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets
{

	/// <summary>
	/// This module implements the basic functionality used by most of the packets
	/// used in communication with master, Source or GoldSrc servers.
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= SteamPacketFactory </seealso>
	public abstract class SteamPacket
	{

		public const sbyte A2S_INFO_HEADER = 0x54;
		public const sbyte S2A_INFO2_HEADER = 0x49;
		public const sbyte S2A_INFO_DETAILED_HEADER = 0x6D;
		public const sbyte A2S_PLAYER_HEADER = 0x55;
		public const sbyte S2A_PLAYER_HEADER = 0x44;
		public const sbyte A2S_RULES_HEADER = 0x56;
		public const sbyte S2A_RULES_HEADER = 0x45;
		public const sbyte A2S_SERVERQUERY_GETCHALLENGE_HEADER = 0x57;
		public const sbyte S2C_CHALLENGE_HEADER = 0x41;
		public const sbyte A2M_GET_SERVERS_BATCH2_HEADER = 0x31;
		public const sbyte M2A_SERVER_BATCH_HEADER = 0x66;
		public const sbyte RCON_GOLDSRC_CHALLENGE_HEADER = 0x63;
		public const sbyte RCON_GOLDSRC_NO_CHALLENGE_HEADER = 0x39;
		public const sbyte RCON_GOLDSRC_RESPONSE_HEADER = 0x6c;

		/// <summary>
		/// This variable stores the content of the package
		/// </summary>
		protected internal PacketBuffer contentData;

		/// <summary>
		/// This byte stores the type of the packet
		/// </summary>
		protected internal sbyte headerData;


		/// <summary>
		/// Creates a new packet object based on the given data
		/// </summary>
		/// <param name="headerData"> The packet header </param>
		protected internal SteamPacket(sbyte headerData) : this(headerData, new sbyte[0])
		{
		}

		/// <summary>
		/// Creates a new packet object based on the given data
		/// </summary>
		/// <param name="headerData"> The packet header </param>
		/// <param name="contentBytes"> The raw data of the packet </param>
		protected internal SteamPacket(sbyte headerData, sbyte[] contentBytes)
		{
			this.contentData = new PacketBuffer(contentBytes);
			this.headerData = headerData;
		}

		/// <summary>
		/// Returns the raw data representing this packet
		/// </summary>
		/// <returns> A byte array containing the raw data of this request packet </returns>
		public virtual sbyte[] Bytes
		{
			get
			{
				sbyte[] bytes = new sbyte[this.contentData.Length + 5];
				bytes[0] = unchecked((sbyte) 0xFF);
				bytes[1] = unchecked((sbyte) 0xFF);
				bytes[2] = unchecked((sbyte) 0xFF);
				bytes[3] = unchecked((sbyte) 0xFF);
				bytes[4] = this.headerData;
				Array.Copy(this.contentData.array(), 0, bytes, 5, bytes.Length - 5);
				return bytes;
			}
		}
	}

}