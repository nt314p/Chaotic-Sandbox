#pragma once
#include "Token.h"
#include "Common.h"

enum class EquationTokenType
{
	Bad,

	OpenParenthesis,
	CloseParenthesis,

	AdditionOperator,
	SubtractionOperator,
	MultiplicationOperator,
	DivisionOperator,
	ExponentiationOperator,
	NegationOperator,
	SineOperator,
	CosineOperator,
	TangentOperator,
	AbsoluteOperator,
	FloorOperator,
	CeilingOperator,
	SquareRootOperator,
	CubeRootOperator,
	EqualsOperator,

	VariableOperand,
	NumericalOperand
};

class EquationToken
{
public:
	String Value;
	EquationTokenType Type;
	double NumericValue;
	EquationToken(const String& value, EquationTokenType type);
	EquationToken(const String& value, EquationTokenType type, double numericValue);
	String ToString() const;
};

std::vector<EquationToken> ConvertToEquationTokens(std::vector<Token>& tokens);

String ToString(EquationTokenType type);

bool CanParseNumericalOperand(const Token& token);
bool HasImpliedMultiply(EquationTokenType previousTokenType, EquationTokenType currentTokenType);

EquationTokenType IdentifierToEquationToken(const String& identifier);

bool IsParenthesis(EquationTokenType equationTokenType);
bool IsOperand(EquationTokenType equationTokenType);
bool IsOperator(EquationTokenType equationTokenType);
bool IsUnaryOperator(EquationTokenType equationTokenType);
bool IsBinaryOperator(EquationTokenType equationTokenType);
int OperatorPriority(EquationTokenType equationTokenType);