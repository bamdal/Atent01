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
		cout << "��ã�ҽ��ϴ�." << endl;
	else
		cout << result << "��°���� ã�ҽ��ϴ�." << endl;

	return 0;

}