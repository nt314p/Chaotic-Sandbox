#include "Token.h"
#include "Common.h"

std::unordered_map<TokenType, String> tokenTypeDict =
{
	{ TokenType::Bad, "Bad" },
	{ TokenType::Whitespace, "Whitespace" },
	{ TokenType::OpenParenthesis, "OpenParenthesis" },
	{ TokenType::CloseParenthesis, "CloseParenthesis" },
	{ TokenType::Plus, "Plus" },
	{ TokenType::Minus, "Minus" },
	{ TokenType::Asterisk, "Asterisk" },
	{ TokenType::ForwardSlash, "ForwardSlash" },
	{ TokenType::Caret, "Caret" },
	{ TokenType::Equals, "Equals" },
	{ TokenType::Identifier, "Identifier" },
	{ TokenType::NumberLiteral, "NumberLiteral" }
};


Token::Token(const String& value, TokenType type)
	: Value(value), Type(type) {}

String Token::ToString() const
{
	return ::ToString(Type) + ": " + Value;
}

std::vector<Token> Tokenize(const String& expression)
{
	std::vector<Token> tokens;
	tokens.reserve(20);

	String tokenValue;
	TokenType tokenType = TokenType::Bad;

	unsigned int cursorIndex = 0;
	unsigned int multiCharStartIndex = 0;

	while (cursorIndex < expression.length())
	{
		tokenType = TokenType::Bad;
		multiCharStartIndex = cursorIndex;
		char currentToken = expression[cursorIndex];
		bool isSingleCharacterToken = true;

		if (IsNumerical(currentToken))
		{
			while (cursorIndex < expression.length() && IsNumerical(expression[cursorIndex]))
			{
				cursorIndex++;
			}

			tokenType = TokenType::NumberLiteral;
			isSingleCharacterToken = false;
		}

		if (IsLetter(currentToken)) {
			while (cursorIndex < expression.length() && IsLetter(expression[cursorIndex]))
			{
				cursorIndex++;
			}

			tokenType = TokenType::Identifier;
			isSingleCharacterToken = false;
		}

		switch (currentToken)
		{
		case '\t':
		case ' ':
			tokenType = TokenType::Whitespace;
			break;
		case '(':
			tokenType = TokenType::OpenParenthesis;
			break;
		case ')':
			tokenType = TokenType::CloseParenthesis;
			break;
		case '+':
			tokenType = TokenType::Plus;
			break;
		case '-':
			tokenType = TokenType::Minus;
			break;
		case '*':
			tokenType = TokenType::Asterisk;
			break;
		case '/':
			tokenType = TokenType::ForwardSlash;
			break;
		case '^':
			tokenType = TokenType::Caret;
			break;
		case '=':
			tokenType = TokenType::Equals;
			break;
		}

		if (isSingleCharacterToken)
		{
			cursorIndex++;
			tokenValue = currentToken;
		}
		else
		{
			tokenValue = expression.substr(multiCharStartIndex, cursorIndex - multiCharStartIndex);
		}

		tokens.emplace_back(tokenValue, tokenType);
	}

	return tokens;
}

String ToString(TokenType type)
{
	return tokenTypeDict[type];
}

bool IsNumerical(char token)
{
	return std::isdigit(token) || token == '.';
}

bool IsLetter(char token)
{
	return std::isalpha(token);
}