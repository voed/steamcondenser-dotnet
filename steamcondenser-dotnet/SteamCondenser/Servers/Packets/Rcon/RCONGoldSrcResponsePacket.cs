/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets.rcon
{

	/// <summary>
	/// This packet class represents a RCON response packet sent by a GoldSrc server
	/// <para>
	/// It is used to transport the output of a command from the server to the
	/// client which requested the command execution.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GoldSrcServer#rconExec </seealso>
	public class RCONGoldSrcResponsePacket : SteamPacket
	{

		/// <summary>
		/// Creates a RCON command response for the given command output
		/// </summary>
		/// <param name="commandResponse"> The output of the command executed on the server </param>
		public RCONGoldSrcResponsePacket(sbyte[] commandResponse) : base(SteamPacket.RCON_GOLDSRC_RESPONSE_HEADER, commandResponse)
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
				return this.contentData.String;
			}
		}
	}

}