using System;

/// <summary>
/// This code is free software; you can redistribute it and/or modify it under
/// the terms of the new BSD License.
/// 
/// Copyright (c) 2010-2013, Sebastian Staudt
/// </summary>

namespace com.github.koraktor.steamcondenser.exceptions
{
	/// <summary>
	/// This exception is raised when a Steam Web API request or a related action
	/// fails. This can have codeious reasons like an invalid Web API key or a
	/// broken request.
	/// 
	/// @author Sebastian Staudt </summary>
	/// <seealso cref= com.github.koraktor.steamcondenser.community.WebApi </seealso>
	public class WebApiException : SteamCondenserException
	{

		public enum Cause
		{
			HTTP_ERROR,
			INVALID_KEY,
			STATUS_BAD,
			UNAUTHORIZED
		}

		private string message;

		/// <summary>
		/// Creates a new <code>WebApiException</code> instance
		/// </summary>
		/// <param name="message"> The message to attach to the exception </param>
		public WebApiException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a new <code>WebApiException</code> instance
		/// </summary>
		/// <param name="message"> The message to attach to the exception </param>
		/// <param name="cause"> The initial error that caused this exception </param>
		public WebApiException(string message, Exception cause) : base(message, cause)
		{
		}

		/// <summary>
		/// Creates a new WebApiException with an error message according to the
		/// given <code>cause</code>. If this cause is <code>STATUS_BAD</code>
		/// (which will origin from the Web API itself) or <code>HTTP_ERROR</code>
		/// the details about this failed request will be taken from
		/// <code>statusCode</code> and <code>statusMessage</code>.
		/// </summary>
		/// <param name="cause"> An integer indicating the problem which caused this
		///        exception:
		/// 
		///        <ul>
		///        <li><code>HTTP_ERROR</code>: An error during the HTTP request
		///            itself will result in an exception with this reason.</li>
		///        <li><code>INVALID_KEY</code>: This occurs when trying to set a
		///            Web API key that isn't valid, i.e. a 128 bit integer in a
		///            hexadecimal string.
		///        <li><code>STATUS_BAD</code>: This is caused by a successful
		///            request that fails for some Web API internal reason (e.g. an
		///            invalid argument). Details about this failed request will be
		///            taken from <code>statusCode</code> and
		///            <code>statusMessage</code>.
		///        <li><code>UNAUTHORIZED</code>: This happens when a Steam Web API
		///            request is rejected as unauthorized. This most likely means
		///            that you did not specify a valid Web API key using
		///            <seealso cref="WebApi#setApiKey"/>.
		///            A Web API key can be obtained from
		///            http://steamcommunity.com/dev/apikey.
		///        </ul>
		/// 
		///        Other undefined reasons will cause a generic error message. </param>
		public WebApiException(Cause cause) : this(cause, null, null)
		{
		}

		/// <summary>
		/// Creates a new WebApiException with an error message according to the
		/// given <code>cause</code>. If this cause is <code>STATUS_BAD</code>
		/// (which will origin from the Web API itself) or <code>HTTP_ERROR</code>
		/// the details about this failed request will be taken from
		/// <code>statusCode</code> and <code>statusMessage</code>.
		/// </summary>
		/// <param name="cause"> An integer indicating the problem which caused this
		///        exception:
		/// 
		///        <ul>
		///        <li><code>HTTP_ERROR</code>: An error during the HTTP request
		///            itself will result in an exception with this reason.</li>
		///        <li><code>INVALID_KEY</code>: This occurs when trying to set a
		///            Web API key that isn't valid, i.e. a 128 bit integer in a
		///            hexadecimal string.
		///        <li><code>STATUS_BAD</code>: This is caused by a successful
		///            request that fails for some Web API internal reason (e.g. an
		///            invalid argument). Details about this failed request will be
		///            taken from <code>statusCode</code> and
		///            <code>statusMessage</code>.
		///        <li><code>UNAUTHORIZED</code>: This happens when a Steam Web API
		///            request is rejected as unauthorized. This most likely means
		///            that you did not specify a valid Web API key using
		///            <seealso cref="WebApi#setApiKey"/>.
		///            A Web API key can be obtained from
		///            http://steamcommunity.com/dev/apikey.
		///        </ul>
		/// 
		///        Other undefined reasons will cause a generic error message. </param>
		/// <param name="statusCode"> The HTTP status code returned by the Web API </param>
		/// <param name="statusMessage"> The status message returned in the response </param>
		public WebApiException(Cause cause, int? statusCode, string statusMessage)
		{
			switch (cause)
			{
				case com.github.koraktor.steamcondenser.exceptions.WebApiException.Cause.HTTP_ERROR:
					this.message = "The Web API request has failed due to an HTTP error: " + statusMessage + " (status code: " + statusCode + ").";
					break;
				case com.github.koraktor.steamcondenser.exceptions.WebApiException.Cause.INVALID_KEY:
					this.message = "This is not a valid Steam Web API key.";
					break;
				case com.github.koraktor.steamcondenser.exceptions.WebApiException.Cause.STATUS_BAD:
					this.message = "The Web API request failed with the following error: " + statusMessage + " (status code: " + statusCode + ").";
					break;
				case com.github.koraktor.steamcondenser.exceptions.WebApiException.Cause.UNAUTHORIZED:
					this.message = "Your Web API request has been rejected. You most likely did not specify a valid Web API key.";
					break;
				default:
					this.message = "An unexpected error occured while executing a Web API request.";
				break;
			}
		}

		public override string Message
		{
			get
			{
				if (string.ReferenceEquals(this.message, null))
				{
					return base.Message;
				}
				else
				{
					return this.message;
				}
			}
		}

	}

}