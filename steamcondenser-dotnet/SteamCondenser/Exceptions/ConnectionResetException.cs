/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.exceptions
{
	/// <summary>
	/// Indicates that a connection has been reset by the peer
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public class ConnectionResetException : SteamCondenserException
	{

		public ConnectionResetException() : base("Connection reset by peer")
		{
		}

	}

}