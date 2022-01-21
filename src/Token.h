#pragma once
#include "Common.h"

enum class TokenType : unsigned char
{
	Bad,

	Whitespace,
	OpenParenthesis,
	CloseParenthesis,

	Plus,
	Minus,
	Asterisk,
	ForwardSlash,
	Caret,
	Equals,

	Identifier,
	NumberLiteral
};

class Token
{
public:
	String Value;
	TokenType Type;

	Token(const String& value, TokenType type);
	String ToString() const;
};

std::vector<Token> Tokenize(const String& expression);
String ToString(TokenType type);
bool IsNumerical(char token);
bool IsLetter(char token);