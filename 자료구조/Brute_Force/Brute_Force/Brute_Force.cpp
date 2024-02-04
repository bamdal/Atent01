#include <stdio.h>
#include <iostream>

using namespace std;

int findString(string parent, string pattern)
{
	int parentSize = parent.size();
	int patternSize = pattern.size();

	for (int i = 0; i <= parentSize - patternSize; i++)
	{
		bool finded = true;
		for (int j = 0; j < patternSize; j++)
		{
			if (parent[i + j] != pattern[j])
			{
				finded = false;
			}
		}
		if (finded)
		{
			return i+1;
		}
	}
	return -1;
}

int main(void)
{
	string parent = "Hello World!";
	string pattern = "llo W";


	int result = findString(parent, pattern);

	if (result == -1)
		cout << "못찾았습니다." << endl;
	else
		cout << result << "번째에서 찾았습니다." << endl;

	return 0;

}