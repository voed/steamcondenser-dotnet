/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.exceptions
{
	/// <summary>
	/// This exception class indicates a problem when parsing packet data from the
	/// responses received from a game or master server
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public class PacketFormatException : SteamCondenserException
	{

		/// <summary>
		/// Creates a new <code>PacketFormatException</code> instance
		/// </summary>
		/// <param name="message"> The message to attach to the exception </param>
		public PacketFormatException(string message) : base(message)
		{
		}
	}

}