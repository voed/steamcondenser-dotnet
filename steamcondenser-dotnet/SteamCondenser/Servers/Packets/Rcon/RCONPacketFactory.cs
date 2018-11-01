/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.packets.rcon
{
	using PacketFormatException = com.github.koraktor.steamcondenser.exceptions.PacketFormatException;

	/// <summary>
	/// This module provides functionality to handle raw packet data for Source RCON
	/// 
	/// It's is used to transform data bytes into packet objects for RCON
	/// communication with Source servers.
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= RCONPacket </seealso>
	public abstract class RCONPacketFactory
	{

		/// <summary>
		/// Creates a new packet object based on the header byte of the given raw
		/// data
		/// </summary>
		/// <param name="rawData"> The raw data of the packet </param>
		/// <returns> RCONPacket The packet object generated from the packet data </returns>
		/// <exception cref="PacketFormatException"> if the packet header is not recognized </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RCONPacket getPacketFromData(byte[] rawData) throws com.github.koraktor.steamcondenser.exceptions.PacketFormatException
		public static RCONPacket getPacketFromData(sbyte[] rawData)
		{
			PacketBuffer packetBuffer = new PacketBuffer(rawData);

			int requestId = Integer.reverseBytes(packetBuffer.Int);
			int header = Integer.reverseBytes(packetBuffer.Int);
			string data = packetBuffer.String;

			switch (header)
			{
				case RCONPacket.SERVERDATA_AUTH_RESPONSE:
					return new RCONAuthResponse(requestId);
				case RCONPacket.SERVERDATA_RESPONSE_VALUE:
					return new RCONExecResponsePacket(requestId, data);
				default:
					throw new PacketFormatException("Unknown packet with header " + header + " received.");
			}
		}
	}

}