#include <stdio.h>
#include <malloc.h>

void BubbleSort(int array[], int size) {
    for (int i = 0; i < size - 1; i++) {
        for (int j = 0; j < size - i - 1; j++) {
            if (array[j] > array[j + 1]) {
                // 인접한 요소 교환
                int temp = array[j];
                array[j] = array[j + 1];
                array[j + 1] = temp;
            }
        }
    }
}

void InsertSort(int array[], int size) {
    int i, key, j;

    for (i = 0; i < size; i++) {
        key = array[i];
        j = i - 1;
        while (j >= 0 && array[j]>key)
        {
            array[j + 1] = array[j];
            j = j - 1;
        }
        array[j + 1] = key;
    }
}

void SelectionSort(int arr[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int minIndex = i; // 최솟값을 가진 원소의 인덱스를 저장

        // 배열의 나머지 부분에서 최솟값을 찾음
        for (int j = i + 1; j < n; j++) {
            if (arr[j] < arr[minIndex]) {
                minIndex = j;
            }
        }

        // 최솟값을 현재 위치로 옮김
        int temp = arr[i];
        arr[i] = arr[minIndex];
        arr[minIndex] = temp;
    }
}

// 배열의 두 원소를 교환하는 함수
void swap(int* a, int* b) {
    int temp = *a;
    *a = *b;
    *b = temp;
}

// 피벗을 정하고 배열을 분할하는 함수
int partition(int arr[], int low, int high) {
    int pivot = arr[high]; // 피벗을 배열의 마지막 원소로 선택
    int i = (low - 1);

    for (int j = low; j < high; j++) {
        if (arr[j] < pivot) {
            i++;
            swap(&arr[i], &arr[j]);
        }
    }

    swap(&arr[i + 1], &arr[high]);
    return (i + 1);
}

// 퀵 정렬을 수행하는 함수
void quickSort(int arr[], int low, int high) {
    if (low < high) {
        int pi = partition(arr, low, high);

        quickSort(arr, low, pi - 1); // 피벗 왼쪽 부분 정렬
        quickSort(arr, pi + 1, high); // 피벗 오른쪽 부분 정렬
    }
}

// 두 하위 배열을 병합하는 함수
void merge(int arr[], int left, int middle, int right) {
    int i, j, k;
    int n1 = middle - left + 1;
    int n2 = right - middle;

    // 두 개의 임시 배열을 생성하고 데이터 복사
    int* L, * R;
    L = new int[sizeof(int) * n1];
    R = new int[sizeof(int) * n2];
    for (i = 0; i < n1; i++)
        L[i] = arr[left + i];
    for (j = 0; j < n2; j++)
        R[j] = arr[middle + 1 + j];

    // 두 임시 배열을 병합
    i = 0;
    j = 0;
    k = left;
    while (i < n1 && j < n2) {
        if (L[i] <= R[j]) {
            arr[k] = L[i];
            i++;
        }
        else {
            arr[k] = R[j];
            j++;
        }
        k++;
    }

    // 남은 원소들을 복사
    while (i < n1) {
        arr[k] = L[i];
        i++;
        k++;
    }
    while (j < n2) {
        arr[k] = R[j];
        j++;
        k++;
    }
}

// 병합 정렬을 수행하는 함수
void mergeSort(int arr[], int left, int right) {
    if (left < right) {
        int middle = left + (right - left) / 2;

        // 두 부분 배열을 정렬
        mergeSort(arr, left, middle);
        mergeSort(arr, middle + 1, right);

        // 정렬된 두 부분 배열을 병합
        merge(arr, left, middle, right);
    }
}
//int main() {
//    int Arr[] = { 64, 34, 25, 12, 22, 11, 90 };
//    int Size = sizeof(Arr) / sizeof(Arr[0]);
//    //InsertSort(Arr, Size);
//    //BubbleSort(Arr, Size);
//    //SelectionSort(Arr, Size);
//    //quickSort(Arr, 0,6);
//    mergeSort(Arr, 0, 6);
//    // 정렬된 배열 출력
//    printf("버블 정렬 후 배열: ");
//    for (int i = 0; i < Size; i++) {
//        printf("%d ", Arr[i]);
//    }
//   
//
//    return 0;
//}

struct 장비
{
    int ATT;
    int DEF;
};

struct node
{
    int data;
    struct node* pre;
    struct node* next;
    struct 장비 Data;
};

struct node* CreateNode(int data,int a1,int a2)
{
    struct node* node = new struct node;
    node->data = data;    
    node->pre = NULL;
    node->next = NULL;
    node->Data.ATT = a1;
    node->Data.DEF = a2;
    return node;
};

void Linked(struct node* n1, struct node*n2) 
{
    n1->next = n2;
    n2->pre = n1;
}

void Deleted(struct node* n1) 
{
    delete(n1);
}

int main() 
{
    struct node* head;

    struct node* node_1 = CreateNode(10,11,12);
    struct node* node_2 = CreateNode(20,21,22);
    struct node* node_3 = CreateNode(30,31,32);
    struct node* node_4 = CreateNode(40,41,42);

    head = node_1;
    Linked(node_1, node_2);
    Linked(node_2, node_3);
    Linked(node_3, node_4);

    struct  node* cur = head;
    while (cur != NULL)
    {
        printf("%d,%d,%d -> ", cur->data,cur->Data.ATT,cur->Data.DEF);
        cur = cur->next;
    }
    printf("NULL\n");
    
    cur = node_4;
    while (cur != NULL)
    {
        printf("%d -> ", cur->data);
        cur = cur->pre;
    }
    printf("NULL\n");
    cur = head;


    Linked(node_2, node_4);
    Deleted(node_3);

    while (cur != NULL)
    {
        printf("%d -> ", cur->data);
        cur = cur->next;
    }
    printf("NULL\n");
    return 0;
}