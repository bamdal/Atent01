#include <iostream>


#define SIZE 5
// 미로의 크기와 미로 배열 정의
int Maze[SIZE][SIZE] = {
   {0, 1, 0, 1, 1},
   {0, 0, 0, 0, 0},
   {1, 0, 1, 0, 1},
   {1, 0, 1, 0, 0},
   {1, 1, 0, 1, 0}
};

// 방문한 경로를 저장하는 스택 정의
struct stack {
    int route_X[SIZE * SIZE];
    int route_Y[SIZE * SIZE];
    int top;
};

// 스택 초기화 함수
void Init(struct stack* s) {
    s->top = -1;
}

// 스택에 위치 추가 함수
void push(struct stack* s, int x, int y) {
    if (s->top < SIZE * SIZE - 1) {
        s->top++;
        s->route_X[s->top] = x;
        s->route_Y[s->top] = y;
    }
}

// 스택에서 위치 제거 함수
void pop(struct stack* s) {
    if (s->top >= 0) {
        s->top--;
    }
}

// 미로를 출력하는 함수
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

// 미로를 역추적하여 경로를 찾는 함수
int backtracking(struct stack* s, int x, int y) {
    if (x < 0 || y < 0 || x >= SIZE || y >= SIZE || Maze[x][y] == 1) {
        return 0; // 벽에 부딪히거나 미로 범위 밖으로 나가면 실패
    }
    if (x == SIZE - 1 && y == SIZE - 1) {
        // 목적지에 도달했을 때
        push(s, x, y);
        return 1;
    }

    // 현재 위치를 스택에 추가
    push(s, x, y);

    // 오른쪽으로 이동
    if (backtracking(s, x, y + 1)) {
        return 1;
    }
    // 아래로 이동
    if (backtracking(s, x + 1, y)) {
        return 1;
    }

    // 이동에 실패한 경우 현재 위치를 스택에서 제거
    pop(s);
    return 0;
}

int main() {
    struct stack s;
    Init(&s);
    printf("미로:\n");
    Maze_Print();

    printf("\n경로:\n");
    if (backtracking(&s, 0, 0)) {
        for (int i = 0; i <= s.top; i++) {
            printf("(%d, %d) -> ", s.route_X[i], s.route_Y[i]);
        }
        printf("목적지에 도달했습니다.\n");
    }
    else {
        printf("목적지에 도달할 수 없습니다.\n");
    }

    return 0;
}