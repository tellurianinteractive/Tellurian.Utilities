namespace Tellurian.Utilities;

/// <summary>
/// Extensions for bool values.
/// </summary>
public static class BoolExtensions
{
    extension(bool value)
    {
        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when the value is true, as a guard on a condition about an argument.
        /// </summary>
        /// <param name="parameterName">The name of the argument the condition is about.</param>
        /// <param name="exceptionMessage">The message of the exception, or the default message.</param>
        /// <exception cref="ArgumentOutOfRangeException">The value is true.</exception>
        public void IfTrueThrows(string parameterName, string? exceptionMessage = null)
        {
            if (value) throw new ArgumentOutOfRangeException(parameterName, exceptionMessage);
        }
    }
}
