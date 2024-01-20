#include<stdio.h>
#include<iostream>


int f(int num)
{
	if (num % 2 == 0||num<0)
		return 0;
	else
	{
		static int a = 65;
		static int stack = 0;
		static int stack2 = 0;
		stack++;
		f(num - 2);
		if (a > 90)
			a = 65;
		for(int i=0;i<stack;i++)
			printf(" ");
		stack2++;
		for (int j = 0; j < stack2; j++)
		{
			stack--;

		printf("%c", a);
		a++;
		}
		printf("\n");
		return a;
	}

}

void main()
{
	int num;
	scanf_s("%d", &num);
	f(num);
}
