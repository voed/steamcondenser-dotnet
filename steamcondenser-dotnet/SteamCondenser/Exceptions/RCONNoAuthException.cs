/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.exceptions
{
	/// <summary>
	/// This exception class indicates that you have not authenticated yet with the
	/// game server you're trying to send commands via RCON
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#rconAuth </seealso>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#rconExec </seealso>
	public class RCONNoAuthException : SteamCondenserException
	{

		/// <summary>
		/// Creates a new <code>RCONNoAuthException</code> instance
		/// </summary>
		public RCONNoAuthException() : base("Not authenticated yet.")
		{
		}
	}

}