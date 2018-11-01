/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets.rcon
{
	/// <summary>
	/// This packet class represents a SERVERDATA_AUTH request sent to a Source
	/// server
	/// <para>
	/// It is used to authenticate the client for RCON communication.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.SourceServer#rconAuth </seealso>
	public class RCONAuthRequestPacket : RCONPacket
	{

		/// <summary>
		/// Creates a RCON authentication request for the given request ID and RCON
		/// password
		/// </summary>
		/// <param name="requestId"> The request ID of the RCON connection </param>
		/// <param name="rconPassword"> The RCON password of the server </param>
		public RCONAuthRequestPacket(int requestId, string rconPassword) : base(requestId, RCONPacket.SERVERDATA_AUTH, rconPassword)
		{
		}
	}

}