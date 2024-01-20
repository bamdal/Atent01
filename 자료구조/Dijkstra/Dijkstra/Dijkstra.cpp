#include <stdio.h>
#include <iostream>
#include <vector>
#include <queue>
#include <climits>
#include <stack>

using namespace std;

struct Edge
{
	int to;
	int weight;

	Edge(int t, int w) : to(t), weight(w) {}

	bool operator > (const Edge& other) const {
		return weight > other.weight;
	}
};


vector<vector<int>> Dijkstra(const vector<vector<Edge>>& g, int start)
{
	int V = g.size();
	vector<vector<int>> distance(V, vector<int>(V, INT_MAX));
	vector<vector<int>> paths(V, vector<int>(V, -1));

	priority_queue<Edge, vector<Edge>, greater<Edge>> pq;//�켱���� ť
	pq.push(Edge(start, 0));
	distance[start][start] = 0;
	while (!pq.empty())
	{
		Edge current = pq.top();
		pq.pop();
		
		int u = current.to;
		int weight_u = current.weight;

		for (const Edge& neighbor : g[u])
		{
			int v = neighbor.to;
			int weight_v = neighbor.weight;

			// ���ι߰��� ����� ����ġ�� ���� ���� ����ġ���� �۴ٸ� ����
			if (distance[start][u] + weight_v < distance[start][v])
			{
				distance[start][v] = distance[start][u] + weight_v;
				paths[start][v] = u;
				pq.push(Edge(v, distance[start][v]));
			}
		}

		
	}
	return paths;
}

stack<int> constructPath(const vector<vector<int>>& paths, int start, int dest)
{
	stack<int> pathstack;
	int current = dest;

	while (current != start)
	{
		pathstack.push(current);
		current = paths[start][current];
	}

	pathstack.push(start);
	return pathstack;
}

int main()
{


	vector<vector<Edge>> g = {
		{{1,2},{2,5},{3,1}},
		{{0,2},{2,3},{3,2}},
		{{0,5},{1,3},{3,3},{4,1},{5,5}},
		{{0,1},{1,2},{2,3},{4,1}},
		{{2,1},{3,1},{5,2}},
		{{2,5},{4,2}}
	};

	int startnode = 0;
	int destnode = 5;

	// ���ͽ�Ʈ�� �˰��� ȣ��
	vector<vector<int>> shortestPaths = Dijkstra(g, startnode);

	// �ִ� ��� ���
	stack<int> pathStack = constructPath(shortestPaths, startnode, destnode);
	cout << "���� " << startnode << "���� ���� " << destnode << "���� �ִ� ���: ";
	while (!pathStack.empty()) {
		cout << pathStack.top() << " ";
		pathStack.pop();
	}
	cout << endl;





	return 0;
}