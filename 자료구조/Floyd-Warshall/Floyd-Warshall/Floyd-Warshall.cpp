#include <stdio.h>
#include <iostream>
#include <vector>
#include <queue>
#include <climits>
#include <stack>

using namespace std;

int num = 4;
int INF = 1000000;

int a[4][4] = {
	{0,5,INF,8},
	{7,0,9,INF},
	{2,INF,0,4},
	{INF,INF,3,0}
};


void FW()
{
	int d[4][4];

	for (int i = 0; i < num; i++)
	{
		for (int j = 0; j < num; j++)
		{
			d[i][j] = a[i][j];

		}
	}

	//���İ��� ���
	for (int k = 0; k < num; k++)
	{	// ��߳��
		for (int i = 0; i < num ; i++)
		{	// �������
			for (int j = 0; j < num; j++)
			{
				if (d[i][k] + d[k][j] < d[i][j])
					d[i][j] = d[i][k]+  d[k][j];
			}
		}
	}
	for (int i = 0; i < num; i++)
	{
		for (int j = 0; j < num; j++)
		{
			printf("%d " ,d[i][j]);

		}
		printf("\n ");
	}


}
int main()
{
	FW();
	return 0;
}