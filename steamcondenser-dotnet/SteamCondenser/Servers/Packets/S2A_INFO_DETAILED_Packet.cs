using System.Collections.Generic;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.packets
{

	/// <summary>
	/// This class represents a S2A_INFO_DETAILED response packet sent by a GoldSrc
	/// server
	/// 
	/// @author Sebastian Staudt </summary>
	/// @deprecated Only outdated GoldSrc servers (before 10/24/2008) use this
	///             format. Newer ones use the same format as Source servers now
	///             (see <seealso cref="S2A_INFO2_Packet"/>). 
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updateServerInfo </seealso>
	public class S2A_INFO_DETAILED_Packet : S2A_INFO_BasePacket
	{

		/// <summary>
		/// Creates a new S2A_INFO_DETAILED response object based on the given data
		/// </summary>
		/// <param name="dataBytes"> The raw packet data replied from the server </param>
		public S2A_INFO_DETAILED_Packet(sbyte[] dataBytes) : base(SteamPacket.S2A_INFO_DETAILED_HEADER, dataBytes)
		{

			this.info["serverIp"] = this.contentData.String;
			this.info["serverName"] = this.contentData.String;
			this.info["mapName"] = this.contentData.String;
			this.info["gameDir"] = this.contentData.String;
			this.info["gameDescription"] = this.contentData.String;
			this.info["numberOfPlayers"] = this.contentData.Byte;
			this.info["maxPlayers"] = this.contentData.Byte;
			this.info["networkVersion"] = this.contentData.Byte;
			this.info["dedicated"] = this.contentData.Byte;
			this.info["operatingSystem"] = this.contentData.Byte;
			this.info["passwordProtected"] = this.contentData.Byte == 1;
			bool isMod = this.contentData.Byte == 1;
			this.info["isMod"] = isMod;

			if (isMod)
			{
				Dictionary<string, object> modInfo = new Dictionary<string, object>(6);
				modInfo["urlInfo"] = this.contentData.String;
				modInfo["urlDl"] = this.contentData.String;
				this.contentData.Byte;
				if (this.contentData.remaining() == 12)
				{
					modInfo["modVersion"] = Integer.reverseBytes(this.contentData.Int);
					modInfo["modSize"] = Integer.reverseBytes(this.contentData.Int);
					modInfo["svOnly"] = this.contentData.Byte == 1;
					modInfo["clDll"] = this.contentData.Byte == 1;
					this.info["secure"] = this.contentData.Byte == 1;
					this.info["numberOfBots"] = this.contentData.Byte;
				}
				this.info["modInfo"] = modInfo;
			}
			else
			{
				this.info["secure"] = this.contentData.Byte == 1;
				this.info["numberOfBots"] = this.contentData.Byte;
			}
		}

	}

}