using System;
using System.Net;

/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.sockets
{

	using ConnectionResetException = com.github.koraktor.steamcondenser.exceptions.ConnectionResetException;
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using RCONPacket = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONPacket;
	using RCONPacketFactory = com.github.koraktor.steamcondenser.servers.packets.rcon.RCONPacketFactory;

	/// <summary>
	/// This class represents a socket used for RCON communication with game servers
	/// based on the Source engine (e.g. Team Fortress 2, Counter-Strike: Source)
	/// <para>
	/// The Source engine uses a stateful TCP connection for RCON communication and
	/// uses an additional socket of this type to handle RCON requests.
	/// 
	/// @author Sebastian Staudt
	/// </para>
	/// </summary>
	public class RCONSocket : SteamSocket
	{

		/// <summary>
		/// Creates a new TCP socket to communicate with the server on the given IP
		/// address and port
		/// </summary>
		/// <param name="ipAddress"> Either the IP address or the DNS name of the server </param>
		/// <param name="portNumber"> The port the server is listening on </param>
		public RCONSocket(IPAddress ipAddress, int portNumber) : base(ipAddress, portNumber)
		{
		}

		/// <summary>
		/// Closes the underlying TCP socket if it is connected
		/// </summary>
		/// <seealso cref= SteamSocket#close </seealso>
		public override void close()
		{
			if (this.channel != null && ((SocketChannel) this.channel).Connected)
			{
				base.close();
			}
		}

		/// <summary>
		/// Sends the given RCON packet to the server
		/// </summary>
		/// <param name="dataPacket"> The RCON packet to send to the server </param>
		/// <exception cref="SteamCondenserException"> if an error occurs while writing to the
		///         socket </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void send(com.github.koraktor.steamcondenser.servers.packets.rcon.RCONPacket dataPacket) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException
		public virtual void send(RCONPacket dataPacket)
		{
			try
			{
				if (this.channel == null || !((SocketChannel)this.channel).Connected)
				{
					this.channel = SocketChannel.open();
					((SocketChannel) this.channel).socket().connect(this.remoteSocket, SteamSocket.timeout);
					this.channel.configureBlocking(false);
				}

				this.buffer = ByteBuffer.wrap(dataPacket.Bytes);
				((SocketChannel)this.channel).write(this.buffer);
			}
			catch (IOException e)
			{
				throw new SteamCondenserException(e.Message, e);
			}
		}

		/// <summary>
		/// Reads a packet from the socket
		/// <para>
		/// The Source RCON protocol allows packets of an arbitrary sice transmitted
		/// using multiple TCP packets. The data is received in chunks and
		/// concatenated into a single response packet.
		/// 
		/// </para>
		/// </summary>
		/// <returns> The packet replied from the server or <code>null</code> if the
		///         connection has been closed by the server </returns>
		/// <exception cref="SteamCondenserException"> if an error occurs while communicating
		///         with the server </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public com.github.koraktor.steamcondenser.servers.packets.rcon.RCONPacket getReply() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		public override RCONPacket Reply
		{
			get
			{
				try
				{
					if (this.receivePacket(4) == 0)
					{
						try
						{
							this.channel.close();
						}
						catch (IOException)
						{
						}
						return null;
					}
				}
				catch (ConnectionResetException)
				{
					try
					{
						this.channel.close();
					}
					catch (IOException)
					{
					}
					return null;
				}
    
				int packetSize = Integer.reverseBytes(this.buffer.Int);
				int remainingBytes = packetSize;
    
				sbyte[] packetData = new sbyte[packetSize];
				int receivedBytes;
				do
				{
					receivedBytes = this.receivePacket(remainingBytes);
					Array.Copy(this.buffer.array(), 0, packetData, packetSize - remainingBytes, receivedBytes);
					remainingBytes -= receivedBytes;
				} while (remainingBytes > 0);
    
				RCONPacket packet = RCONPacketFactory.getPacketFromData(packetData);
    
				Console.WriteLine("Received packet of type \"" + packet.GetType() + "\".");
    
				return packet;
			}
		}
	}

}