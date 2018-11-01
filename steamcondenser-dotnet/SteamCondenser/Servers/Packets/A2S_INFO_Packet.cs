/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets
{
	/// <summary>
	/// The A2S_INFO_Packet class represents a A2S_INFO request send to the server
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updateServerInfo </seealso>
	public class A2S_INFO_Packet : SteamPacket
	{

		/// <summary>
		/// Creates a new A2S_INFO request object
		/// </summary>
		public A2S_INFO_Packet() : base(SteamPacket.A2S_INFO_HEADER, "Source Engine Query\0".GetBytes())
		{
		}
	}

}