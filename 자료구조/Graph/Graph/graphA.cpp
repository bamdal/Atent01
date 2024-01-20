#include<stdio.h>
#include<vector>
#include<iostream>
#include<math.h>
#include<algorithm>
#include<list>
#include<stack>
#include <queue>
using namespace std;

struct Graph {
    int NumVertices;
    vector<vector<int>> adMatrix;

    Graph(int vectices) : NumVertices(vectices), adMatrix(vectices, vector<int>(vectices, 0)) {}

    void addEdge(int start, int end)
    {
        adMatrix[start][end] = 1;
        adMatrix[end][start] = 1;
    }

    void printGraph()
    {
        cout << "인접행렬 표현" << endl;
        for (int i = 0; i < NumVertices; i++)
        {
            for (int j = 0; j < NumVertices; j++)
            {
                cout << adMatrix[i][j] << " ";
            }
            cout << endl;
        }
    }
};


struct GraphList {
    int NumVertices;
    vector<list<int>> adList;

    GraphList(int vectices) : NumVertices(vectices), adList(vectices) {}

    void addEdge(int start, int end)
    {
        adList[start].push_back(end);
        adList[end].push_back(start);
    }

    void DFShelp(int vertex, vector <bool>& visited) // 깊이 우선 탐색
    {
        visited[vertex] = true;

        cout << vertex << " ";

        for (auto& nei : adList[vertex])
        {
            if (!visited[nei])
                DFShelp(nei, visited);
        }
    }

    void DFS_Start(int startVertex)
    {
        vector<bool> visited(NumVertices, false);

        cout << "DFS 시작 " << startVertex << " : ";

        DFShelp(startVertex, visited);
        cout << endl;

    }

    void DFS(int vertex)
    {
        vector<bool> visited(NumVertices, false);
        stack<int> stack;

        cout << "DFS 시작 " << vertex << " : ";

        stack.push(vertex);

        while (!stack.empty())
        {
            int currentvertex = stack.top();

            stack.pop();

            if (!visited[currentvertex])
            {
                visited[currentvertex] = true;
                cout << currentvertex << " ";

                for (auto& nei : adList[currentvertex])
                {
                    if (!visited[nei])
                    {
                        stack.push(nei);
                    }
                }
            }
        }
        cout << endl;
    }

    void BFS(int vertex)
    {
        vector<bool> visited(NumVertices, false);
        queue<int> queue;

        cout << "BFS 시작 " << vertex << " : ";
        visited[vertex] = true;
        queue.push(vertex);

        while (!queue.empty())
        {
            int cur = queue.front();

            queue.pop();
            cout << cur << " ";

            for (auto& nei : adList[cur])
            {
                if (!visited[nei])
                {
                    visited[nei] = true;
                    queue.push(nei);
                } 

            }
        }
        cout << endl;
    }

    void printGraph()
    {
        cout << "인접리스트 표현" << endl;
        for (int i = 0; i < NumVertices; i++)
        {
            cout << i << " -> ";
            for (auto& nei : adList[i])
            {
                cout << nei << " ";
            }
            cout << endl;
        }
    }
};

int main()
{
    GraphList gl(10);
    Graph g(4);

    //gl.addEdge(0, 1);
    //gl.addEdge(0, 3);
    //gl.addEdge(1, 2);
    //gl.addEdge(1, 5);
    //gl.addEdge(5, 6);
    //gl.addEdge(2, 6);
    gl.addEdge(0,1 );
    gl.addEdge(0,2 );
    gl.addEdge(1,3 );
    gl.addEdge(1,4 );
    gl.addEdge(1,5 );
    gl.addEdge(5,6 );
    gl.addEdge(5,4 );
    gl.addEdge(6,4 );
    gl.addEdge(2,3 );
    gl.addEdge(7,3 );


    gl.DFS_Start(0);
    gl.DFS(0);
    gl.BFS(0);
    gl.printGraph();

    g.addEdge(0, 1);
    g.addEdge(0, 3);
    g.addEdge(1, 2);

    g.printGraph();

    return 0;
}