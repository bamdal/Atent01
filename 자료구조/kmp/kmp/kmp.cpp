#include <stdio.h>
#include <iostream>
#include <vector>
using namespace std;


void computeLPSArray(string& pattern, vector<int>& lps)
{
	int m = pattern.length();
	int len = 0;

	lps.resize(m);
	lps[0] = 0;
	
	int i = 1;

	while (i<m)
	{
		if (pattern[i] == pattern[len])
		{
			len++;
			lps[i] = len;
			i++;
		}
		else {
			if (len != 0)
			{
				len = lps[len - 1];
			}
			else {
				lps[i] = 0;
				i++;
			}
		}
	}

}

void KMPSearch(string& text, string& pattern)
{
	int n = text.length();
	int m = pattern.length();

	vector<int> lps;
	computeLPSArray(pattern, lps);

	int i = 0;
	int j = 0;

	while (i < n)
	{
		if (pattern[j] == text[i])
		{
			i++; j++;
		}

		if (j == m)
		{
			cout << i - j << "에서 발견되었습니다." << endl;
			j = lps[j - 1];
		}
		else if (i < n && pattern[j] != text[i]) {
			if (j != 0)
			{
				j = lps[j - 1];
			}
			else
				i++;
		}
	}

}


int main(void)
{
	string parent = "Hello World! Hello World!";
	string pattern = "llo W";

	//int result = findString(parent, pattern);

	KMPSearch(parent, pattern);

	return 0;
}