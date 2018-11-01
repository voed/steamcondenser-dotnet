using System.Collections.Generic;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.packets
{

	using PacketFormatException = com.github.koraktor.steamcondenser.exceptions.PacketFormatException;

	/// <summary>
	/// This class represents a S2A_RULES response sent by a game server
	/// <para>
	/// It is used to transfer a list of server rules (a.k.a. CVARs) with their
	/// active values.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.servers.GameServer#updateRules </seealso>
	public class S2A_RULES_Packet : SteamPacket
	{

		private Dictionary<string, string> rulesHash;

		/// <summary>
		/// Creates a new S2A_RULES response object based on the given data
		/// </summary>
		/// <param name="dataBytes"> The raw packet data sent by the server </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public S2A_RULES_Packet(byte[] dataBytes) throws com.github.koraktor.steamcondenser.exceptions.PacketFormatException
		public S2A_RULES_Packet(sbyte[] dataBytes) : base(SteamPacket.S2A_RULES_HEADER, dataBytes)
		{

			if (this.contentData.Length == 0)
			{
				throw new PacketFormatException("Wrong formatted S2A_RULES response packet.");
			}

			int rulesCount = Short.reverseBytes(this.contentData.Short);
			this.rulesHash = new Dictionary<>(rulesCount);

			string rule;
			string value;
			for (int i = 0; i < rulesCount; i++)
			{
				rule = this.contentData.String;
				value = this.contentData.String;

				if (rule.Equals(""))
				{
					break;
				}

				this.rulesHash[rule] = value;
			}
		}

		/// <summary>
		/// Returns the list of server rules (a.k.a. CVars) with the current values
		/// </summary>
		/// <returns> array A list of server rules </returns>
		public virtual Dictionary<string, string> RulesHash
		{
			get
			{
				return this.rulesHash;
			}
		}
	}

}