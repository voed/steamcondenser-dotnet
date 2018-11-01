/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2009-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.exceptions
{
	/// <summary>
	/// This exception class indicates that the IP address your accessing the game
	/// server from has been banned by the server
	/// <para>
	/// You or the server operator will have to unban your IP address on the server.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#rconAuth </seealso>
	public class RCONBanException : SteamCondenserException
	{

		/// <summary>
		/// Creates a new <code>RCONBanException</code> instance
		/// </summary>
		public RCONBanException() : base("You have been banned from this server.")
		{
		}
	}

}