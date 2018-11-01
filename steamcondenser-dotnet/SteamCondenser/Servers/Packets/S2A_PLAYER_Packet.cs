using System.Collections.Generic;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.packets
{

	using PacketFormatException = com.github.koraktor.steamcondenser.exceptions.PacketFormatException;

	/// <summary>
	/// This class represents a S2A_PLAYER response sent by a game server
	/// <para>
	/// It is used to transfer a list of players currently playing on the server.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updatePlayers </seealso>
	public class S2A_PLAYER_Packet : SteamPacket
	{

		private Dictionary<string, SteamPlayer> playerHash;

		/// <summary>
		/// Creates a new S2A_PLAYER response object based on the given data
		/// </summary>
		/// <param name="dataBytes"> The raw packet data sent by the server </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public S2A_PLAYER_Packet(byte[] dataBytes) throws com.github.koraktor.steamcondenser.exceptions.PacketFormatException
		public S2A_PLAYER_Packet(sbyte[] dataBytes) : base(SteamPacket.S2A_PLAYER_HEADER, dataBytes)
		{

			if (this.contentData.Length == 0)
			{
				throw new PacketFormatException("Wrong formatted S2A_PLAYER response packet.");
			}

			this.playerHash = new Dictionary<>(this.contentData.Byte);

			while (this.contentData.hasRemaining())
			{
				int playerId = this.contentData.Byte & 0xff;
				string playerName = this.contentData.String;
				this.playerHash[playerName] = new SteamPlayer(playerId, playerName, Integer.reverseBytes(this.contentData.Int), Float.intBitsToFloat(Integer.reverseBytes(this.contentData.Int)));
			}
		}

		/// <summary>
		/// Returns the list of active players provided by the server
		/// </summary>
		/// <returns> All active players on the server </returns>
		public virtual Dictionary<string, SteamPlayer> PlayerHash
		{
			get
			{
				return this.playerHash;
			}
		}
	}

}