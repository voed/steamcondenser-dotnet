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
	/// This packet class represents a RCON request packet sent to a GoldSrc server
	/// <para>
	/// It is used to request a command execution on the server.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GoldSrcServer#rconExec </seealso>
	public class RCONGoldSrcRequestPacket : SteamPacket
	{
		/// <summary>
		/// Creates a request for the given request string
		/// <para>
		/// The request string has the form <code>rcon {challenge number} {RCON
		/// password} {command}</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="request"> The request string to send to the server </param>
		public RCONGoldSrcRequestPacket(string request) : base((sbyte) 0, request.GetBytes())
		{
		}

		/// <summary>
		/// Returns the raw data representing this packet
		/// </summary>
		/// <returns> A byte array containing the raw data of this request packet </returns>
		public override sbyte[] Bytes
		{
			get
			{
				sbyte[] bytes = new sbyte[this.contentData.Length + 4];
    
				Array.Copy(Helper.byteArrayFromInteger(unchecked((int)0xFFFFFFFF)), 0, bytes, 0, 4);
				Array.Copy(this.contentData.array(), 0, bytes, 4, this.contentData.Length);
    
				return bytes;
			}
		}
	}

}