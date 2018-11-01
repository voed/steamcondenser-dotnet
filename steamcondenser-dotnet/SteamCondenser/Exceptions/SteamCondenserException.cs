using System;

/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.exceptions
{
	/// <summary>
	/// This exception class is used as a base class for all exceptions related to
	/// Steam Condenser's operation
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public class SteamCondenserException : Exception
	{

		/// <summary>
		/// Creates a new <code>SteamCondenserException</code> instance
		/// </summary>
		public SteamCondenserException()
		{
		}

		/// <summary>
		/// Creates a new <code>SteamCondenserException</code> instance
		/// </summary>
		/// <param name="message"> The message to attach to the exception </param>
		public SteamCondenserException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a new <code>SteamCondenserException</code> instance
		/// </summary>
		/// <param name="message"> The message to attach to the exception </param>
		/// <param name="cause"> The initial error that caused this exception </param>
		public SteamCondenserException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}