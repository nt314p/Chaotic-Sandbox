#include "Common.h"
#include "Token.h"
#include "EquationToken.h"

int main()
{
	String expression = "c^2 = a^2 + b^2 - 2abdfe cos(C) + nop";

	std::vector<Token> tokens = Tokenize(expression);
	std::vector<EquationToken> equationTokens = ConvertToEquationTokens(tokens);

	std::cout << "Tokens:" << std::endl;
	for (const Token& token : tokens)
		std::cout << token.ToString() << std::endl;

	std::cout << "\nEquation Tokens:" << std::endl;
	for (const EquationToken& equationToken : equationTokens)
		std::cout << equationToken.ToString() << std::endl;

	std::cout << "\nDone!" << std::endl;

	std::cin.get();
	return 0;
}