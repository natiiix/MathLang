using System;
using System.Numerics;

namespace MathLang
{
    /// <summary>
    /// Class for operating with numeric fractions.
    /// </summary>
    public class Fraction
    {
        /// <summary>
        /// Value of the numerator part of the fraction.
        /// </summary>
        public BigInteger Numerator { get; private set; }

        /// <summary>
        /// Value of the denominator part of the fraction.
        /// </summary>
        public BigInteger Denominator { get; private set; }

        /// <summary>
        /// Constructs a fraction object using the provided numerator and denominator.
        /// </summary>
        /// <param name="numerator">Value of the fraction's numerator.</param>
        /// <param name="denominator">Value of the fraction's denominator.</param>
        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            // Copy the values
            Numerator = numerator;
            Denominator = denominator;

            // Simplify the fraction
            Simplify();
        }

        /// <summary>
        /// Constructs a fraction object from a whole number.
        /// When representing a whole number as a fraction, the numerator is equal to
        /// the value of the number and denominator is always equal to zero.
        /// </summary>
        /// <param name="number">A whole number to be represented by the fraction.</param>
        public Fraction(BigInteger number) : this(number, 1)
        {
        }

        /// <summary>
        /// An overload of the default constructor that uses standard integers instead of big integers.
        /// </summary>
        /// <param name="numerator">Numerator value of the fraction.</param>
        /// <param name="denominator">Denominator value of the fraction. Set to 1 be default (when representing whole numbers).</param>
        public Fraction(int numerator, int denominator = 1)
        {
            // Convert the integer values into big integers and copy them
            Numerator = numerator;
            Denominator = denominator;

            // Simplify the fraction
            Simplify();
        }

        /// <summary>
        /// Constructs the fraction object from a double precision floating point value.
        /// </summary>
        /// <param name="value">Double precision floating point value to construct the fraction from.</param>
        public Fraction(double value)
        {
            // Example:
            // input value = 1.25
            // generated fraction = 125/100
            // simplified fraction = 5/4

            // Get the currently used decimal separator
            string decimalSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // Get a string representation of the input double value
            string strValue = value.ToString();

            // Get the numerator by removing the decimal separator from the string representation
            // of the double value and treating the remainder of the string as a whole number
            if (BigInteger.TryParse(strValue.Replace(decimalSeparator, string.Empty), out BigInteger numerator))
            {
                // Copy the numerator
                Numerator = numerator;
            }
            // The parsing has failed
            else
            {
                // Throw an exception
                throw new ArgumentException();
            }

            // Get index of the decimal separator
            int decimalSeparatorIdx = strValue.IndexOf(decimalSeparator);

            // Calculate the number of decimal digits
            int decimalDigits = 0;

            // If the index of the decimal separator is below zero, there are no decimal digits
            if (decimalSeparatorIdx >= 0)
            {
                // The number of decimal digits is equal to the index of the last digit minus the index of the decimal separator
                decimalDigits = strValue.Length - 1 - decimalSeparatorIdx;
            }

            // Calculate the denominator
            Denominator = BigInteger.Pow(10, decimalDigits);

            // Simplify the fraction
            Simplify();
        }

        /// <summary>
        /// Simplifies the fraction as much as possible without losing precision.
        /// Also checks for the denominator being zero, which could result in a division by zero.
        /// This method should be called after each numeric modification of the fraction.
        /// </summary>
        private void Simplify()
        {
            // Make sure the denominator is never equal to zero
            if (Denominator == 0)
            {
                // If the denominator is equal to zero, throw a division-by-zero exception
                throw new DivideByZeroException("Denominator of a fraction must not be equal to zero!");
            }

            // In a fraction with a negative value, the numerator part should be negative rather than the denominator part
            // If the denominator is negative
            if (Denominator.Sign < 0)
            {
                // Negate both parts of the fraction
                Numerator = -Numerator;
                Denominator = -Denominator;
            }

            // Get the greatest common divisor of the numerator and the denominator
            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);

            // Divide both parts of the fraction by the GCD
            Numerator /= gcd;
            Denominator /= gcd;
        }

        private static BigInteger LeastCommonDenominator(Fraction a, Fraction b, out BigInteger numeratorA, out BigInteger numeratorB)
        {
            // Get the GCD of the input denominators
            BigInteger denomGcd = BigInteger.GreatestCommonDivisor(a.Denominator, b.Denominator);

            // Calculate by how much will each part of each of the fractions have to be multiplied
            BigInteger multA = (b.Denominator / denomGcd);
            BigInteger multB = (a.Denominator / denomGcd);

            // Calcuate the new numerators
            numeratorA = a.Numerator * multA;
            numeratorB = b.Numerator * multA;

            // Calculate the least common multiply of the denominators and return it
            return multA * multB;
        }

        /// <summary>
        /// Compares this fraction object to another object of an unknown type.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>Returns true if the input object is a fraction with the same value.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is Fraction)
            {
                Fraction f = obj as Fraction;
                return f.Numerator == Numerator && f.Denominator == Denominator;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Calculates the hash code of this fraction.
        /// </summary>
        /// <returns>Returns the hash code of this fraction.</returns>
        public override int GetHashCode()
        {
            return (int)Numerator ^ (int)Denominator;
        }

        /// <summary>
        /// Convert the fraction to a string.
        /// The denominator may be omitted based on its value and the value of the input argument.
        /// </summary>
        /// <param name="forceFractionForm">Determines if the denominator should be included even if its value is 1.</param>
        /// <returns>Returns a string representation of the fraction.</returns>
        public string ToString(bool forceFractionForm)
        {
            // If the demoniator is not equal to 1 or the input argument explicitly requires a fraction form
            if (Denominator != 1 || forceFractionForm)
            {
                // Convert the value of the fraction to its string representation in a fraction form
                return string.Format("{0}/{1}", Numerator, Denominator);
            }
            // Otherwise, if the denominator is equal to 1 and the fraction form is not required
            else
            {
                // Return a string representing the value of the fraction in the form of a whole number
                return Numerator.ToString();
            }
        }

        /// <summary>
        /// Converts the fraction to a string.
        /// Omits the denominator if its value is 1.
        /// </summary>
        /// <returns>Returns a string representation of the fraction.</returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Coverts the fraction to a double precision floating point value.
        /// </summary>
        /// <returns>Returns the value of the fraction represented as a double precision floating point value.</returns>
        public double ToDouble()
        {
            // If the denominator is 1 - if the fraction represents a whole number
            if (Denominator == 1)
            {
                // Return the value of the numerator as a double
                return (double)Numerator;
            }
            // Otherwise, if the fraction doesn't represent a whole number
            else
            {
                // Divide the numerator by the denominator, both represented as floating point values, and return the result
                return (double)Numerator / (double)Denominator;
            }
        }

        /// <summary>
        /// Equality operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns true if the input fractions are equal, false otherwise.</returns>
        public static bool operator ==(Fraction a, Fraction b) => a.Equals(b);

        /// <summary>
        /// Inequality operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns true if the input fractions are not equal, false if they are equal.</returns>
        public static bool operator !=(Fraction a, Fraction b) => !(a == b);

        /// <summary>
        /// Less than operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns true if the value of the first fraction is less than the value of the second fraction.</returns>
        public static bool operator <(Fraction a, Fraction b)
        {
            // Get the numerator values when converted to a common denominator
            LeastCommonDenominator(a, b, out BigInteger numerA, out BigInteger numerB);

            return numerA < numerB;
        }

        /// <summary>
        /// Greater than operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns true if the value of the first fraction is greater than the value of the second fraction.</returns>
        public static bool operator >(Fraction a, Fraction b)
        {
            // Get the numerator values when converted to a common denominator
            LeastCommonDenominator(a, b, out BigInteger numerA, out BigInteger numerB);

            return numerA > numerB;
        }

        /// <summary>
        /// Less than or equal operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns true if the value of the first fraction is less than the value of the second fraction or if their values are equal.</returns>
        public static bool operator <=(Fraction a, Fraction b)
        {
            // Return true if the fractions are equal
            if (a == b)
            {
                return true;
            }
            // If the fractions are not equal
            else
            {
                // Perform a "less than" comparison
                return a < b;
            }
        }

        /// <summary>
        /// Greater than or equal operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns true if the value of the first fraction is greater than the value of the second fraction or if their values are equal.</returns>
        public static bool operator >=(Fraction a, Fraction b)
        {
            // Return true if the fractions are equal
            if (a == b)
            {
                return true;
            }
            // If the fractions are not equal
            else
            {
                // Perform a "greater than" comparison
                return a > b;
            }
        }

        /// <summary>
        /// Negation (unary subtraction) operator overload.
        /// </summary>
        /// <param name="a">Input fraction.</param>
        /// <returns>Returns a fraction with a negated value of the input fraction.</returns>
        public static Fraction operator -(Fraction a)
        {
            return new Fraction(-a.Numerator, a.Denominator);
        }

        /// <summary>
        /// Inversion (bitwise complement) operator overload.
        /// </summary>
        /// <param name="a">Input fraction.</param>
        /// <returns>Returns an inverted fraction to the input fraction.</returns>
        public static Fraction operator ~(Fraction a)
        {
            return new Fraction(a.Denominator, a.Numerator);
        }

        /// <summary>
        /// Addition operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns a fraction with the value equal to the added values of the input fractions.</returns>
        public static Fraction operator +(Fraction a, Fraction b)
        {
            // Get the least common denominator and the new numerator values
            BigInteger commonDenom = LeastCommonDenominator(a, b, out BigInteger numerA, out BigInteger numerB);

            // Create the result fraction by adding the numerators and using the common denominator
            return new Fraction(numerA + numerB, commonDenom);
        }

        /// <summary>
        /// Subtraction operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns a fraction with the value equal to the difference of the input fractions.</returns>
        public static Fraction operator -(Fraction a, Fraction b)
        {
            // Get the least common denominator and the new numerator values
            BigInteger commonDenom = LeastCommonDenominator(a, b, out BigInteger numerA, out BigInteger numerB);

            // Create the result fraction by subtracting the second numerator from the first numerator and using the common denominator
            return new Fraction(numerA - numerB, commonDenom);
        }

        /// <summary>
        /// Multiplication operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns a fraction with the value equal to the multiplied values of the input fractions.</returns>
        public static Fraction operator *(Fraction a, Fraction b)
        {
            // (k/l)*(m/n) = (k*m)/(l*n)
            return new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
        }

        /// <summary>
        /// Division operator overload.
        /// </summary>
        /// <param name="a">First fraction.</param>
        /// <param name="b">Second fraction.</param>
        /// <returns>Returns a fraction with the value equal to the value of the first fraction divided by the value of the second fraction.</returns>
        public static Fraction operator /(Fraction a, Fraction b)
        {
            // (k/l)/(m/n) = (k/l)*(n/m) = (k*n)/(l*m)
            return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }
    }
}