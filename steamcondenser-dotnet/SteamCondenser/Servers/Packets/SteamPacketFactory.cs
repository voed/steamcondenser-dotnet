using System;
using System.Collections.Generic;
using System.IO;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.packets
{

	using BZip2CompressorInputStream = org.apache.commons.compress.compressors.bzip2.BZip2CompressorInputStream;

	using PacketFormatException = com.github.koraktor.steamcondenser.exceptions.PacketFormatException;
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using RCONGoldSrcResponsePacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONGoldSrcResponsePacket;

	/// <summary>
	/// This module provides functionality to handle raw packet data, including data
	/// split into several UDP / TCP packets and BZIP2 compressed data. It's the
	/// main utility to transform data bytes into packet objects.
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= SteamPacket </seealso>
	public abstract class SteamPacketFactory
	{

		/// <summary>
		/// Creates a new packet object based on the header byte of the given raw
		/// data
		/// </summary>
		/// <param name="rawData"> The raw data of the packet </param>
		/// <exception cref="PacketFormatException"> if the packet header is not recognized </exception>
		/// <returns> The packet object generated from the packet data </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SteamPacket getPacketFromData(byte[] rawData) throws com.github.koraktor.steamcondenser.exceptions.PacketFormatException
		public static SteamPacket getPacketFromData(sbyte[] rawData)
		{
			sbyte header = rawData[0];
			sbyte[] data = new sbyte[rawData.Length - 1];
			Array.Copy(rawData, 1, data, 0, rawData.Length - 1);

			switch (header)
			{
				case SteamPacket.A2S_INFO_HEADER:
					return new A2S_INFO_Packet();

				case SteamPacket.S2A_INFO_DETAILED_HEADER:
					return new S2A_INFO_DETAILED_Packet(data);

				case SteamPacket.S2A_INFO2_HEADER:
					return new S2A_INFO2_Packet(data);

				case SteamPacket.A2S_PLAYER_HEADER:
					return new A2S_PLAYER_Packet(Helper.integerFromByteArray(data));

				case SteamPacket.S2A_PLAYER_HEADER:
					return new S2A_PLAYER_Packet(data);

				case SteamPacket.A2S_RULES_HEADER:
					return new A2S_RULES_Packet(Helper.integerFromByteArray(data));

				case SteamPacket.S2A_RULES_HEADER:
					return new S2A_RULES_Packet(data);

				case SteamPacket.A2S_SERVERQUERY_GETCHALLENGE_HEADER:
					return new A2S_SERVERQUERY_GETCHALLENGE_Packet();

				case SteamPacket.S2C_CHALLENGE_HEADER:
					return new S2C_CHALLENGE_Packet(data);

				case SteamPacket.M2A_SERVER_BATCH_HEADER:
					return new M2A_SERVER_BATCH_Packet(data);

				case SteamPacket.RCON_GOLDSRC_CHALLENGE_HEADER:
				case SteamPacket.RCON_GOLDSRC_NO_CHALLENGE_HEADER:
				case SteamPacket.RCON_GOLDSRC_RESPONSE_HEADER:
					return new RCONGoldSrcResponsePacket(data);

				default:
					throw new PacketFormatException("Unknown packet with header 0x" + header.ToString("x") + " received.");
			}
		}

		/// <summary>
		/// Reassembles the data of a split packet into a single packet object
		/// </summary>
		/// <param name="splitPackets"> An array of packet data </param>
		/// <exception cref="SteamCondenserException"> if decompressing the packet data fails </exception>
		/// <exception cref="PacketFormatException"> if the calculated CRC32 checksum does not
		///         match the expected value </exception>
		/// <returns> SteamPacket The reassembled packet </returns>
		/// <seealso cref= SteamPacketFactory#getPacketFromData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SteamPacket reassemblePacket(java.util.ArrayList<byte[]> splitPackets) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public static SteamPacket reassemblePacket(List<sbyte[]> splitPackets)
		{
			return SteamPacketFactory.reassemblePacket(splitPackets, false, 0, 0);
		}

		/// <summary>
		/// Reassembles the data of a split and/or compressed packet into a single
		/// packet object
		/// </summary>
		/// <param name="splitPackets"> An array of packet data </param>
		/// <param name="isCompressed"> whether the data of this packet is compressed </param>
		/// <param name="uncompressedSize"> The size of the decompressed packet data </param>
		/// <param name="packetChecksum"> The CRC32 checksum of the decompressed
		///        packet data </param>
		/// <exception cref="SteamCondenserException"> if decompressing the packet data fails </exception>
		/// <exception cref="PacketFormatException"> if the calculated CRC32 checksum does not
		///         match the expected value </exception>
		/// <returns> SteamPacket The reassembled packet </returns>
		/// <seealso cref= SteamPacketFactory#getPacketFromData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static SteamPacket reassemblePacket(java.util.ArrayList<byte[]> splitPackets, boolean isCompressed, int uncompressedSize, int packetChecksum) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public static SteamPacket reassemblePacket(List<sbyte[]> splitPackets, bool isCompressed, int uncompressedSize, int packetChecksum)
		{
			sbyte[] packetData, tmpData;
			packetData = new sbyte[0];

			foreach (sbyte[] splitPacket in splitPackets)
			{
				tmpData = packetData;
				packetData = new sbyte[tmpData.Length + splitPacket.Length];
				Array.Copy(tmpData, 0, packetData, 0, tmpData.Length);
				Array.Copy(splitPacket, 0, packetData, tmpData.Length, splitPacket.Length);
			}

			if (isCompressed)
			{
				try
				{
					MemoryStream stream = new MemoryStream(packetData);
					stream.Read();
					stream.Read();
					BZip2CompressorInputStream bzip2 = new BZip2CompressorInputStream(stream);
					sbyte[] uncompressedPacketData = new sbyte[uncompressedSize];
					bzip2.read(uncompressedPacketData, 0, uncompressedSize);

					CRC32 crc32 = new CRC32();
					crc32.update(uncompressedPacketData);
					int crc32checksum = (int) crc32.Value;

					if (crc32checksum != packetChecksum)
					{
						throw new PacketFormatException("CRC32 checksum mismatch of uncompressed packet data.");
					}
					packetData = uncompressedPacketData;
				}
				catch (IOException e)
				{
					throw new SteamCondenserException(e.Message, e);
				}
			}

			tmpData = packetData;
			packetData = new sbyte[tmpData.Length - 4];
			Array.Copy(tmpData, 4, packetData, 0, tmpData.Length - 4);

			return SteamPacketFactory.getPacketFromData(packetData);
		}
	}

}