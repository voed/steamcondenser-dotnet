/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2008-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser
{
	/// <summary>
	/// A helper class used to convert byte arrays into integers and vice-versa
	/// 
	/// @author Sebastian Staudt
	/// </summary>
	public abstract class Helper
	{

		/// <summary>
		/// Convert an integer value into the corresponding byte array
		/// </summary>
		/// <param name="integer"> The integer to convert </param>
		/// <returns> The byte array representing the given integer </returns>
		public static sbyte[] byteArrayFromInteger(int integer)
		{
			return new sbyte[] {(sbyte)(integer >> 24), (sbyte)(integer >> 16), (sbyte)(integer >> 8), (sbyte) integer};
		}

		/// <summary>
		/// Convert a byte array into the corresponding integer value of its bytes
		/// </summary>
		/// <param name="byteArray"> The byte array to convert </param>
		/// <returns> The integer represented by the byte array </returns>
		public static int integerFromByteArray(sbyte[] byteArray)
		{
			return byteArray[0] << 24 | (byteArray[1] & 0xff) << 16 | (byteArray[2] & 0xff) << 8 | (byteArray[3] & 0xff);
		}
	}

}