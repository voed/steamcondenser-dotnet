using System;

/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets.rcon
{

	/// <summary>
	/// This module is included by all classes representing a packet used by
	/// Source's RCON protocol
	/// <para>
	/// It provides a basic implementation for initializing and serializing such a
	/// packet.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= RCONPacketFactory </seealso>
	public abstract class RCONPacket : SteamPacket
	{

		/// <summary>
		/// Header for authentication requests
		/// </summary>
		public const sbyte SERVERDATA_AUTH = 3;

		/// <summary>
		/// Header for replies to authentication attempts
		/// </summary>
		public const sbyte SERVERDATA_AUTH_RESPONSE = 2;

		/// <summary>
		/// Header for command execution requests
		/// </summary>
		public const sbyte SERVERDATA_EXECCOMMAND = 2;

		/// <summary>
		/// Header for packets with the output of a command execution
		/// </summary>
		public const sbyte SERVERDATA_RESPONSE_VALUE = 0;

		/// <summary>
		/// The packet header specifying the packet type
		/// </summary>
		protected internal int header;

		/// <summary>
		/// The request ID used to identify the RCON communication
		/// </summary>
		protected internal int requestId;

		/// <summary>
		/// Creates a new RCON packet object with the given request ID, type and
		/// content data
		/// </summary>
		/// <param name="requestId"> The request ID for the current RCON communication </param>
		/// <param name="rconHeader"> The header for the packet type </param>
		/// <param name="rconData"> The raw packet data </param>
		protected internal RCONPacket(int requestId, int rconHeader, string rconData) : base((sbyte) 0, (rconData + "\0\0").GetBytes())
		{

			this.header = rconHeader;
			this.requestId = requestId;
		}

		/// <summary>
		/// Returns the raw data representing this packet
		/// </summary>
		/// <returns> A byte array containing the raw data of this RCON packet </returns>
		public override sbyte[] Bytes
		{
			get
			{
				sbyte[] bytes = new sbyte[this.contentData.Length + 12];
    
				Array.Copy(Helper.byteArrayFromInteger(Integer.reverseBytes(bytes.Length - 4)), 0, bytes, 0, 4);
				Array.Copy(Helper.byteArrayFromInteger(Integer.reverseBytes(this.requestId)), 0, bytes, 4, 4);
				Array.Copy(Helper.byteArrayFromInteger(Integer.reverseBytes(this.header)), 0, bytes, 8, 4);
				Array.Copy(this.contentData.array(), 0, bytes, 12, bytes.Length - 12);
    
				return bytes;
			}
		}

		/// <summary>
		/// Returns the request ID used to identify the RCON communication
		/// </summary>
		/// <returns> The request ID used to identify the RCON communication </returns>
		public virtual int RequestId
		{
			get
			{
				return this.requestId;
			}
		}

	}

}