#include<stdio.h>
#include<iostream>

int linearSearch(int arr[], int n, int key) { // 선형탐색
	for (int i = 0; i < n; i++) {
		if (arr[i] == key) {
			return i;
		}
	}
	return -1;
}

struct TreeNode
{
	int Data;
	TreeNode* left;
	TreeNode* right;

	TreeNode(int data) : Data(data), left(nullptr), right(nullptr) {}
};

TreeNode* insertNode(TreeNode* root, int data) // 이진트리 만들기
{
	if (root == nullptr)
	{
		return new TreeNode(data);
	}

	if (data < root->Data)
	{
		root->left = insertNode(root->left, data);
	}
	else if (data > root->Data)
	{
		root->right = insertNode(root->right, data);
	}

	return root;
}

void inorderT(TreeNode* root) // 중위 순회
{
	if (root != nullptr)
	{
		inorderT(root->left);
		std::cout << root->Data << ",";
		inorderT(root->right);
	}
}

void preorderT(TreeNode* root) // 전위 순회
{
	if (root != nullptr)
	{
		std::cout << root->Data << ",";
		preorderT(root->left);
		preorderT(root->right);
	}
}

void PostorderT(TreeNode* root) // 후위 순회
{
	if (root != nullptr)
	{
		PostorderT(root->left);
		PostorderT(root->right);
		std::cout << root->Data << ",";
	}
}
#define MAX_SIZE 100

struct MaxHeap {
	int data[MAX_SIZE];
	int size;
};

void InitHeap(struct MaxHeap* heap)
{
	heap->size = 0;
}

void InsertHeap(struct MaxHeap* heap, int value)
{
	int Index;
	if (heap->size >= (MAX_SIZE - 1))
	{
		std::cout << "힙이 가득 찼습니다." << std::endl;
	}
	else {
		heap->size++;
		Index = heap->size;
		heap->data[Index] = value;

		while ((Index != 1) && (heap->data[Index] > heap->data[Index / 2]))
		{
			//부모와 자리 바꾸기
			int temp = heap->data[Index / 2];
			heap->data[Index / 2] = heap->data[Index];
			heap->data[Index] = temp;

			Index = Index / 2;
		}

	}
}

int DeleteHeap(struct MaxHeap* heap)
{
	int cur, child;
	int deldata = heap->data[1];

	if (heap->size == 0)
	{
		std::cout << "힙이 없습니다." << std::endl;
		return 0;
	}
	else {
		heap->data[1] = heap->data[heap->size];
		heap->size--;

		cur = 1;
		// 부모 자식 간 노드 비교 후 위치 교환
		while ((cur * 2) <= heap->size)
		{
			child = cur * 2;

			if (((child + 1) <= heap->size) && (heap->data[child] < heap->data[child + 1]))
			{
				child++;
			}

			if (heap->data[child] <= heap->data[cur])
				break;

			int temp = heap->data[child];
			heap->data[child] = heap->data[cur];
			heap->data[cur] = temp;

			cur = child;
		}
	}

	return deldata;
}


void HeapPrint(struct MaxHeap* heap)
{
	std::cout << "힙";
	for (int i = 1; i < heap->size + 1; i++)
	{
		std::cout << heap->data[i] << " ";
	}
	std::cout << std::endl;
}

//#define MAX_SIZE 100
//
//struct MaxHeap
//{
//	int data[MAX_SIZE];
//	int size;
//};
//
//void InitHeap(struct MaxHeap * heap)
//{
//	heap->size = -1;
//}
//
//void InsertHeap(struct MaxHeap* heap, int value)
//{
//	int index;
//	if (heap->size >= (MAX_SIZE - 1))
//	{
//		std::cout << "힙이 가득 찼습니다." << std::endl;
//	}
//	else
//	{
//		heap->size++;
//		index = heap->size;
//		heap->data[index] = value;
//
//		while ((index != 0) && (heap->data[index] > heap->data[index/2]))
//		{
//			//부모와 자리 바꾸기
//			int temp = heap->data[index / 2];
//			heap->data[index / 2] = heap->data[index];
//			heap->data[index] = temp;
//
//			index = index / 2;
//		}
//	}
//
//}
//
//void HeapPrint(struct  MaxHeap* heap)
//{
//	std::cout << "힙";
//	for (int i = 0; i <= heap->size; i++)
//	{
//		std::cout << heap->data[i] << " ";
//	}
//	std::cout << std::endl;
//
//};
//
//int DeleteHeap(struct MaxHeap* heap)
//{
//	int cur, child;
//	int deldata = heap->data[0];
//
//	if (heap->size == 0)
//	{
//		std::cout << "힙이 없습니다." << std::endl;
//		return 0;
//	}
//	else
//	{
//		heap->data[0] = heap->data[heap->size];
//		heap->size--;
//
//		cur = 1;
//		// 부모자식간 노드 비교 후 위치 교환
//		while ((cur * 2) <= heap->size)
//		{
//			child = cur * 2;
//			if ((child ) <= heap->size && (heap->data[child] < heap->data[child ]))
//			{
//				child++;
//			}
//
//			if (heap->data[child] <= heap->data[cur])
//				break;
//
//			int temp = heap->data[child];
//			heap->data[child] = heap->data[cur];
//			heap->data[cur] = temp;
//
//			cur = child;
//		}
//	}
//	return deldata;
//}


int main()
{
	int arr[] = { 12, 45, 23, 6, 78, 11, 3, 56, 67, 9 };
	int n = sizeof(arr) / sizeof(arr[0]);
	int key = 56;

	int result = linearSearch(arr, n, key);

	if (result != -1) {
		std::cout << "요소 " << key << "은 배열의 인덱스 " << result << "에 있습니다." << std::endl;
	}
	else
	{
		std::cout << "요소를 못찾음" << std::endl;
	}

	TreeNode* root = nullptr;
	root = insertNode(root, 23);

	for (int i = 0; i < n; i++)
	{
		insertNode(root, arr[i]);
	}
	std::cout << "중위" << std::endl;
	inorderT(root);
	std::cout << std::endl;
	std::cout << "전위" << std::endl;
	preorderT(root);
	std::cout << std::endl;
	std::cout << "후위" << std::endl;
	PostorderT(root);
	std::cout << std::endl;

	struct MaxHeap maxHeap;
	InitHeap(&maxHeap);
	std::cout << std::endl;
	


	for (int i = 0; i < n; i++)
	{
		InsertHeap(&maxHeap, arr[i]);
	}
	std::cout << std::endl;
	HeapPrint(&maxHeap);
	std::cout << std::endl;
	int heapsize = maxHeap.size;
	for (int i = 0; i < heapsize; i++)
	{
		std::cout << DeleteHeap(&maxHeap) << std::endl;
	}

	return 0;
}

