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
	/// This module implements methods to generate and access server information
	/// from S2A_INFO_DETAILED and S2A_INFO2 response packets
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= S2A_INFO_DETAILED_Packet </seealso>
	/// <seealso cref= S2A_INFO2_Packet </seealso>
	public abstract class S2A_INFO_BasePacket : SteamPacket
	{

		protected internal Dictionary<string, object> info;

		internal S2A_INFO_BasePacket(sbyte headerByte, sbyte[] dataBytes) : base(headerByte, dataBytes)
		{

			this.info = new Dictionary<>();
		}

		/// <summary>
		/// Returns a generated array of server properties from the instance
		/// variables of the packet object
		/// </summary>
		/// <returns> The information provided by the server </returns>
		public virtual Dictionary<string, object> Info
		{
			get
			{
				return this.info;
			}
		}

	}

}