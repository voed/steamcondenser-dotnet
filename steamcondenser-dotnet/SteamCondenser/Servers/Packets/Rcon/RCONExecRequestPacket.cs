/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets.rcon
{
	/// <summary>
	/// This packet class represents a SERVERDATA_EXECCOMMAND packet sent to a
	/// Source server
	/// <para>
	/// It is used to request a command execution on the server.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.SourceServer#rconExec </seealso>
	public class RCONExecRequestPacket : RCONPacket
	{

		/// <summary>
		/// Creates a RCON command execution request for the given request ID and
		/// command
		/// </summary>
		/// <param name="requestId"> The request ID of the RCON connection </param>
		/// <param name="rconCommand"> The command to execute on the server </param>
		public RCONExecRequestPacket(int requestId, string rconCommand) : base(requestId, RCONPacket.SERVERDATA_EXECCOMMAND, rconCommand)
		{
		}
	}

}