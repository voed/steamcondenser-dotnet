/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets
{
	/// <summary>
	/// This class represents a S2A_INFO_DETAILED response packet sent by a Source
	/// or GoldSrc server
	/// <para>
	/// Out-of-date (before 10/24/2008) GoldSrc servers use an older format (see
	/// <seealso cref="S2A_INFO_DETAILED_Packet"/>).
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updateServerInfo </seealso>
	public class S2A_INFO2_Packet : S2A_INFO_BasePacket
	{

		private static sbyte EDF_GAME_ID = (sbyte) 0x01;
		private static sbyte EDF_GAME_PORT = unchecked((sbyte) 0x80);
		private static sbyte EDF_SERVER_ID = (sbyte) 0x10;
		private static sbyte EDF_SERVER_TAGS = (sbyte) 0x20;
		private static sbyte EDF_SOURCE_TV = (sbyte) 0x40;

		/// <summary>
		/// Creates a new S2A_INFO2 response object based on the given data
		/// </summary>
		/// <param name="dataBytes"> The raw packet data replied from the server </param>
		public S2A_INFO2_Packet(sbyte[] dataBytes) : base(SteamPacket.S2A_INFO2_HEADER, dataBytes)
		{

			this.info["networkVersion"] = this.contentData.Byte;
			this.info["serverName"] = this.contentData.String;
			this.info["mapName"] = this.contentData.String;
			this.info["gameDir"] = this.contentData.String;
			this.info["gameDescription"] = this.contentData.String;
			this.info["appId"] = Short.reverseBytes(this.contentData.Short);
			this.info["numberOfPlayers"] = this.contentData.Byte;
			this.info["maxPlayers"] = this.contentData.Byte;
			this.info["numberOfBots"] = this.contentData.Byte;
			this.info["dedicated"] = this.contentData.Byte;
			this.info["operatingSystem"] = this.contentData.Byte;
			this.info["passwordProtected"] = this.contentData.Byte == 1;
			this.info["secure"] = this.contentData.Byte == 1;
			this.info["gameVersion"] = this.contentData.String;

			if (this.contentData.remaining() > 0)
			{
				sbyte extraDataFlag = this.contentData.Byte;

				if ((extraDataFlag & EDF_GAME_PORT) != 0)
				{
					this.info["serverPort"] = Short.reverseBytes(this.contentData.Short);
				}

				if ((extraDataFlag & EDF_SERVER_ID) != 0)
				{
					this.info["serverId"] = Long.reverseBytes((this.contentData.Int << 32) | this.contentData.Int);
				}

				if ((extraDataFlag & EDF_SOURCE_TV) != 0)
				{
					this.info["tvPort"] = Short.reverseBytes(this.contentData.Short);
					this.info["tvName"] = this.contentData.String;
				}

				if ((extraDataFlag & EDF_SERVER_TAGS) != 0)
				{
					this.info["serverTags"] = this.contentData.String;
				}

				if ((extraDataFlag & EDF_GAME_ID) != 0)
				{
					this.info["gameId"] = Long.reverseBytes((this.contentData.Int << 32) | this.contentData.Int);
				}
			}
		}

	}

}