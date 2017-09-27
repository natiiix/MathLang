using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MathLang
{
    public static class Expression
    {
        //private const char OPERATOR_NONE = '\0';

        private const char OPERATOR_ADD = '+';
        private const char OPERATOR_SUBTRACT = '-';

        private const char OPERATOR_MULTIPLY = '*';
        private const char OPERATOR_DIVIDE = '/';

        //private const char OPERATOR_EXPONENT = '^';
        //private const char OPERATOR_FACTORIAL = '!';

        private const char PARENTHESIS_LEFT = '(';
        private const char PARENTHESIS_RIGHT = ')';

        // AS - add / subtract
        private static readonly char[] OPERATORS_AS = new char[] { OPERATOR_ADD, OPERATOR_SUBTRACT };

        // MD - mutliply / divide
        private static readonly char[] OPERATORS_MD = new char[] { OPERATOR_MULTIPLY, OPERATOR_DIVIDE };

        // All the operator characters
        private static readonly char[] OPERATORS_ALL = new char[] { OPERATOR_ADD, OPERATOR_SUBTRACT, OPERATOR_MULTIPLY, OPERATOR_DIVIDE };

        public static BigInteger Evaluate(string expr)
        {
            // Clean up the expression
            string cleaned = CleanUpExpression(expr);

            // Constant expression
            if (BigInteger.TryParse(cleaned, out BigInteger exprValue))
            {
                return exprValue;
            }
            // Solve MD and AS operations
            else if (cleaned.IndexOfAny(OPERATORS_ALL) >= 0)
            {
                // Parse the expression, separate values from operators
                ParseExpression(cleaned, out List<BigInteger> values, out List<char> operators);

                // AS pool
                BigInteger asPool = 0;
                char lastAsOperator = OPERATOR_ADD;

                // MD pool
                BigInteger mdPool = values.First();

                // TODO: exponent, factorial

                // Solve the expression by iterating through the individual operations
                // in their order and applying them to the values related to them
                for (int i = 0; i < operators.Count; i++)
                {
                    switch (operators[i])
                    {
                        // Addition
                        case OPERATOR_ADD:
                            asPool += mdPool;

                            mdPool = values[i + 1];
                            lastAsOperator = operators[i];
                            break;

                        // Subtraction
                        case OPERATOR_SUBTRACT:
                            asPool -= mdPool;

                            mdPool = values[i + 1];
                            lastAsOperator = operators[i];
                            break;

                        // Multiplication
                        case OPERATOR_MULTIPLY:
                            mdPool *= values[i + 1];
                            break;

                        // Division
                        case OPERATOR_DIVIDE:
                            mdPool /= values[i + 1];
                            break;

                        default:
                            throw new Exception();
                    }
                }

                // Flush the MD pool to the AS pool and return the result
                switch (lastAsOperator)
                {
                    case OPERATOR_ADD:
                        return asPool + mdPool;

                    case OPERATOR_SUBTRACT:
                        return asPool - mdPool;

                    default:
                        throw new Exception();
                }
            }

            throw new ArgumentException("Invalid expression!");
        }

        private static void ParseExpression(string expr, out List<BigInteger> values, out List<char> operators)
        {
            values = new List<BigInteger>();
            operators = new List<char>();

            bool expectValue = true;
            bool negateNextValue = false;

            for (int i = 0; i < expr.Length; i++)
            {
                // Value
                if (expectValue)
                {
                    // Minus sign / negative value
                    if (expr[i] == OPERATOR_SUBTRACT)
                    {
                        negateNextValue = !negateNextValue;
                    }
                    else
                    {
                        // Find the sub-expression and evaluate it
                        BigInteger val = Evaluate(ReadToEnd(expr, i, out int len));

                        // If the last operator was a subtraction, negate the value
                        values.Add(negateNextValue ? -val : val);

                        // Skip to the end of the sub-expression
                        i += len - 1;

                        // A value most be followed by an operator (except for the last value in the expression)
                        expectValue = false;

                        // Reset the negation bool
                        negateNextValue = false;
                    }
                }
                // Operator
                else if (OPERATORS_ALL.Contains(expr[i]))
                {
                    operators.Add(expr[i]);

                    // An operator must be followed by a value / sub-expression
                    expectValue = true;
                }
            }

            // There must be exactly one operator between a pair of values
            if (values.Count != operators.Count + 1)
            {
                throw new ArgumentException("Invalid expression!");
            }
        }

        private static string CleanUpExpression(string expr)
        {
            // Make sure each parenthesis has its counterpart
            if (expr.Count(x => x == PARENTHESIS_LEFT) != expr.Count(x => x == PARENTHESIS_RIGHT))
            {
                throw new ArgumentException("Parentheses mismatch!");
            }

            // Remove spaces
            string noSpaces = expr.Replace(" ", string.Empty);

            // If the expression is inside parenthesis, remove them
            if (noSpaces.First() == PARENTHESIS_LEFT && noSpaces.Last() == PARENTHESIS_RIGHT)
            {
                return noSpaces.Substring(1, noSpaces.Length - 2);
            }
            // Otherwise don't do anything and return the version without spaces
            else
            {
                return noSpaces;
            }
        }

        private static bool IsBaseTenDigit(this char c)
        {
            return (c >= '0' && c <= '9');
        }

        private static string ReadToEnd(string expr, int start, out int length)
        {
            length = 0;

            if (expr[start] == PARENTHESIS_LEFT)
            {
                int depth = 0;

                for (int i = start + 1; i < expr.Length; i++)
                {
                    if (expr[i] == PARENTHESIS_LEFT)
                    {
                        depth++;
                    }
                    else if (expr[i] == PARENTHESIS_RIGHT)
                    {
                        if (depth == 0)
                        {
                            length = i - start + 1;
                            break;
                        }
                        else
                        {
                            depth--;
                        }
                    }
                }
            }
            else if (expr[start].IsBaseTenDigit() || expr[start] == OPERATOR_SUBTRACT)
            {
                length++;

                while (start + length < expr.Length && expr[start + length].IsBaseTenDigit())
                {
                    length++;
                }
            }

            if (length > 0)
            {
                return expr.Substring(start, length);
            }
            else
            {
                throw new ArgumentException("Unable to recognize the sub-expression!");
            }
        }
    }
}