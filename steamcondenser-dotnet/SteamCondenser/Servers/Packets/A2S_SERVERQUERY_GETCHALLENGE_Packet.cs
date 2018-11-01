/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets
{
	/// <summary>
	/// This packet class represents a A2S_SERVERQUERY_GETCHALLENGE request send to
	/// a game server
	/// <para>
	/// It is used to retrieve a challenge number from the game server, which helps
	/// to identify the requesting client.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updateChallengeNumber </seealso>
	public class A2S_SERVERQUERY_GETCHALLENGE_Packet : SteamPacket
	{

		/// <summary>
		/// Creates a new A2S_SERVERQUERY_GETCHALLENGE request object
		/// </summary>
		public A2S_SERVERQUERY_GETCHALLENGE_Packet() : base(SteamPacket.A2S_SERVERQUERY_GETCHALLENGE_HEADER)
		{
		}
	}

}