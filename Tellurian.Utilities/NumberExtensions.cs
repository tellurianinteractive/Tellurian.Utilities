namespace Tellurian.Utilities;

public static class NumberExtensions
{
    extension(int number)
    {
        /// <summary>
        /// Determines whether the specified number has the same parity (odd or even) as the current number.
        /// </summary>
        /// <param name="otherNumber">The number to compare with the current number to check for matching parity.</param>
        /// <returns>true if both numbers are odd or both are even; otherwise, false.</returns>
        public bool IsAlsoOddOrEven(int otherNumber) => (number - otherNumber) % 2 == 0;
    }
}
