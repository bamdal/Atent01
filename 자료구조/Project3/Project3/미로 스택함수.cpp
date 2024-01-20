#include <iostream>


#define SIZE 5
// �̷��� ũ��� �̷� �迭 ����
int Maze[SIZE][SIZE] = {
   {0, 1, 0, 1, 1},
   {0, 0, 0, 0, 0},
   {1, 0, 1, 0, 1},
   {1, 0, 1, 0, 0},
   {1, 1, 0, 1, 0}
};

// �湮�� ��θ� �����ϴ� ���� ����
struct stack {
    int route_X[SIZE * SIZE];
    int route_Y[SIZE * SIZE];
    int top;
};

// ���� �ʱ�ȭ �Լ�
void Init(struct stack* s) {
    s->top = -1;
}

// ���ÿ� ��ġ �߰� �Լ�
void push(struct stack* s, int x, int y) {
    if (s->top < SIZE * SIZE - 1) {
        s->top++;
        s->route_X[s->top] = x;
        s->route_Y[s->top] = y;
    }
}

// ���ÿ��� ��ġ ���� �Լ�
void pop(struct stack* s) {
    if (s->top >= 0) {
        s->top--;
    }
}

// �̷θ� ����ϴ� �Լ�
void Maze_Print() {
    for (int i = 0; i < SIZE; i++) {
        for (int j = 0; j < SIZE; j++) {
            if (Maze[i][j] == 0) {
                printf("�� ");
            }
            else {
                printf("�� ");
            }
        }
        printf("\n");
    }
}

// �̷θ� �������Ͽ� ��θ� ã�� �Լ�
int backtracking(struct stack* s, int x, int y) {
    if (x < 0 || y < 0 || x >= SIZE || y >= SIZE || Maze[x][y] == 1) {
        return 0; // ���� �ε����ų� �̷� ���� ������ ������ ����
    }
    if (x == SIZE - 1 && y == SIZE - 1) {
        // �������� �������� ��
        push(s, x, y);
        return 1;
    }

    // ���� ��ġ�� ���ÿ� �߰�
    push(s, x, y);

    // ���������� �̵�
    if (backtracking(s, x, y + 1)) {
        return 1;
    }
    // �Ʒ��� �̵�
    if (backtracking(s, x + 1, y)) {
        return 1;
    }

    // �̵��� ������ ��� ���� ��ġ�� ���ÿ��� ����
    pop(s);
    return 0;
}

int main() {
    struct stack s;
    Init(&s);
    printf("�̷�:\n");
    Maze_Print();

    printf("\n���:\n");
    if (backtracking(&s, 0, 0)) {
        for (int i = 0; i <= s.top; i++) {
            printf("(%d, %d) -> ", s.route_X[i], s.route_Y[i]);
        }
        printf("�������� �����߽��ϴ�.\n");
    }
    else {
        printf("�������� ������ �� �����ϴ�.\n");
    }

    return 0;
}