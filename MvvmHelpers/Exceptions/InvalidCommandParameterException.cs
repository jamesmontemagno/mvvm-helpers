using System;

namespace MvvmHelpers.Exceptions
{
	/// <summary>
	/// Represents errors that occur during IAsyncCommand execution.
	/// </summary>
	public class InvalidCommandParameterException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MvvmHelpersInvalidCommandParameterException"/> class.
		/// </summary>
		/// <param name="expectedType">Expected parameter type for AsyncCommand.Execute.</param>
		/// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
		/// <param name="innerException">Inner Exception</param>
		public InvalidCommandParameterException(Type expectedType, Type actualType, Exception innerException) : base(CreateErrorMessage(expectedType, actualType), innerException)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MvvmHelpers.InvalidCommandParameterException"/> class.
		/// </summary>
		/// <param name="expectedType">Expected parameter type for AsyncCommand.Execute.</param>
		/// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
		public InvalidCommandParameterException(Type expectedType, Type actualType) : base(CreateErrorMessage(expectedType, actualType))
		{

		}

		static string CreateErrorMessage(Type expectedType, Type actualType) =>
			$"Invalid type for parameter. Expected Type: {expectedType}, but received Type: {actualType}";
	}
}
