using System;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.packets
{

	/// <summary>
	/// This packet class represents a A2M_GET_SERVERS_BATCH2 request sent to a
	/// master server
	/// <para>
	/// It is used to receive a list of game servers matching the specified filters.
	/// </para>
	/// <para>
	/// Filtering:
	/// </para>
	/// <para>
	/// Instead of filtering the results sent by the master server locally, you
	/// should at least use the following filters to narrow down the results sent by
	/// the master server. Receiving all servers from the master server is taking
	/// quite some time.
	/// </para>
	/// <para>
	/// Available filters:
	/// <ul>
	///  <li>\type\d: Request only dedicated servers</li>
	///  <li>\secure\1: Request only secure servers</li>
	///  <li>\gamedir\[mod]: Request only servers of a specific mod</li>
	///  <li>\map\[mapname]: Request only servers running a specific map</li>
	///  <li>\linux\1: Request only linux servers</li>
	///  <li>\emtpy\1: Request only <b>non</b>-empty servers</li>
	///  <li>\full\1: Request only servers <b>not</b> full</li>
	///  <li>\proxy\1: Request only spectator proxy servers</li>
	/// </ul>
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	/// <seealso cref= MasterServer#getServers(byte, String) </seealso>
	public class A2M_GET_SERVERS_BATCH2_Packet : SteamPacket
	{

		private string filter;
		private sbyte regionCode;
		private string startIp;

		/// <summary>
		/// Creates a new A2M_GET_SERVERS_BATCH2 request object, filtering by the
		/// given paramters
		/// </summary>
		/// <param name="regionCode"> The region code to filter servers by region. </param>
		/// <param name="startIp"> This should be the last IP received from the master
		///        server or 0.0.0.0 </param>
		/// <param name="filter"> The filters to apply in the form ("\filtername\value...") </param>
		public A2M_GET_SERVERS_BATCH2_Packet(sbyte regionCode, string startIp, string filter) : base(SteamPacket.A2M_GET_SERVERS_BATCH2_HEADER)
		{

			this.filter = filter;
			this.regionCode = regionCode;
			this.startIp = startIp;
		}

		/// <summary>
		/// Returns the raw data representing this packet
		/// </summary>
		/// <returns> A byte array containing the raw data of this request packet </returns>
		public override sbyte[] Bytes
		{
			get
			{
				sbyte[] bytes, filterBytes, startIpBytes;
    
				filterBytes = (this.filter + "\0").GetBytes();
				startIpBytes = (this.startIp + "\0").GetBytes();
				bytes = new sbyte[2 + startIpBytes.Length + filterBytes.Length];
    
				bytes[0] = this.headerData;
				bytes[1] = this.regionCode;
				Array.Copy(startIpBytes, 0, bytes, 2, startIpBytes.Length);
				Array.Copy(filterBytes, 0, bytes, startIpBytes.Length + 2, filterBytes.Length);
    
				return bytes;
			}
		}
	}

}