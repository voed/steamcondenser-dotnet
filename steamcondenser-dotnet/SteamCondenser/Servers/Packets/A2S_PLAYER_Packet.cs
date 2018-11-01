/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets
{

	/// <summary>
	/// This packet class represents a A2S_PLAYER request send to a game server
	/// <para>
	/// It is used to request the list of players currently playing on the server.
	/// </para>
	/// <para>
	/// This packet type requires the client to challenge the server in advance,
	/// which is done automatically if required.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updatePlayers </seealso>
	public class A2S_PLAYER_Packet : SteamPacket
	{

		/// <summary>
		/// Creates a new A2S_PLAYER request object without a challenge number
		/// </summary>
		public A2S_PLAYER_Packet() : this(-1)
		{
		}

		/// <summary>
		/// Creates a new A2S_PLAYER request object including the challenge number
		/// </summary>
		/// <param name="challengeNumber"> The challenge number received from the server </param>
		public A2S_PLAYER_Packet(int challengeNumber) : base(SteamPacket.A2S_PLAYER_HEADER, Helper.byteArrayFromInteger(Integer.reverseBytes(challengeNumber)))
		{
		}

	}

}