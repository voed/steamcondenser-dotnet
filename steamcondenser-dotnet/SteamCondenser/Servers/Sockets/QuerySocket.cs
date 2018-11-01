/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>
using System.Net;

namespace com.github.koraktor.steamcondenser.servers.sockets
{
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using SteamPacket = com.github.koraktor.steamcondenser.servers.packets.SteamPacket;

	/// <summary>
	/// This class implements basic functionality for UDP based sockets
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public abstract class QuerySocket : SteamSocket
	{

		/// <summary>
		/// Creates a new socket to communicate with the server on the given IP
		/// address and port
		/// </summary>
		/// <param name="ipAddress"> Either the IP address or the DNS name of the server </param>
		/// <param name="portNumber"> The port the server is listening on </param>
		/// <exception cref="SteamCondenserException"> if the socket cannot be opened </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected QuerySocket(java.net.IPAddress ipAddress, int portNumber) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		protected internal QuerySocket(IPAddress ipAddress, int portNumber) : base(ipAddress, portNumber)
		{

			try
			{
				this.channel = DatagramChannel.open();
				this.channel.configureBlocking(false);
				((DatagramChannel) this.channel).connect(this.remoteSocket);
			}
			catch (IOException e)
			{
				throw new SteamCondenserException(e.Message, e);
			}
		}

		/// <summary>
		/// Returns whether a packet in the buffer is split
		/// </summary>
		/// <returns> <code>true</code> if the packet is split </returns>
		protected internal virtual bool packetIsSplit()
		{
			return (Integer.reverseBytes(this.buffer.Int) == 0xFFFFFFFE);
		}

		/// <summary>
		/// Reads an UDP packet into the buffer
		/// </summary>
		/// <returns> The number of bytes received </returns>
		/// <exception cref="SteamCondenserException"> if an error occurs while reading from
		///         the socket </exception>
		/// <exception cref="TimeoutException"> if no UDP packet was received </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int receivePacket() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		protected internal virtual int receivePacket()
		{
			return this.receivePacket(0);
		}

		/// <summary>
		/// Sends the given packet to the server
		/// </summary>
		/// <param name="dataPacket"> The packet to send to the server </param>
		/// <exception cref="SteamCondenserException"> if an error occurs while writing to the
		///         socket </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void send(com.github.koraktor.steamcondenser.servers.packets.SteamPacket dataPacket) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public virtual void send(SteamPacket dataPacket)
		{
			Console.WriteLine("Sending data packet of type \"" + dataPacket.GetType().Name + "\"");

			try
			{
				this.buffer = ByteBuffer.wrap(dataPacket.Bytes);
				((DatagramChannel) this.channel).send(this.buffer, this.remoteSocket);
				this.buffer.flip();
			}
			catch (IOException e)
			{
				throw new SteamCondenserException(e.Message, e);
			}
		}
	}

}