#include <iostream>
#include <string>


namespace {

std::size_t commonStart(const std::string &word1, const std::string &word2)
{
	const std::size_t max = std::max(word1.length(), word2.length());
	size_t last_utf8 = 0;
	for (std::size_t i = 0; i != max; ++i)
	{
		if ((word1[i] & 0x80) == 0)
			last_utf8 = i;
		if (word1[i] != word2[i])
			return last_utf8;
	}
	return max;
}

}

int main()
{
	std::string prevWord = " ";
	while (std::cin)
	{
		std::string word;
		std::getline(std::cin, word);
		if (word.empty())
			continue;
		const std::size_t common = commonStart(word, prevWord);
		const std::string &compWord =
				common != 0 ?
				std::to_string(common) + word.substr(common) :
				word;
		std::cout << compWord << '\n';
		prevWord = std::move(word);
	}
	std::cout << std::flush;
	
	return 0;
}
