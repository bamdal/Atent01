#include <stdio.h>
#include <iostream>
#include <vector>
#include <queue>
using namespace std;

#define MAX 10001

struct Edge {
    int node;
    int time;

    Edge(int n, int t) : node(n), time(t) {}
};


int n, start, finish;
int inDegree[MAX], result[MAX], c[MAX];
vector<Edge> a[MAX];
vector<Edge> b[MAX];

void topolgySort() {
    queue<int> q;
    q.push(start);
    while (!q.empty())
    {
        int x = q.front();

        q.pop();

        for (int i = 0; i < a[x].size(); i++)
        {
            Edge y = Edge(a[x][i].node,a[x][i].time);

            if (result[y.node] <= y.time + result[x])
            {
                result[y.node] = y.time + result[x];
            }
            if (--inDegree[y.node] == 0)
            {
                q.push(y.node);
            }

        }

    }

    int count = 0;
    q.push(finish);

    while (!q.empty())
    {
        int y = q.front();

        q.pop();

        for (int i = 0; i < b[y].size(); i++)
        {
            Edge x = Edge(b[y][i].node, b[y][i].time);

            if (result[y] -result[x.node]==x.time)
            {
                count++;
                if (c[x.node] == 0)
                {
                    q.push(x.node);
                    c[x.node] = 1;
                }

            }

        }


    }

    cout << result[finish] << endl << count << endl;
}

int main(void)
{
    int m;
    cin >> n >> m;

    for (int i = 0; i < m; i++)
    {
        int x, node, time;
        cin >> x >> node >> time;

        a[x].push_back(Edge(node, time));
        b[node].push_back(Edge(x, time));

        inDegree[node]++;


    }
 
    cin >> start >> finish;
    cout << "Start = " << start << " " << "finish = " << finish << endl;
    topolgySort();

    return 0;
}