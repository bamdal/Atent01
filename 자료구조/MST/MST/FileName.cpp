#include<stdio.h>
#include<iostream>
#include <vector>


using namespace std;

struct Edge
{
	int to;
	int weight;

	Edge(int t, int w) : to (t), weight(w) {}
};

struct Compare
{
	bool operator() (const Edge& e1, const Edge& e2)
	{
		return e1.weight > e2.weight;
	}
};

int prim(vector<vector<Edge>>& g)
{
	int VNum = g.size(); // 정점 수
	vector<int> minWeight(VNum, INT16_MAX);
	vector<int> parent(VNum, -1);
	vector<bool> inMST(VNum, false);

	
	int startVertex = 0; // 시작점
	minWeight[startVertex] = 0;

	for (int i = 0; i < VNum - 1; i++)
	{
		int minVertex = -1;
		for (int v = 0; v < VNum; v++)
		{
			if (!inMST[v] && (minVertex == -1 || minWeight[v] < minWeight[minVertex]))
			{
				minVertex = v;
			}
		}

		inMST[minVertex] = true;

		for (const Edge& e : g[minVertex])
		{
			int toVertex = e.to;
			int weight = e.weight;

			if (!inMST[toVertex] && (weight < minWeight[toVertex]))
			{
				minWeight[toVertex] = weight;
				parent[toVertex] = minVertex;
			}
		}


	}

	int total = 0;
	for (int v = 0; v < VNum; v++)
	{
		total += minWeight[v];
	}
	return total;

}

int main()
{
	//vector<vector<Edge>> graph = {
	//	{{1,2},{2,1},{3,3}},
	//	{{0,2},{2,2},{4,1}},
	//	{{0,1},{1,2},{4,4}},
	//	{{0,3},{4,5}},
	//	{{1,1},{2,4},{3,5}}
	//};

	vector<vector<Edge>> graph = {
		{{1,2},{2,5},{3,1}},
		{{0,2},{2,3},{3,2}},
		{{0,5},{1,3},{3,3},{4,1},{5,5}},
		{{0,1},{1,2},{2,3},{4,1}},
		{{2,1},{3,1},{5,2}},
		{{2,5},{4,2}}
	};

	//vector<vector<Edge>> g(7);

	//g[0].push_back(Edge(1,2 ));
	//g[0].push_back(Edge(2, 5));
	//g[0].push_back(Edge(3, 1));
	//g[1].push_back(Edge(0,2 ));
	//g[1].push_back(Edge(2,3 ));
	//g[1].push_back(Edge(3, 2));
	//g[2].push_back(Edge(1,3 ));
	//g[2].push_back(Edge(0,5 ));
	//g[2].push_back(Edge(3,3 ));
	//g[2].push_back(Edge(4, 1));
	//g[2].push_back(Edge(5, 5));
	//g[3].push_back(Edge(0, 1));
	//g[3].push_back(Edge(1, 2));
	//g[3].push_back(Edge(2, 3));
	//g[3].push_back(Edge(4, 1));
	//g[4].push_back(Edge(2,1 ));
	//g[4].push_back(Edge(3, 1));
	//g[4].push_back(Edge(5, 2));
	//g[5].push_back(Edge(4,2 ));
	//g[5].push_back(Edge(2,5 ));


	int MSTWeight = prim(graph);

	cout << "최소신장 트리 가중치 합 :  " << MSTWeight << endl;

	return 0;
}