#include <stdio.h>
#include <iostream>


unsigned long long int self_fibonacci(int n) 
{
	if (n <= 1) { return n; }
	return self_fibonacci(n - 1) + self_fibonacci(n-2);
}
#define N 100
unsigned long long int memo[N]{};
unsigned long long int memo_fibonacci(int n)
{
	if (n <= 1)
		return n;
	if (memo[n] != 0)
	{
		return memo[n];
	}

	memo[n] = memo_fibonacci(n - 1) + memo_fibonacci(n - 2);
	return memo[n];

};



int main()
{
	int input_n = 0;

	//while (!(input_n % 2))
	//{
	//	std::cout << "È¦¼ö";
	//	std::cin >> input_n;
	//}
	std::cin >> input_n;
	int b = 0;


	//for (int i = 0; i < input_n; i++)
	//{
	//	for (int c = 0; c < input_n-i-1;c++)
	//	{
	//		std::cout << " ";
	//	}
	//	for (int j = 0; j < i; j++)
	//	{
	//		std::cout << (char)(65 + (b++ % 26));
	//	}
	//	std:: cout<<std::endl;
	//}


	//for (int i = 0; i < input_n; i++)
	//{
	//	for (int j = 0; j < input_n ; j++)
	//	{
	//		if(0 >= input_n - (i+j) -1)
	//			std::cout << (char)(65 + (b++ % 26));
	//		else
	//			std::cout << " ";
	//	}
	//	std::cout << std::endl;
	//}
	

	for (int i = 0; i < input_n; i++)
	{
		std::cout << memo_fibonacci(i) << std::endl;
	}

	return 0;
}

