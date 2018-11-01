/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets.rcon
{
	/// <summary>
	/// This packet class represents a SERVERDATA_AUTH_RESPONSE packet sent by a
	/// Source server
	/// <para>
	/// It is used to indicate the success or failure of an authentication attempt
	/// of a client for RCON communication.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.SourceServer#rconAuth </seealso>
	public class RCONAuthResponse : RCONPacket
	{

		/// <summary>
		/// Creates a RCON authentication response for the given request ID
		/// <para>
		/// The request ID of the packet will match the client's request if
		/// authentication was successful
		/// 
		/// </para>
		/// </summary>
		/// <param name="requestId"> The request ID of the RCON connection </param>
		public RCONAuthResponse(int requestId) : base(requestId, RCONPacket.SERVERDATA_AUTH_RESPONSE, "")
		{
		}
	}

}