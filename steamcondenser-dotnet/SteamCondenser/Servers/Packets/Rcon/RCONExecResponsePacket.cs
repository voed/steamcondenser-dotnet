/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets.rcon
{
	/// <summary>
	/// This packet class represents a SERVERDATA_RESPONSE_VALUE packet sent by a
	/// Source server
	/// <para>
	/// It is used to transport the output of a command from the server to the
	/// client which requested the command execution.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.SourceServer#rconExec </seealso>
	public class RCONExecResponsePacket : RCONPacket
	{

		/// <summary>
		/// Creates a RCON command response for the given request ID and command
		/// output
		/// </summary>
		/// <param name="requestId"> The request ID of the RCON connection </param>
		/// <param name="commandReturn"> The output of the command executed on the server </param>
		public RCONExecResponsePacket(int requestId, string commandReturn) : base(requestId, RCONPacket.SERVERDATA_RESPONSE_VALUE, commandReturn)
		{
		}

		/// <summary>
		/// Returns the output of the command execution
		/// </summary>
		/// <returns> The output of the command </returns>
		public virtual string Response
		{
			get
			{
				string response = StringHelper.NewString(this.contentData.array());
				return response.Substring(0, response.Length - 2);
			}
		}
	}

}