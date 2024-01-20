#include<stdio.h>
#include<iostream>
#define MAX_STACK_SIZE 100
unsigned long long f(int n)
{
	if (n <= 0) return 0;
	else
	{
		unsigned int result = 0;
		unsigned int a = 0, b = 1;

		for (int i = 2; i <= n; i++)
		{
			result = a + b;
			a = b;;
			b = result;
		}
		return result;
	}

}

struct stack {
	int arr[MAX_STACK_SIZE];
	int top;
};

void init(struct stack* a)
{
	a->top = -1;
}
int pop(struct stack* s)
{
	if (s->top >= 0)
		return s->arr[s->top--];
	else
		printf("없다");
	return -1;
}

void push(struct stack* s, int value)
{
	if (s->top < MAX_STACK_SIZE - 1)
	{
		s->top++;
		s->arr[s->top] = value;
	}
}

int Factorial(int n, struct stack* s,int m)
{

	if (n == 0 || n == 1)
		return 1;
	else
	{
		push(s, n);
	
		int re = Factorial(n - 1, s,m);
	
		int cur = pop(s);

		printf("%d! = ", cur-1);
		printf("%d  \n", re);
		if (n == m) {
			printf("%d! = ", cur);
			printf("%d  \n", re * cur);
		}
		return re * cur;
	}
}
void main()
{
	//std::cout << "피보나치 : " ;
	//for (int n = 0; n < 200; n++)
	//{
	//	
	//	std::cout << f(n) << ", ";
	//}

	struct stack s;
	init(&s);
	int target = 6;
	int res = Factorial(target, &s, target);
	
}
