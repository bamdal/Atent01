#include <iostream>

#define MAX_SIZE 100

typedef struct {
    int items[MAX_SIZE]; // 시간 제한
    int front;
    int rear;
} Queue;

// 큐 초기화 함수
void initialize(Queue* q) {
    q->front = -1;
    q->rear = -1;
}

// 큐가 비어있는지 확인하는 함수
int isEmpty(Queue* q) {
    return q->front == -1;
}

// 큐가 가득 차있는지 확인하는 함수
int isFull(Queue* q) {
    return (q->rear + 1) % MAX_SIZE == q->front;
}

// 큐에 요소 추가하는 함수
void enqueue(Queue* q, int value) {
    if (isFull(q)) {
        printf("큐가 가득 찼습니다.\n");
        return;
    }

    if (isEmpty(q)) {
        q->front = 0;
    }

    q->rear = (q->rear + 1) % MAX_SIZE;
    q->items[q->rear] = value;
}

// 큐에서 요소 제거하는 함수
int dequeue(Queue* q) {
    int value;

    if (isEmpty(q)) {
        printf("큐가 비어 있습니다.\n");
        return -1;
    }

    value = q->items[q->front];

    if (q->front == q->rear) {
        // 큐에 하나의 요소만 남아있는 경우
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
