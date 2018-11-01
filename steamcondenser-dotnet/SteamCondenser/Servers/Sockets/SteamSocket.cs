/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2014, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.servers.sockets
{
    using System.IO;
    using System.Net;
    using ConnectionResetException = com.github.koraktor.steamcondenser.exceptions.ConnectionResetException;
	using PacketFormatException = com.github.koraktor.steamcondenser.exceptions.PacketFormatException;
	using SteamCondenserException = com.github.koraktor.steamcondenser.exceptions.SteamCondenserException;
	using SteamPacket = com.github.koraktor.steamcondenser.servers.packets.SteamPacket;
	using SteamPacketFactory = com.github.koraktor.steamcondenser.servers.packets.SteamPacketFactory;

	/// <summary>
	/// This abstract class implements common functionality for sockets used to
	/// connect to game and master servers
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public abstract class SteamSocket
	{

		protected internal static int timeout = 1000;

		protected internal ByteBuffer buffer;
		protected internal SelectableChannel channel;
		protected internal InetSocketAddress remoteSocket;

		/// <summary>
		/// Sets the timeout for socket operations
		/// <para>
		/// Any request that takes longer than this time will cause a {@link
		/// TimeoutException}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="timeout"> The amount of milliseconds before a request times out </param>
		public static int Timeout
		{
			set
			{
				SteamSocket.timeout = value;
			}
		}

		/// <summary>
		/// Creates a new UDP socket to communicate with the server on the given IP
		/// address and port
		/// </summary>
		/// <param name="ipAddress"> Either the IP address or the DNS name of the server </param>
		/// <param name="portNumber"> The port the server is listening on </param>
		protected internal SteamSocket(IPAddress ipAddress, int portNumber)
		{
			this.buffer = ByteBuffer.allocate(1400);
			this.buffer.order(ByteOrder.LITTLE_ENDIAN);

			this.remoteSocket = new InetSocketAddress(ipAddress, portNumber);
		}

		/// <summary>
		/// Reads a single packet from the buffer into a packet object
		/// </summary>
		/// <returns> The packet object created from the data in the buffer </returns>
		/// <exception cref="PacketFormatException"> if the data is not formatted correctly </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected com.github.koraktor.steamcondenser.servers.packets.SteamPacket getPacketFromData() throws com.github.koraktor.steamcondenser.exceptions.PacketFormatException
		protected internal virtual SteamPacket PacketFromData
		{
			get
			{
				sbyte[] packetData = new sbyte[this.buffer.remaining()];
				this.buffer.get(packetData);
    
				return SteamPacketFactory.getPacketFromData(packetData);
			}
		}

		/// <summary>
		/// Subclasses have to implement this method for their individual packet
		/// formats
		/// </summary>
		/// <returns> The packet replied from the server </returns>
		/// <exception cref="SteamCondenserException"> if an error occurs while communicating
		///         with the server </exception>
		/// <exception cref="TimeoutException"> if the request times out </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract com.github.koraktor.steamcondenser.servers.packets.SteamPacket getReply() throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException;
		public abstract SteamPacket Reply {get;}

		/// <summary>
		/// Reads the given amount of data from the socket and wraps it into the
		/// buffer
		/// </summary>
		/// <param name="bufferLength"> The data length to read from the socket </param>
		/// <exception cref="SteamCondenserException"> if an error occurs while reading from
		///         the socket </exception>
		/// <exception cref="TimeoutException"> if no packet is received on time </exception>
		/// <returns> int The number of bytes that have been read from the socket </returns>
		/// <seealso cref= ByteBuffer </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int receivePacket(int bufferLength) throws com.github.koraktor.steamcondenser.exceptions.SteamCondenserException, java.util.concurrent.TimeoutException
		protected internal virtual int receivePacket(int bufferLength)
		{
			Selector selector = null;
			try
			{
				selector = Selector.open();
				this.channel.register(selector, SelectionKey.OP_READ);

				if (selector.select(SteamSocket.timeout) == 0)
				{
					throw new TimeoutException();
				}

				int bytesRead;

				if (bufferLength == 0)
				{
					this.buffer.clear();
				}
				else
				{
					this.buffer = ByteBuffer.allocate(bufferLength);
				}

				bytesRead = ((ReadableByteChannel) this.channel).read(this.buffer);
				if (bytesRead < 0)
				{
					bytesRead = 0;
				}

				this.buffer.rewind();
				this.buffer.limit(bytesRead);

				return bytesRead;
			}
			catch (IOException e)
			{
				if ("Connection reset by peer".Equals(e.Message))
				{
					throw new ConnectionResetException();
				}
				throw new SteamCondenserException(e.Message, e);
			}
			finally
			{
				if (selector != null)
				{
					try
					{
						selector.close();
					}
					catch (IOException e)
					{
						throw new SteamCondenserException(e.Message, e);
					}
				}
			}
		}

		/// <summary>
		/// Closes this socket
		/// </summary>
		/// <seealso cref= #close </seealso>
		~SteamSocket()
		{
			this.close();
		}

		/// <summary>
		/// Closes the underlying socket
		/// </summary>
		/// <seealso cref= SelectableChannel#close </seealso>
		public virtual void close()
		{
			try
			{
				if (this.channel.Open)
				{
					this.channel.close();
				}
			}
			catch (IOException)
			{
			}
		}
	}

}