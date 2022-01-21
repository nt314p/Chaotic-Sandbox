#include "EquationToken.h"
#include "Token.h"
#include "Common.h"

std::unordered_map<EquationTokenType, String> equationTokenTypeDict =
{
	{ EquationTokenType::Bad, "Bad" },
	{ EquationTokenType::OpenParenthesis, "OpenParenthesis" },
	{ EquationTokenType::CloseParenthesis, "CloseParenthesis" },
	{ EquationTokenType::AdditionOperator, "AdditionOperator" },
	{ EquationTokenType::SubtractionOperator, "SubtractionOperator" },
	{ EquationTokenType::MultiplicationOperator, "MultiplicationOperator" },
	{ EquationTokenType::DivisionOperator, "DivisionOperator" },
	{ EquationTokenType::ExponentiationOperator, "ExponentiationOperator" },
	{ EquationTokenType::NegationOperator, "NegationOperator" },
	{ EquationTokenType::SineOperator, "SineOperator" },
	{ EquationTokenType::CosineOperator, "CosineOperator" },
	{ EquationTokenType::TangentOperator, "TangentOperator" },
	{ EquationTokenType::AbsoluteOperator, "AbsoluteOperator" },
	{ EquationTokenType::FloorOperator, "FloorOperator" },
	{ EquationTokenType::CeilingOperator, "CeilingOperator" },
	{ EquationTokenType::SquareRootOperator, "SquareRootOperator" },
	{ EquationTokenType::CubeRootOperator, "CubeRootOperator" },
	{ EquationTokenType::EqualsOperator, "EqualsOperator" },
	{ EquationTokenType::VariableOperand, "VariableOperand" },
	{ EquationTokenType::NumericalOperand, "NumericalOperand" }
};

std::unordered_map<String, EquationTokenType> identifierDict =
{
	{ "sin", EquationTokenType::SineOperator },
	{ "cos", EquationTokenType::CosineOperator },
	{ "tan", EquationTokenType::TangentOperator },
	{ "abs", EquationTokenType::AbsoluteOperator },
	{ "floor", EquationTokenType::FloorOperator },
	{ "ceil", EquationTokenType::CeilingOperator },
	{ "sqrt", EquationTokenType::SquareRootOperator },
	{ "cbrt", EquationTokenType::CubeRootOperator }
};

std::vector<EquationToken> ConvertToEquationTokens(std::vector<Token>& tokens)
{
	std::vector<EquationToken> equationTokens;

	EquationTokenType previousEquationTokenType = EquationTokenType::Bad;

	int index = 0;
	int parenthesisCount = 0;
	bool containsEqualsOperator = false;

	while (index < tokens.size())
	{
		Token token = tokens[index];
		TokenType tokenType = token.Type;

		EquationTokenType equationTokenType = EquationTokenType::Bad;
		switch (tokenType)
		{
		case TokenType::Bad:
			std::cerr << "Bad token!" << std::endl;
			break;
		case TokenType::Whitespace:
			index++;
			continue;
		case TokenType::OpenParenthesis:
			equationTokenType = EquationTokenType::OpenParenthesis;
			parenthesisCount++;
			break;
		case TokenType::CloseParenthesis:
			if (previousEquationTokenType == EquationTokenType::OpenParenthesis)
			{
				std::cerr << "Cannot have empty parenthesis" << std::endl;
			}

			if (IsOperator(previousEquationTokenType))
			{
				EquationToken previousOperator = equationTokens.back();
				std::cerr << "Operator '" << previousOperator.Value << "' is missing an operand" << std::endl;
			}
			equationTokenType = EquationTokenType::CloseParenthesis;
			parenthesisCount--;
			break;
		case TokenType::Plus:
			equationTokenType = EquationTokenType::AdditionOperator;

			break;
		case TokenType::Minus:
			equationTokenType = IsOperand(previousEquationTokenType) ||
				previousEquationTokenType == EquationTokenType::CloseParenthesis ?
				EquationTokenType::SubtractionOperator :
				EquationTokenType::NegationOperator;
			break;
		case TokenType::Asterisk:
			equationTokenType = EquationTokenType::MultiplicationOperator;
			break;
		case TokenType::ForwardSlash:
			equationTokenType = EquationTokenType::DivisionOperator;
			break;
		case TokenType::Caret:
			equationTokenType = EquationTokenType::ExponentiationOperator;
			break;
		case TokenType::Equals:
			containsEqualsOperator = true;
			equationTokenType = EquationTokenType::EqualsOperator;
			break;
		case TokenType::Identifier:
			equationTokenType = IdentifierToEquationToken(token.Value);
			if (equationTokenType == EquationTokenType::VariableOperand) // variable operands are only a single character
			{
				for (int i = 0; i < token.Value.length() - 1; i++) // charwise expand identifier
				{
					equationTokens.emplace_back(token.Value.substr(i, 1), EquationTokenType::VariableOperand);
					equationTokens.emplace_back("*", EquationTokenType::MultiplicationOperator);
				}

				equationTokens.emplace_back(token.Value.substr(token.Value.length() - 1, 1), EquationTokenType::VariableOperand);
				
				index++;
				continue;
			}
			break;
		case TokenType::NumberLiteral:
			if (!CanParseNumericalOperand(token))
			{
				std::cerr << "Could not parse number literal '" << token.Value << "'" << std::endl;
			}
			else
			{
				equationTokenType = EquationTokenType::NumericalOperand;
			}

			break;
		default:
			break;
		}

		if (HasImpliedMultiply(previousEquationTokenType, equationTokenType))
		{
			equationTokens.emplace_back("*", EquationTokenType::MultiplicationOperator);
		}

		/* u: unary operator, b: binary operator
			 * ub: sin+    :(
			 * uu: sin sin :)
			 * bb: + /     :(
			 * bu: +sin    :)
			 */
		if (IsOperator(previousEquationTokenType) && IsBinaryOperator(equationTokenType))
		{
			std::cerr << "Expected operand but got operator '" << token.Value << "'" << std::endl;
		}

		if (parenthesisCount < 0)
			std::cerr << "Unmatched close parenthesis";

		if (equationTokenType == EquationTokenType::NumericalOperand)
		{
			equationTokens.emplace_back(token.Value, equationTokenType, std::stod(token.Value));
		}
		else
		{
			equationTokens.emplace_back(token.Value, equationTokenType);
		}

		previousEquationTokenType = equationTokenType;
		index++;
	}

	if (IsOperator(previousEquationTokenType))
		std::cerr << "Expression cannot end with operator" << std::endl;

	if (parenthesisCount != 0)
		std::cerr << "Unmatched open parenthesis" << std::endl;

	// equal operator checking

	return equationTokens;
}

EquationToken::EquationToken(const String& value, EquationTokenType type)
	: Value(value), Type(type), NumericValue(0.0) {}

EquationToken::EquationToken(const String& value, EquationTokenType type, double numericValue)
	: Value(value), Type(type), NumericValue(numericValue)
{
	if (type != EquationTokenType::NumericalOperand)
		std::cerr << "Cannot use numeric constructor for non numerical operand equation token" << std::endl;
}

EquationTokenType IdentifierToEquationToken(const String& identifier)
{
	if (identifierDict.find(identifier) == identifierDict.end())
	{
		return EquationTokenType::VariableOperand;
	}

	return identifierDict[identifier];
}

bool CanParseNumericalOperand(const Token& token)
{
	try
	{
		std::stod(token.Value);
	}
	catch (const std::invalid_argument&)
	{
		return false;
	}

	return true;
}


/*
 * Implied multiply between
 * v: variable operand
 * u: unary operator (excluding negation)
 * n: number operand
 *
 * )v = )*v
 * )u = )*u
 * )( = )*(
 * )n = )*n
 *
 * nv = n*v
 * nu = n*u
 * n( = n*(
 *
 * v( = v*(
 * vu = v*u
 */
bool HasImpliedMultiply(EquationTokenType previousTokenType, EquationTokenType currentTokenType)
{
	// previous token must be ), n, v
	if (previousTokenType != EquationTokenType::CloseParenthesis &&
		!IsOperand(previousTokenType)) return false;


	// current token must be (, u, n, v, and u cannot be the negation operator
	if (currentTokenType != EquationTokenType::OpenParenthesis &&
		!IsUnaryOperator(currentTokenType) &&
		!IsOperand(currentTokenType)) return false;

	if (currentTokenType == EquationTokenType::NegationOperator) return false;

	switch (previousTokenType)
	{
	case EquationTokenType::CloseParenthesis:
		return true;
	case EquationTokenType::NumericalOperand:
		return currentTokenType != EquationTokenType::NumericalOperand;
	case EquationTokenType::VariableOperand:
		return currentTokenType == EquationTokenType::OpenParenthesis ||
			IsUnaryOperator(currentTokenType);
	}

	return false;
}

String EquationToken::ToString() const
{
	return ::ToString(Type) + ": " + Value;
}

String ToString(EquationTokenType type)
{
	return equationTokenTypeDict[type];
}

bool IsParenthesis(EquationTokenType equationTokenType)
{
	return equationTokenType == EquationTokenType::OpenParenthesis
		|| equationTokenType == EquationTokenType::CloseParenthesis;
}

bool IsOperand(EquationTokenType equationTokenType)
{
	return equationTokenType == EquationTokenType::NumericalOperand
		|| equationTokenType == EquationTokenType::VariableOperand;
}

bool IsOperator(EquationTokenType equationTokenType)
{
	return IsUnaryOperator(equationTokenType) || IsBinaryOperator(equationTokenType);
}

bool IsUnaryOperator(EquationTokenType equationTokenType)
{
	return equationTokenType == EquationTokenType::NegationOperator
		|| equationTokenType == EquationTokenType::SineOperator
		|| equationTokenType == EquationTokenType::CosineOperator
		|| equationTokenType == EquationTokenType::TangentOperator
		|| equationTokenType == EquationTokenType::AbsoluteOperator
		|| equationTokenType == EquationTokenType::FloorOperator
		|| equationTokenType == EquationTokenType::CeilingOperator
		|| equationTokenType == EquationTokenType::SquareRootOperator
		|| equationTokenType == EquationTokenType::CubeRootOperator;
}

bool IsBinaryOperator(EquationTokenType equationTokenType) {
	return equationTokenType == EquationTokenType::AdditionOperator
		|| equationTokenType == EquationTokenType::SubtractionOperator
		|| equationTokenType == EquationTokenType::MultiplicationOperator
		|| equationTokenType == EquationTokenType::DivisionOperator
		|| equationTokenType == EquationTokenType::ExponentiationOperator
		|| equationTokenType == EquationTokenType::EqualsOperator;
}

int OperatorPriority(EquationTokenType equationTokenType)
{
	switch (equationTokenType)
	{
	case EquationTokenType::AdditionOperator:
	case EquationTokenType::SubtractionOperator:
		return 1;
	case EquationTokenType::MultiplicationOperator:
	case EquationTokenType::DivisionOperator:
		return 2;
	case EquationTokenType::ExponentiationOperator:
		return 3;
	case EquationTokenType::NegationOperator:
	case EquationTokenType::SineOperator:
	case EquationTokenType::CosineOperator:
	case EquationTokenType::TangentOperator:
	case EquationTokenType::AbsoluteOperator:
	case EquationTokenType::FloorOperator:
	case EquationTokenType::CeilingOperator:
	case EquationTokenType::SquareRootOperator:
	case EquationTokenType::CubeRootOperator:
		return 4;
	default:
		std::cerr << "Operator priority undefined for equation token type: " << ToString(equationTokenType) << std::endl;
		return -1;
	}
}