using System;
using System.Collections.Generic;
using System.Net;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser.servers.sockets
{

	using RCONBanException = com.github.koraktor.steamcondenser.exceptions.RCONBanException;
	using RCONNoAuthException = com.github.koraktor.steamcondenser.exceptions.RCONNoAuthException;
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using SteamPacket = com.github.koraktor.steamcondenser.servers.packets.SteamPacket;
	using SteamPacketFactory = com.github.koraktor.steamcondenser.servers.packets.SteamPacketFactory;
	using RCONGoldSrcRequestPacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONGoldSrcRequestPacket;
	using RCONGoldSrcResponsePacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONGoldSrcResponsePacket;

	/// <summary>
	/// This class represents a socket used to communicate with game servers based
	/// on the GoldSrc engine (e.g. Half-Life, Counter-Strike)
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public class GoldSrcSocket : QuerySocket
	{

		protected internal bool isHLTV;
		protected internal long rconChallenge = -1;

		/// <summary>
		/// Creates a new socket to communicate with the server on the given IP
		/// address and port
		/// </summary>
		/// <param name="ipAddress"> Either the IP address or the DNS name of the server </param>
		/// <param name="portNumber"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if the socket cannot be opened </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcSocket(java.net.IPAddress ipAddress, int portNumber) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcSocket(IPAddress ipAddress, int portNumber) : base(ipAddress, portNumber)
		{
			this.isHLTV = false;
		}

		/// <summary>
		/// Creates a new socket to communicate with the server on the given IP
		/// address and port
		/// </summary>
		/// <param name="ipAddress"> Either the IP address or the DNS name of the server </param>
		/// <param name="portNumber"> The port the server is listening on </param>
		/// <param name="isHLTV"> <code>true</code> if the target server is a HTLV instance.
		///        HLTV behaves slightly different for RCON commands, this flag
		///        increases compatibility. </param>
		/// <exception cref="SteamCondenserException"> if the socket cannot be opened </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GoldSrcSocket(java.net.IPAddress ipAddress, int portNumber, boolean isHLTV) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public GoldSrcSocket(IPAddress ipAddress, int portNumber, bool isHLTV) : base(ipAddress, portNumber)
		{
			this.isHLTV = isHLTV;
		}

		/// <summary>
		/// Reads a packet from the socket
		/// <para>
		/// The Source query protocol specifies a maximum packet size of 1,400
		/// bytes. Bigger packets will be split over several UDP packets. This
		/// method reassembles split packets into single packet objects.
		/// 
		/// </para>
		/// </summary>
		/// <returns> The packet replied from the server </returns>
		/// <exception cref="SteamCondenserException"> if an error occurs while communicating
		///         with the server </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public com.github.koraktor.steamcondenser.servers.packets.SteamPacket getReply() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public override SteamPacket Reply
		{
			get
			{
				int bytesRead;
				SteamPacket packet;
    
				bytesRead = this.receivePacket(1400);
    
				if (this.packetIsSplit())
				{
					sbyte[] splitData;
					int packetCount, packetNumber;
					int requestId;
					sbyte packetNumberAndCount;
					List<sbyte[]> splitPackets = new List<sbyte[]>();
    
					do
					{
						requestId = Integer.reverseBytes(this.buffer.Int);
						packetNumberAndCount = this.buffer.get();
						packetCount = packetNumberAndCount & 0xF;
						packetNumber = (packetNumberAndCount >> 4) + 1;
    
						splitData = new sbyte[this.buffer.remaining()];
						this.buffer.get(splitData);
						splitPackets.Capacity = packetCount;
						splitPackets.Insert(packetNumber - 1, splitData);
    
						Console.WriteLine("Received packet #" + packetNumber + " of " + packetCount + " for request ID " + requestId + ".");
    
						if (splitPackets.Count < packetCount)
						{
							try
							{
								bytesRead = this.receivePacket();
							}
							catch (TimeoutException)
							{
								bytesRead = 0;
							}
						}
						else
						{
							bytesRead = 0;
						}
					} while (bytesRead > 0 && this.packetIsSplit());
    
					packet = SteamPacketFactory.reassemblePacket(splitPackets);
				}
				else
				{
					packet = this.PacketFromData;
				}
    
				Console.WriteLine("Received packet of type \"" + packet.GetType().Name + "\"");
    
				return packet;
			}
		}

		/// <summary>
		/// Executes the given command on the server via RCON
		/// </summary>
		/// <param name="password"> The password to authenticate with the server </param>
		/// <param name="command"> The command to execute on the server </param>
		/// <returns> The response replied by the server </returns>
		/// <seealso cref= #rconChallenge </seealso>
		/// <seealso cref= #rconSend </seealso>
		/// <exception cref="RCONBanException"> if the IP of the local machine has been banned
		///         on the game server </exception>
		/// <exception cref="RCONNoAuthException"> if the password is incorrect </exception>
		/// <exception cref="SteamCondenserException"> if an error occurs while communicating
		///         with the server </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String rconExec(String password, String command) throws java.util.concurrent.TimeoutException, com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public virtual string rconExec(string password, string command)
		{
			if (this.rconChallenge == -1 || this.isHLTV)
			{
				this.rconGetChallenge();
			}

			this.rconSend("rcon " + this.rconChallenge + " " + password + " " + command);

			string response;
			if (this.isHLTV)
			{
				try
				{
					response = ((RCONGoldSrcResponsePacket)this.Reply).Response;
				}
				catch (TimeoutException)
				{
					response = "";
				}
			}
			else
			{
				response = ((RCONGoldSrcResponsePacket)this.Reply).Response;
			}

			if (response.Trim().Equals("Bad rcon_password."))
			{
				throw new RCONNoAuthException();
			}
			else if (response.Trim().Equals("You have been banned from this server"))
			{
				throw new RCONBanException();
			}

			this.rconSend("rcon " + this.rconChallenge + " " + password);

			string responsePart;
			do
			{
				responsePart = ((RCONGoldSrcResponsePacket)this.Reply).Response;
				response += responsePart;
			} while (responsePart.Length > 0);

			return response;
		}

		/// <summary>
		/// Requests a challenge number from the server to be used for further
		/// requests
		/// </summary>
		/// <seealso cref= #rconSend </seealso>
		/// <exception cref="SteamCondenserException"> if an error occurs while communicating
		///         with the server </exception>
		/// <exception cref="RCONBanException"> if the IP of the local machine has been banned
		///         on the game server </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void rconGetChallenge() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public virtual void rconGetChallenge()
		{
			this.rconSend("challenge rcon");

			string response = ((RCONGoldSrcResponsePacket)this.Reply).Response.Trim();
			if (response.Equals("You have been banned from this server."))
			{
				throw new RCONBanException();
			}

			this.rconChallenge = Convert.ToInt64(response.Substring(14));
		}

		/// <summary>
		/// Wraps the given command in a RCON request packet and send it to the
		/// server
		/// </summary>
		/// <param name="command"> The RCON command to send to the server </param>
		/// <exception cref="SteamCondenserException"> if an error occured while writing to the
		///         socket </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void rconSend(String command) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		protected internal virtual void rconSend(string command)
		{
			this.send(new RCONGoldSrcRequestPacket(command));
		}
	}

}