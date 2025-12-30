namespace Tellurian.Utilities;

public static class BoolExtensions
{
    extension(bool value)
    {
        public void IfTrueThrows(string parameterName, string? exceptionMessage = null)
        {
            if (value) throw new ArgumentOutOfRangeException(parameterName, exceptionMessage);
        }

    }
}
