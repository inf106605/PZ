#include <iostream>
#include <string>


namespace {

std::size_t numberAtBegining(std::string &word)
{
	std::size_t result = 0;
	std::size_t i;
	for (i = 0; word[i] >= '0' && word[i] <= '9'; ++i)
	{
		result *= 10;
		result += word[i] - '0';
	}
	if (result != 0)
		word = word.substr(i);
	return result;
}

}

int main()
{
	unsigned i = 0;
	std::string prevWord = " ";
	while (std::cin)
	{
		std::string word;
		std::getline(std::cin, word);
		if (word.empty())
			continue;
		const std::size_t common = numberAtBegining(word);
		const std::string &decompWord =
				common != 0 ?
				prevWord.substr(0, common) + word :
				word;
		std::cout << decompWord << '\n';
		prevWord = std::move(decompWord);
		if (++i % 100 == 0)
			std::cerr << '\r' << i << std::flush;
	}
	std::cout << std::flush;
	
	return 0;
}
