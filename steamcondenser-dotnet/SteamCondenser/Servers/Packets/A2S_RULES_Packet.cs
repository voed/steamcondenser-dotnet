/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets
{

	/// <summary>
	/// This packet class represents a A2S_RULES request send to a game server
	/// <para>
	/// The game server will return a list of currently active game rules, e.g.
	/// <code>mp_friendlyfire = 1</code>.
	/// </para>
	/// <para>
	/// This packet type requires the client to challenge the server in advance,
	/// which is done automatically if required.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updateRules </seealso>
	public class A2S_RULES_Packet : SteamPacket
	{

		/// <summary>
		/// Creates a new A2S_RULES request object including the challenge number
		/// </summary>
		/// <param name="challengeNumber"> The challenge number received from the server </param>
		public A2S_RULES_Packet(int challengeNumber) : base(SteamPacket.A2S_RULES_HEADER, Helper.byteArrayFromInteger(Integer.reverseBytes(challengeNumber)))
		{
		}
	}

}