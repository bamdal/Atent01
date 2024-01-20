#include <iostream>
#include <stack>
#include <vector>

using namespace std;

const int SIZE = 5;

// 미로의 크기와 미로 배열 정의
int Maze[SIZE][SIZE] = {
    {0, 1, 0, 1, 1},
    {0, 0, 0, 0, 0},
    {1, 0, 1, 0, 1},
    {1, 0, 1, 0, 0},
    {1, 1, 0, 1, 0}
};

struct Point {
    int x, y;
};

//// 미로를 출력하는 함수
void Maze_Print() {
    for (int i = 0; i < SIZE; i++) {
        for (int j = 0; j < SIZE; j++) {
            if (Maze[i][j] == 0) {
                printf("□ ");
            }
            else {
                printf("■ ");
            }
        }
        printf("\n");
    }
}

bool isValid(int x, int y) {
    return x >= 0 && x < SIZE && y >= 0 && y < SIZE && Maze[x][y] == 0;
}

void printPath(const vector<Point>& path) {
    cout << "최단 경로: ";
    for (const Point& p : path) {
        cout << "(" << p.x << ", " << p.y << ") -> ";
    }
    cout << "도착지" << endl;
}

void dfs(int startX, int startY, int destX, int destY) {
    stack<Point> s;
    vector<Point> path;  // 경로 저장

    s.push({ startX, startY });
    path.push_back({ startX, startY });

    while (!s.empty()) {
        Point current = s.top();
        s.pop();

        int x = current.x;
        int y = current.y;

        // 목적지에 도달한 경우
        if (x == destX && y == destY) {
            cout << "목적지에 도달했습니다." << endl;
            printPath(path);
            return;
        }

        // 방문한 위치는 표시
        Maze[x][y] = 2;

        // 상하좌우 이동
        int dx[] = { -1, 1, 0, 0 };
        int dy[] = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; ++i) {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (isValid(newX, newY)) {
                s.push({ newX, newY });
                path.push_back({ newX, newY });
            }
        }
    }

    cout << "목적지에 도달할 수 없습니다." << endl;
}

int main() {
    int startX = 0, startY = 0;
    int destX = 1, destY = 3;
    //int destX = 4, destY = 4;

    Maze_Print();

    dfs(startX, startY, destX, destY);

    return 0;
}