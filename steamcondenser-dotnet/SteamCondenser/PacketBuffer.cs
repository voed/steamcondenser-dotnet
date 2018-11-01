using System;

/*
 * This code is free software; you can redistribute it and/or modify it under
 * the terms of the new BSD License.
 *
 * Copyright (c) 2008-2018, Sebastian Staudt
 */

namespace com.github.koraktor.steamcondenser
{


	/// <summary>
	/// A convenience class wrapping around <seealso cref="ByteBuffer"/> used for easy
	/// retrieval of string values
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public class PacketBuffer
	{

		private ByteBuffer byteBuffer;

		/// <summary>
		/// Creates a new packet buffer from the given byte array
		/// </summary>
		/// <param name="data"> The data wrap into the underlying byte buffer </param>
		public PacketBuffer(sbyte[] data)
		{
			this.byteBuffer = ByteBuffer.wrap(data);
		}

		/// <summary>
		/// Returns the backing byte array of the underlying byte buffer
		/// </summary>
		/// <returns> The backing byte array </returns>
		public virtual sbyte[] array()
		{
			return this.byteBuffer.array();
		}

		/// <summary>
		///// Returns the next byte at the buffer's current position
		/// </summary>
		/// <returns> A byte </returns>
		public virtual sbyte Byte
		{
			get
			{
				return this.byteBuffer.get();
			}
		}

		/// <summary>
		/// Returns an integer value from the buffer's current position
		/// </summary>
		/// <returns> An integer value </returns>
		public virtual int Int
		{
			get
			{
				return this.byteBuffer.Int;
			}
		}

		/// <summary>
		/// Returns the length of this buffer
		/// </summary>
		/// <returns> The length of this buffer </returns>
		public virtual int Length
		{
			get
			{
				return this.byteBuffer.capacity();
			}
		}

		/// <summary>
		/// Returns a short integer value from the buffer's current position
		/// </summary>
		/// <returns> A short integer value </returns>
		public virtual short Short
		{
			get
			{
				return this.byteBuffer.Short;
			}
		}

		/// <summary>
		/// Returns a string value from the buffer's current position
		/// <para>
		/// This reads the bytes up to the first zero-byte of the underlying byte
		/// buffer into a String
		/// 
		/// </para>
		/// </summary>
		/// <returns> A string value </returns>
		public virtual string String
		{
			get
			{
				sbyte[] remainingBytes = new sbyte[this.byteBuffer.remaining()];
				this.byteBuffer.slice().get(remainingBytes);
				int zeroPosition = ArrayUtils.IndexOf(remainingBytes, (sbyte) 0);
    
				if (zeroPosition == ArrayUtils.INDEX_NOT_FOUND)
				{
					return null;
				}
				else
				{
					sbyte[] stringBytes = new sbyte[zeroPosition];
					Array.Copy(remainingBytes, 0, stringBytes, 0, zeroPosition);
					this.byteBuffer.position(this.byteBuffer.position() + zeroPosition + 1);
    
					return StringHelper.NewString(stringBytes);
				}
			}
		}

		/// <summary>
		/// Returns the number of bytes remaining in the underlying byte buffer from
		/// the current position up to the end
		/// </summary>
		/// <returns> The number of bytes remaining in this buffer </returns>
		public virtual int remaining()
		{
			return this.byteBuffer.remaining();
		}

		/// <summary>
		/// Returns whether there is more data available in this buffer after the
		/// current position
		/// </summary>
		/// <returns> <code>true</code> if there's at least one byte left remaining </returns>
		public virtual bool hasRemaining()
		{
			return this.byteBuffer.hasRemaining();
		}
	}

}