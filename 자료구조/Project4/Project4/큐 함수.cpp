#include <iostream>

#define MAX_SIZE 100

typedef struct {
    int items[MAX_SIZE]; // �ð� ����
    int front;
    int rear;
} Queue;

// ť �ʱ�ȭ �Լ�
void initialize(Queue* q) {
    q->front = -1;
    q->rear = -1;
}

// ť�� ����ִ��� Ȯ���ϴ� �Լ�
int isEmpty(Queue* q) {
    return q->front == -1;
}

// ť�� ���� ���ִ��� Ȯ���ϴ� �Լ�
int isFull(Queue* q) {
    return (q->rear + 1) % MAX_SIZE == q->front;
}

// ť�� ��� �߰��ϴ� �Լ�
void enqueue(Queue* q, int value) {
    if (isFull(q)) {
        printf("ť�� ���� á���ϴ�.\n");
        return;
    }

    if (isEmpty(q)) {
        q->front = 0;
    }

    q->rear = (q->rear + 1) % MAX_SIZE;
    q->items[q->rear] = value;
}

// ť���� ��� �����ϴ� �Լ�
int dequeue(Queue* q) {
    int value;

    if (isEmpty(q)) {
        printf("ť�� ��� �ֽ��ϴ�.\n");
        return -1;
    }

    value = q->items[q->front];

    if (q->front == q->rear) {
        // ť�� �ϳ��� ��Ҹ� �����ִ� ���
        initialize(q);
    }
    else {
        q->front = (q->front + 1) % MAX_SIZE;
    }

    return value;
}

int main() {
    Queue myQueue;
    initialize(&myQueue);

    enqueue(&myQueue, 1);
    enqueue(&myQueue, 2);
    enqueue(&myQueue, 3);

    printf("Dequeued: %d\n", dequeue(&myQueue));
    printf("Dequeued: %d\n", dequeue(&myQueue));

    enqueue(&myQueue, 4);

    printf("Dequeued: %d\n", dequeue(&myQueue));
    printf("Dequeued: %d\n", dequeue(&myQueue));

    return 0;
}
