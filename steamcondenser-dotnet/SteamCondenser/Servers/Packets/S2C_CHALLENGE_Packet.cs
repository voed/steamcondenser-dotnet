/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets
{
	/// <summary>
	/// This packet class represents a S2C_CHALLENGE response replied by a game
	/// server
	/// <para>
	/// It is used to provide a challenge number to a client requesting information
	/// from the game server.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updateChallengeNumber </seealso>
	public class S2C_CHALLENGE_Packet : SteamPacket
	{

		/// <summary>
		/// Creates a new S2C_CHALLENGE response object based on the given data
		/// </summary>
		/// <param name="challengeNumberBytes"> The raw packet data replied from the server </param>
		public S2C_CHALLENGE_Packet(sbyte[] challengeNumberBytes) : base(SteamPacket.S2C_CHALLENGE_HEADER, challengeNumberBytes)
		{
		}

		/// <summary>
		/// Returns the challenge number received from the game server
		/// </summary>
		/// <returns> The challenge number provided by the game server </returns>
		public virtual int ChallengeNumber
		{
			get
			{
				return Integer.reverseBytes(this.contentData.Int);
			}
		}
	}

}